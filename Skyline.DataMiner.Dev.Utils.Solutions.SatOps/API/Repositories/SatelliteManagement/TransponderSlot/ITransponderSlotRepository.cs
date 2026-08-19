namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;

    /// <summary>
    /// Represents a repository for managing <see cref="TransponderSlot"/> entities.
    /// </summary>
    public interface ITransponderSlotRepository : IRepository<TransponderSlot>
    {
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

    /// <summary>
    /// Calculates the slots of a transponder plan. Implemented by the transponder slot repository so that the
    /// create pipeline can expand a slot generation request, without exposing the calculation on the public API.
    /// </summary>
    internal interface ITransponderSlotBuilder
    {
        /// <summary>
        /// Calculates the slots for the specified transponder plan without persisting or deleting anything.
        /// </summary>
        /// <param name="transponderPlanId">The unique identifier of the transponder plan.</param>
        /// <returns>The collection of calculated, not yet persisted, <see cref="TransponderSlot"/> instances.</returns>
        IReadOnlyCollection<TransponderSlot> BuildSlots(Guid transponderPlanId);
    }
}
