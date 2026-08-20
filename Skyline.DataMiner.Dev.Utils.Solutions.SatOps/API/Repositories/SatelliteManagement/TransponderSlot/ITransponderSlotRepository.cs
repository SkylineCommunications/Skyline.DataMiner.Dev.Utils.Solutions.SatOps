namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;

    /// <summary>
    /// Represents a repository for managing <see cref="TransponderSlot"/> entities.
    /// </summary>
    public interface ITransponderSlotRepository : IBulkCreatableRepository<TransponderSlot>, IBulkDeletableRepository<TransponderSlot>, 
        IPageableRepository<TransponderSlot>, ICountableRepository<TransponderSlot>
    {
        /// <summary>
        /// Generates the slots of the specified transponder plan and persists them.
        /// The slots are calculated from the plan rows and the transponder of the plan.
        /// Any slots that already belong to the plan are removed first.
        /// </summary>
        /// <param name="transponderPlanId">The unique identifier of the transponder plan.</param>
        /// <returns>The collection of created <see cref="TransponderSlot"/> instances.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="transponderPlanId"/> is an empty GUID.</exception>
        IReadOnlyCollection<TransponderSlot> Create(Guid transponderPlanId);

        /// <summary>
        /// Reads all slots that belong to the specified transponder plan.
        /// </summary>
        /// <param name="transponderPlanId">The unique identifier of the transponder plan.</param>
        /// <returns>An enumerable of <see cref="TransponderSlot"/> instances for the plan.</returns>
        IEnumerable<TransponderSlot> ReadByTransponderPlan(Guid transponderPlanId);

        /// <summary>
        /// Deletes all slots that belong to the specified transponder plan.
        /// </summary>
        /// <param name="transponderPlanId">The unique identifier of the transponder plan.</param>
        void DeleteSlotsByTransponderPlan(Guid transponderPlanId);
    }
}
