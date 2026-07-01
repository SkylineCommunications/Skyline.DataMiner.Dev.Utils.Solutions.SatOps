namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Profiles;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Constants;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TransponderResourceCreationMiddleware : IBulkCreatableMiddleware<Transponder>, IBulkUpdatableMiddleware<Transponder>
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
            CreateResource(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<Transponder> OnCreate(IEnumerable<Transponder> oToCreate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            var transponders = oToCreate.ToList();
            foreach (var transponder in transponders)
                CreateResource(transponder);
            return next(transponders);
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


        private void CreateResource(Transponder transponder)
        {
            var satelliteName = _satelliteResolver(transponder.TransponderSatellite.Value).Name;

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

            var resource = _mediaOpsPlanApi.Resources.Read(transponder.DOMResource.Value);

            try
            {
                if (resource == null)
                {
                    _logger?.Error($"Resource not found for transponder '{transponder.Name}' (Id: {transponder.Id}).");
                    return;
                }

                var changed = false;

                if (resource.Name != transponder.Name)
                {
                    resource.Name = transponder.Name;
                    changed = true;
                }

                var satelliteName = _satelliteResolver(transponder.TransponderSatellite.Value).Name;
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
            catch (Exception e)
            {
                _logger?.Error($"Failed to update resource for transponder '{transponder.Name}' (Resource ID: {transponder.DOMResource}).", e);
            }
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
