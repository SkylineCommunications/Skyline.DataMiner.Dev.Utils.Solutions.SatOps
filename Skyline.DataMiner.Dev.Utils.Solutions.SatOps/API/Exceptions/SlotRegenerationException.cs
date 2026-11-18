namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Slot replacement failed after persistence started and may have applied partially.</summary>
    public sealed class SlotRegenerationException : InvalidOperationException
    {
        internal SlotRegenerationException(IEnumerable<Guid> planIds, string stage, Exception innerException)
            : base("Slot regeneration could not be completed. Some slots may have changed. Refresh the slot list before trying again.", innerException)
        {
            PlanIds = planIds.ToList().AsReadOnly();
            Stage = stage;
        }

        /// <summary>Gets the plans involved in the replacement.</summary>
        public IReadOnlyCollection<Guid> PlanIds { get; }

        /// <summary>Gets the persistence stage that failed.</summary>
        public string Stage { get; }
    }
}
