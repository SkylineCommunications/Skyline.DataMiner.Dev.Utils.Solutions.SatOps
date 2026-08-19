namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;

    /// <summary>
    /// Represents a repository for managing <see cref="TransponderSlot"/> entities.
    /// </summary>
    public interface ITransponderSlotRepository : IRepository<TransponderSlot>
    {
        /// <summary>
        /// Generates slots for the specified transponder plan.
        /// Deletes any existing slots linked to the plan before creating the new ones.
        /// </summary>
        /// <param name="transponderPlanId">The unique identifier of the transponder plan.</param>
        /// <returns>The collection of created <see cref="TransponderSlot"/> instances.</returns>
        IReadOnlyCollection<TransponderSlot> GenerateSlots(Guid transponderPlanId);

        /// <summary>
        /// Generates slots for the specified transponder plan.
        /// Deletes any existing slots linked to the plan before creating the new ones.
        /// </summary>
        /// <param name="transponderPlan">The transponder plan to generate slots for.</param>
        /// <returns>The collection of created <see cref="TransponderSlot"/> instances.</returns>
        IReadOnlyCollection<TransponderSlot> GenerateSlots(TransponderPlan transponderPlan);

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
