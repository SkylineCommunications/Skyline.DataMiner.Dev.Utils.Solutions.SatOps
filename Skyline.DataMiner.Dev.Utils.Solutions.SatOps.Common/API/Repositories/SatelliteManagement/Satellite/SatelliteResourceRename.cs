namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;

    internal sealed class SatelliteResourceRename
    {
        private readonly IMediaOpsPlanApi planApi;

        internal SatelliteResourceRename(IMediaOpsPlanApi planApi)
        {
            this.planApi = planApi ?? throw new ArgumentNullException(nameof(planApi));
        }

        internal Satellite Execute(IEnumerable<Transponder> transponders, string name, Func<Satellite> saveSatellite)
        {
            var resources = transponders
                .Where(item => item.DOMResource.HasValue && item.DOMResource.Value != Guid.Empty)
                .Select(item => item.DOMResource.Value)
                .Distinct()
                .Select(id => planApi.Resources.Read(id)
                    ?? throw new InvalidOperationException($"Cannot rename satellite '{name}': linked resource '{id}' was not found."))
                .ToList();

            if (resources.Count == 0)
                return saveSatellite();

            var capability = planApi.Capabilities.Read(PredefinedGuids.SatelliteCapabilityGuid)
                ?? throw new InvalidOperationException($"Cannot rename satellite '{name}': the Satellite capability was not found.");
            var settings = resources.Select(resource => GetSatelliteSettings(resource, capability.Id)).ToList();
            var originalValues = settings.Select(setting => setting.Discretes.ToList()).ToList();
            var changedResources = new List<int>();

            // Keep old options: existing jobs can still refer to them and must not be rewritten here.
            if (!capability.Discretes.Contains(name))
            {
                capability.AddDiscrete(name);
                planApi.Capabilities.Update(capability);
            }

            try
            {
                for (int index = 0; index < resources.Count; index++)
                {
                    if (originalValues[index].SequenceEqual(new[] { name }))
                        continue;

                    settings[index].SetDiscretes(new[] { name });
                    changedResources.Add(index);
                    planApi.Resources.Update(resources[index]);
                }

                return saveSatellite();
            }
            catch (Exception failure)
            {
                RestoreResources(resources, settings, originalValues, changedResources, name, failure);
                throw;
            }
        }

        private void RestoreResources(
            IReadOnlyList<Resource> resources,
            IReadOnlyList<CapabilitySettings> settings,
            IReadOnlyList<List<string>> originalValues,
            IEnumerable<int> changedResources,
            string name,
            Exception failure)
        {
            var errors = new List<Exception> { failure };
            foreach (var index in changedResources.Reverse())
            {
                try
                {
                    settings[index].SetDiscretes(originalValues[index]);
                    planApi.Resources.Update(resources[index]);
                }
                catch (Exception rollbackFailure)
                {
                    errors.Add(new InvalidOperationException(
                        $"Could not restore Satellite capability on resource '{resources[index].Id}'.", rollbackFailure));
                }
            }

            if (errors.Count > 1)
                throw new AggregateException($"Satellite '{name}' rename failed and resource rollback was incomplete.", errors);
        }

        private static CapabilitySettings GetSatelliteSettings(Resource resource, Guid capabilityId)
        {
            return resource.Capabilities.SingleOrDefault(setting => setting.Id == capabilityId)
                ?? throw new InvalidOperationException(
                    $"Cannot rename satellite: linked resource '{resource.Id}' has no Satellite capability.");
        }
    }
}
