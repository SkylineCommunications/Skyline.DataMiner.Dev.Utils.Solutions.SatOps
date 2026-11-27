namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder
{
    using System;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;

    internal sealed class TransponderResourceLifecycle
    {
        private readonly IResourcesRepository resources;

        internal TransponderResourceLifecycle(IResourcesRepository resources)
        {
            this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        internal Transponder Transition(Guid resourceId, ResourceState targetState, Func<Transponder> transitionTransponder)
        {
            var resource = resources.Read(resourceId)
                ?? throw new InvalidOperationException($"Cannot change transponder state: linked resource '{resourceId}' was not found.");
            var originalState = resource.State;
            if (originalState == targetState)
                return transitionTransponder();

            if (originalState != ResourceState.Complete && originalState != ResourceState.Deprecated)
                throw new InvalidOperationException(
                    $"Cannot change transponder state: linked resource '{resourceId}' is '{originalState}', not Complete or Deprecated.");

            var transitionedResource = ChangeState(resource, targetState);
            try
            {
                return transitionTransponder();
            }
            catch (Exception failure)
            {
                try
                {
                    ChangeState(transitionedResource, originalState);
                }
                catch (Exception rollbackFailure)
                {
                    throw new AggregateException(
                        $"Transponder state change failed and resource '{resourceId}' could not be restored to '{originalState}'.",
                        failure, rollbackFailure);
                }

                throw;
            }
        }

        private Resource ChangeState(Resource resource, ResourceState targetState)
        {
            switch (targetState)
            {
                case ResourceState.Deprecated:
                    return resources.Deprecate(resource);
                case ResourceState.Complete:
                    return resources.Restore(resource);
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetState), targetState, "Unsupported transponder resource state.");
            }
        }
    }
}
