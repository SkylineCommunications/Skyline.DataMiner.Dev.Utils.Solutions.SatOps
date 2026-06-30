namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;

    /// <summary>
    /// Represents a repository for managing <see cref="TransponderPlan"/> entities.
    /// </summary>
    public interface ITransponderPlanRepository : IRepository<TransponderPlan>
    {
        /// <summary>
        /// Initializes a new <see cref="TransponderPlan"/> instance.
        /// </summary>
        /// <returns>A new <see cref="TransponderPlan"/> instance.</returns>
        TransponderPlan Initialize();

        /// <summary>
        /// Activates a transponder plan by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transponder plan to activate.</param>
        /// <returns>The activated <see cref="TransponderPlan"/>.</returns>
        TransponderPlan Activate(Guid id);

        /// <summary>
        /// Activates the specified transponder plan.
        /// </summary>
        /// <param name="transponderPlan">The transponder plan to activate.</param>
        /// <returns>The activated <see cref="TransponderPlan"/>.</returns>
        TransponderPlan Activate(TransponderPlan transponderPlan);

        /// <summary>
        /// Activates multiple transponder plans.
        /// </summary>
        /// <param name="transponderPlans">The collection of transponder plans to activate.</param>
        /// <returns>A read-only collection of activated transponder plans.</returns>
        IReadOnlyCollection<TransponderPlan> Activate(IEnumerable<TransponderPlan> transponderPlans);

        /// <summary>
        /// Activates multiple transponder plans by their unique identifiers.
        /// </summary>
        /// <param name="transponderPlanIds">The collection of transponder plan identifiers to activate.</param>
        /// <returns>A read-only collection of activated transponder plans.</returns>
        IReadOnlyCollection<TransponderPlan> Activate(IEnumerable<Guid> transponderPlanIds);

        /// <summary>
        /// Deprecates a transponder plan by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transponder plan to deprecate.</param>
        /// <returns>The deprecated <see cref="TransponderPlan"/>.</returns>
        TransponderPlan Deprecate(Guid id);

        /// <summary>
        /// Deprecates the specified transponder plan.
        /// </summary>
        /// <param name="transponderPlan">The transponder plan to deprecate.</param>
        /// <returns>The deprecated <see cref="TransponderPlan"/>.</returns>
        TransponderPlan Deprecate(TransponderPlan transponderPlan);

        /// <summary>
        /// Deprecates multiple transponder plans.
        /// </summary>
        /// <param name="transponderPlans">The collection of transponder plans to deprecate.</param>
        /// <returns>A read-only collection of deprecated transponder plans.</returns>
        IReadOnlyCollection<TransponderPlan> Deprecate(IEnumerable<TransponderPlan> transponderPlans);

        /// <summary>
        /// Deprecates multiple transponder plans by their unique identifiers.
        /// </summary>
        /// <param name="transponderPlanIds">The collection of transponder plan identifiers to deprecate.</param>
        /// <returns>A read-only collection of deprecated transponder plans.</returns>
        IReadOnlyCollection<TransponderPlan> Deprecate(IEnumerable<Guid> transponderPlanIds);

        /// <summary>
        /// Reactivates a transponder plan by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transponder plan to reactivate.</param>
        /// <returns>The reactivated <see cref="TransponderPlan"/>.</returns>
        TransponderPlan Reactivate(Guid id);

        /// <summary>
        /// Reactivates the specified transponder plan.
        /// </summary>
        /// <param name="transponderPlan">The transponder plan to reactivate.</param>
        /// <returns>The reactivated <see cref="TransponderPlan"/>.</returns>
        TransponderPlan Reactivate(TransponderPlan transponderPlan);

        /// <summary>
        /// Reactivates multiple transponder plans.
        /// </summary>
        /// <param name="transponderPlans">The collection of transponder plans to reactivate.</param>
        /// <returns>A read-only collection of reactivated transponder plans.</returns>
        IReadOnlyCollection<TransponderPlan> Reactivate(IEnumerable<TransponderPlan> transponderPlans);

        /// <summary>
        /// Reactivates multiple transponder plans by their unique identifiers.
        /// </summary>
        /// <param name="transponderPlanIds">The collection of transponder plan identifiers to reactivate.</param>
        /// <returns>A read-only collection of reactivated transponder plans.</returns>
        IReadOnlyCollection<TransponderPlan> Reactivate(IEnumerable<Guid> transponderPlanIds);
    }
}
