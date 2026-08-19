
namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Profiles;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ExceptionMessages = Logging.ExceptionMessages;

    internal sealed class TransponderResourceCreationMiddleware : IBulkCreatableMiddleware<Transponder>, IBulkUpdatableMiddleware<Transponder>, IBulkDeletableMiddleware<Transponder>
    {
        private readonly IConnection _connection;
        private readonly IMediaOpsPlanApi _mediaOpsPlanApi;
        private readonly Func<Guid, Satellite> _satelliteResolver;
        private readonly ProfileHelper _profileHelper;
        private readonly ILogger _logger;

        public TransponderResourceCreationMiddleware(IConnection connection, Func<Guid, Satellite> satelliteResolver, ILogger logger = null)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mediaOpsPlanApi = connection.GetMediaOpsPlanApi();
            _satelliteResolver = satelliteResolver ?? throw new ArgumentNullException(nameof(satelliteResolver));
            _profileHelper = new ProfileHelper(connection.HandleMessages);
            _logger = logger;
        }

        public Transponder OnCreate(Transponder oToCreate, Func<Transponder, Transponder> next)
        {
            CreateResource(oToCreate, ReadExistingResourceNames());

            try
            {
                return next(oToCreate);
            }
            catch
            {
                RollbackCreatedResources(new[] { oToCreate });
                throw;
            }
        }

        public IReadOnlyCollection<Transponder> OnCreate(IEnumerable<Transponder> oToCreate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            var transponders = oToCreate.ToList();

            // Read the taken names once for the whole batch; CreateResource also adds each new name to the
            // set so that duplicates inside the batch itself are rejected.
            var takenResourceNames = ReadExistingResourceNames();

            try
            {
                foreach (var transponder in transponders)
                    CreateResource(transponder, takenResourceNames);

                return next(transponders);
            }
            catch
            {
                RollbackCreatedResources(transponders);
                throw;
            }
        }

        public Transponder OnUpdate(Transponder oToUpdate, Func<Transponder, Transponder> next)
        {
            UpdateResource(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<Transponder> OnUpdate(IEnumerable<Transponder> oToUpdate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            var transponders = oToUpdate.ToList();
            foreach (var transponder in transponders)
                UpdateResource(transponder);
            return next(transponders);
        }

        public void OnDelete(Transponder oToDelete, Action<Transponder> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            // Capture the associated artifacts before the transponder is removed.
            var transponders = new[] { oToDelete };
            var resourceIds = CollectResourceIds(transponders);
            var satelliteNames = CollectSatelliteNames(transponders);

            // Delete the transponder first so that a resource-cleanup failure cannot strand the
            // transponder, and so there is no window where the transponder points at a deleted resource.
            next(oToDelete);

            DeleteResources(resourceIds);
            RemoveSatelliteDiscretes(satelliteNames);
        }

        public void OnDelete(IEnumerable<Transponder> oToDelete, Action<IEnumerable<Transponder>> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var transponders = oToDelete.ToList();

            // Capture the associated artifacts before the transponders are removed.
            var resourceIds = CollectResourceIds(transponders);
            var satelliteNames = CollectSatelliteNames(transponders);

            next(transponders);

            DeleteResources(resourceIds);
            RemoveSatelliteDiscretes(satelliteNames);
        }

        private static List<Guid> CollectResourceIds(IEnumerable<Transponder> transponders)
        {
            return transponders
                .Where(t => t?.DOMResource != null && t.DOMResource.Value != Guid.Empty)
                .Select(t => t.DOMResource.Value)
                .Distinct()
                .ToList();
        }

        private List<string> CollectSatelliteNames(IEnumerable<Transponder> transponders)
        {
            var names = new List<string>();

            foreach (var transponder in transponders)
            {
                if (transponder?.TransponderSatellite == null || transponder.TransponderSatellite.Value == Guid.Empty)
                    continue;

                try
                {
                    var satellite = _satelliteResolver(transponder.TransponderSatellite.Value);
                    if (satellite != null && !string.IsNullOrWhiteSpace(satellite.Name))
                        names.Add(satellite.Name);
                }
                catch (Exception e)
                {
                    _logger?.Error($"Failed to resolve satellite '{transponder.TransponderSatellite.Value}' for transponder '{transponder.Name}' during delete cleanup.", e);
                }
            }

            return names.Distinct().ToList();
        }

        private void DeleteResources(IReadOnlyCollection<Guid> resourceIds)
        {
            if (resourceIds == null || resourceIds.Count == 0)
                return;

            var ids = resourceIds.ToArray();

            // Deprecate first so the resource is immediately removed from active use, then attempt a hard
            // delete. If the delete is blocked (e.g. the resource is still referenced by active bookings),
            // leave the resource deprecated and log instead of failing the transponder delete.
            try
            {
                _mediaOpsPlanApi.Resources.Deprecate(ids);
            }
            catch (Exception e)
            {
                _logger?.Error($"Failed to deprecate transponder resource(s) '{string.Join(", ", ids)}'.", e);
            }

            try
            {
                _mediaOpsPlanApi.Resources.Delete(ids);
            }
            catch (Exception e)
            {
                _logger?.Error($"Transponder resource(s) '{string.Join(", ", ids)}' were deprecated but could not be deleted (likely still referenced by active bookings); left deprecated.", e);
            }
        }

        private void RemoveSatelliteDiscretes(IReadOnlyCollection<string> satelliteNames)
        {
            if (satelliteNames == null || satelliteNames.Count == 0)
                return;

            foreach (var satelliteName in satelliteNames)
            {
                if (string.IsNullOrWhiteSpace(satelliteName))
                    continue;

                try
                {
                    // Only remove the discrete when no remaining resource still references it. The
                    // "Satellite" capability is shared across every transponder resource, so a satellite
                    // used by another (still existing) transponder must keep its discrete.
                    var stillUsed = _mediaOpsPlanApi.Resources
                        .Read()
                        .Any(r => r.Capabilities?.Any(c => c.Discretes?.Any(d => d == satelliteName) ?? false) ?? false);

                    if (stillUsed)
                        continue;

                    var capability = _mediaOpsPlanApi.Capabilities
                        .Read()
                        .FirstOrDefault(c => c.Name == NamingConstants.SatelliteCapabilityName);

                    if (capability == null || !(capability.Discretes?.Any(d => d == satelliteName) ?? false))
                        continue;

                    try
                    {
                        capability = capability.RemoveDiscrete(satelliteName);
                        _mediaOpsPlanApi.Capabilities.Update(capability);
                    }
                    catch (Exception)
                    {
                        // The Plan API caches capability state internally; fall back to a DB-level
                        // ProfileHelper update, mirroring the create path (AddCapabilityViaProfileHelper).
                        RemoveSatelliteDiscreteViaProfileHelper(capability.Id, satelliteName);
                    }
                }
                catch (Exception e)
                {
                    _logger?.Error($"Failed to remove satellite discrete '{satelliteName}' from capability '{NamingConstants.SatelliteCapabilityName}'.", e);
                }
            }
        }

        private void RemoveSatelliteDiscreteViaProfileHelper(Guid capabilityId, string satelliteName)
        {
            var parameter = _profileHelper.ProfileParameters
                .Read(ParameterExposers.ID.Equal(capabilityId))
                .FirstOrDefault();

            if (parameter == null)
                return;

            var discretes = parameter.Discretes ?? new List<string>();
            if (discretes.Remove(satelliteName))
            {
                parameter.Discretes = discretes;
                _profileHelper.ProfileParameters.Update(parameter);
            }
        }

        private HashSet<string> ReadExistingResourceNames()
        {
            return new HashSet<string>(
                _mediaOpsPlanApi.Resources.Read()
                    .Where(r => r != null && !string.IsNullOrWhiteSpace(r.Name))
                    .Select(r => r.Name.Trim()),
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Removes the resources that were provisioned for the supplied transponders.
        /// </summary>
        /// <remarks>
        /// Called when persisting the transponders fails after their resources were already created.
        /// Without this the resource would linger and permanently block that name from being reused.
        /// </remarks>
        private void RollbackCreatedResources(IEnumerable<Transponder> transponders)
        {
            var created = transponders?.Where(t => t?.DOMResource != null && t.DOMResource.Value != Guid.Empty).ToList();
            if (created == null || created.Count == 0)
                return;

            var resourceIds = created.Select(t => t.DOMResource.Value).Distinct().ToArray();

            try
            {
                _mediaOpsPlanApi.Resources.Delete(resourceIds);
            }
            catch (Exception e)
            {
                _logger?.Error($"Failed to roll back transponder resource(s) '{string.Join(", ", resourceIds)}' after the transponder could not be persisted.", e);
                return;
            }

            foreach (var transponder in created)
                transponder.DOMResource = null;
        }

        private void CreateResource(Transponder transponder, HashSet<string> takenResourceNames)
        {
            if (transponder == null)
                throw new ArgumentNullException(nameof(transponder));

            if (string.IsNullOrWhiteSpace(transponder.Name))
                throw new ArgumentException("Transponder name is required.", nameof(transponder));

            // Resource names must be unique in DataMiner. Reject the collision before anything is provisioned,
            // otherwise Resources.Create fails halfway through and leaves capability/pool changes behind.
            if (takenResourceNames != null && !takenResourceNames.Add(transponder.Name.Trim()))
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderResourceNameAlreadyExists, transponder.Name), nameof(transponder));

            if (!transponder.Bandwidth.HasValue)
                throw new ArgumentException("Bandwidth is required.", nameof(transponder));

            if (!transponder.TransponderSatellite.HasValue || transponder.TransponderSatellite.Value == Guid.Empty)
                throw new ArgumentException("Transponder satellite ID is required.", nameof(transponder));

            var satellite = _satelliteResolver(transponder.TransponderSatellite.Value) ??
                throw new ArgumentException($"Satellite '{transponder.TransponderSatellite.Value}' not found for transponder '{transponder.Name}'.", nameof(transponder));
            var satelliteName = satellite.Name;

            Guid transponderCapabilityId;
            try
            {
                transponderCapabilityId = CreateOrGetTransponderCapability(satelliteName);
            }
            catch (Exception)
            {
                // The existing Plan API instance caches capability state internally.
                // After the ProfileHelper DB-level update, obtain a fresh instance so
                // that Resources.Create validation sees the newly added discrete.
                AddCapabilityViaProfileHelper(satelliteName);
                transponderCapabilityId = CreateOrGetTransponderCapability(satelliteName, _connection.GetMediaOpsPlanApi());
            }

            SetupSchedulingApp();
            var transponderCapacityId = CreateOrGetTransponderCapacity();
            var transponderResourcePoolId = CreateOrGetTransponderResourcePool();

            var resource = new UnmanagedResource
            {
                Name = transponder.Name,
                Concurrency = 100,
                IsExternallyManaged = true,
            }
            .AddCapability(new CapabilitySettings(transponderCapabilityId).SetDiscretes(
                new List<string> { satelliteName }
            ))
            .AddCapacity(new RangeCapacitySetting(transponderCapacityId)
            {
                MinValue = 0,
                MaxValue = (decimal)transponder.Bandwidth.Value,
            })
            .AssignToPool(transponderResourcePoolId);

            var createdResource = _mediaOpsPlanApi.Resources.Create(resource);
            _mediaOpsPlanApi.Resources.Complete(createdResource);

            transponder.DOMResource = createdResource.Id;
        }

        private void UpdateResource(Transponder transponder)
        {
            if (!transponder.DOMResource.HasValue)
                return;

            try
            {
                var resource = _mediaOpsPlanApi.Resources.Read(transponder.DOMResource.Value);

                if (resource == null)
                {
                    _logger?.Error($"Resource not found for transponder '{transponder.Name}' (Id: {transponder.Id}).");
                    return;
                }

                var changed = false;

                if (resource.Name != transponder.Name)
                {
                    EnsureResourceNameIsAvailable(transponder.Name, resource.Id);
                    resource.Name = transponder.Name;
                    changed = true;
                }

                if (!transponder.TransponderSatellite.HasValue)
                {
                    _logger?.Error($"Transponder satellite ID is not set for '{transponder.Name}'.");
                    return;
                }

                var satellite = _satelliteResolver(transponder.TransponderSatellite.Value);
                if (satellite == null)
                {
                    _logger?.Error($"Satellite '{transponder.TransponderSatellite.Value}' not found for transponder '{transponder.Name}'.");
                    return;
                }

                var satelliteName = satellite.Name;
                var transponderCapabilityId = CreateOrGetTransponderCapability(satelliteName);
                var transponderCapacityId = CreateOrGetTransponderCapacity();

                var capabilitySetting = resource.Capabilities.FirstOrDefault(c => c.Id == transponderCapabilityId);
                if (capabilitySetting == null)
                {
                    _logger?.Error($"Capability '{NamingConstants.SatelliteCapabilityName}' ({transponderCapabilityId}) not found on resource '{resource.Id}'.");
                    return;
                }

                var currentDiscretes = capabilitySetting.Discretes?.ToList() ?? new List<string>();
                var isSatelliteDiscreteCorrect = currentDiscretes.Count == 1 && currentDiscretes[0] == satelliteName;

                if (!isSatelliteDiscreteCorrect)
                {
                    capabilitySetting.SetDiscretes(new List<string> { satelliteName });
                    changed = true;
                }

                if (!(resource.Capacities.FirstOrDefault(c => c.Id == transponderCapacityId) is RangeCapacitySetting capacitySetting))
                {
                    _logger?.Error($"Capacity '{NamingConstants.TransponderBandwidthCapacityName}' ({transponderCapacityId}) not found on resource '{resource.Id}'.");
                    return;
                }

                var requestedMax = (decimal)transponder.Bandwidth.Value;
                if (capacitySetting.MaxValue != requestedMax)
                {
                    capacitySetting.MaxValue = requestedMax;
                    changed = true;
                }

                if (changed)
                    _mediaOpsPlanApi.Resources.Update(resource);
            }
            catch (ArgumentException)
            {
                // A resource name conflict is user-correctable; surface it instead of silently skipping the rename.
                throw;
            }
            catch (Exception e)
            {
                _logger?.Error($"Failed to update resource for transponder '{transponder.Name}' (Resource ID: {transponder.DOMResource.Value}).", e);
            }
        }

        private void EnsureResourceNameIsAvailable(string name, Guid resourceIdToIgnore)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            var conflicts = _mediaOpsPlanApi.Resources
                .Read()
                .Any(r => r != null
                    && r.Id != resourceIdToIgnore
                    && string.Equals(r.Name?.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (conflicts)
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderResourceNameAlreadyExists, name), nameof(name));
        }

        private Guid CreateOrGetTransponderCapability(string satelliteName, IMediaOpsPlanApi api = null)
        {
            var planApi = api ?? _mediaOpsPlanApi;

            var capabilities = planApi.Capabilities
                .Read()
                .Where(c => c.Name == NamingConstants.SatelliteCapabilityName)
                .ToList();

            var matched = capabilities.FirstOrDefault(c => c.Discretes?.Any(d => d == satelliteName) ?? false);
            if (matched != null)
            {
                return matched.Id;
            }

            var capability = capabilities.FirstOrDefault();
            if (capability != null)
            {
                capability = capability.AddDiscrete(satelliteName);
                planApi.Capabilities.Update(capability);
                return capability.Id;
            }

            var newCapability = new Capability(PredefinedGuids.SatelliteCapabilityGuid)
            {
                Name = NamingConstants.SatelliteCapabilityName,
            }
            .AddDiscrete(satelliteName);

            planApi.Capabilities.Create(newCapability);
            return newCapability.Id;
        }

        private Guid CreateOrGetTransponderCapacity()
        {
            var capacity = _mediaOpsPlanApi.Capacities
                .Read()
                .FirstOrDefault(c => c.Name == NamingConstants.TransponderBandwidthCapacityName);

            if (capacity != null)
                return capacity.Id;

            var newCapacity = new RangeCapacity(PredefinedGuids.TransponderBandwidthGuid)
            {
                Name = NamingConstants.TransponderBandwidthCapacityName
            };

            _mediaOpsPlanApi.Capacities.Create(newCapacity);
            return newCapacity.Id;
        }

        private void AddCapabilityViaProfileHelper(string satelliteName)
        {
            var capability = _mediaOpsPlanApi.Capabilities
                .Read()
                .FirstOrDefault(c => c.Name == NamingConstants.SatelliteCapabilityName) ??
                throw new InvalidOperationException($"Capability '{NamingConstants.SatelliteCapabilityName}' not found.\nCannot add discrete '{satelliteName}'.");

            var parameter = _profileHelper.ProfileParameters
                .Read(ParameterExposers.ID.Equal(capability.Id))
                .FirstOrDefault() ??
                throw new InvalidOperationException($"Parameter for capability '{NamingConstants.SatelliteCapabilityName}' not found.");

            var discretes = parameter.Discretes ?? new List<string>();
            if (!discretes.Contains(satelliteName))
            {
                discretes.Add(satelliteName);
                parameter.Discretes = discretes;
                _profileHelper.ProfileParameters.Update(parameter);
            }
        }

        private Guid CreateOrGetTransponderResourcePool()
        {
            var existingPool = _mediaOpsPlanApi.ResourcePools
                .Read()
                .FirstOrDefault(r => r.Name == NamingConstants.Transponders);

            if (existingPool != null)
                return existingPool.Id;

            var pool = new ResourcePool(PredefinedGuids.TransponderResourcePoolGuid)
            {
                Name = NamingConstants.Transponders,
                IconImage = "Transponder",
            };

            AssignBandwidthSizeConfiguration(pool);
            AssignSatelliteConfiguration(pool);

            _mediaOpsPlanApi.ResourcePools.Create(pool);
            _mediaOpsPlanApi.ResourcePools.Complete(pool);

            return pool.Id;
        }

        private void AssignBandwidthSizeConfiguration(ResourcePool pool)
        {
            var bandwidthSizeConfig = CreateOrGetBandwidthSizeConfiguration();
            pool.OrchestrationSettings.AddConfiguration(new NumberConfigurationSetting(bandwidthSizeConfig));
        }

        private void AssignSatelliteConfiguration(ResourcePool pool)
        {
            var satCapability = _mediaOpsPlanApi.Capabilities.Read()
                .FirstOrDefault(c => c.Name == NamingConstants.SatelliteCapabilityName);

            if (satCapability != null)
                pool.OrchestrationSettings.AddCapability(new CapabilitySetting(satCapability));
        }

        private NumberConfiguration CreateOrGetBandwidthSizeConfiguration()
        {
            var configuration = _mediaOpsPlanApi.Configurations
                .Read()
                .OfType<NumberConfiguration>()
                .FirstOrDefault(c => c.Name == NamingConstants.BandwidthSizeParameterName);

            if (configuration != null)
                return configuration;

            var bandwidthSizeConfig = new NumberConfiguration(PredefinedGuids.BandwidthSizeGuid)
            {
                Name = NamingConstants.BandwidthSizeParameterName,
                Units = "MHz",
                Decimals = 3,
                StepSize = 0.001m,
                RangeMin = 0,
            };

            _mediaOpsPlanApi.Configurations.Create(bandwidthSizeConfig);
            return bandwidthSizeConfig;
        }

        private void SetupSchedulingApp()
        {
            var propertiesHelper = new DomHelper(_connection.HandleMessages, SlcPropertiesIds.ModuleId);
            var properties = propertiesHelper.DomInstances.Read(DomInstanceExposers.Name.Equal(NamingConstants.PropertyInfoName));

            if (properties.Any())
            {
                return;
            }

            var propertyInfoInstance = new PropertyInstance
            {
                PropertyInfo =
                {
                    Name = NamingConstants.PropertyInfoName,
                    PropertyType = SlcPropertiesIds.Enums.PropertytypeEnum.String,
                    Scope = NamingConstants.PropertyInfoScope,
                    Default = string.Empty,
                },
                Layout =
                {
                    SectionName = NamingConstants.PropertyInfoScope,
                    Order = 1,
                },
            };

            propertyInfoInstance.Save(propertiesHelper);
        }
    }
}
