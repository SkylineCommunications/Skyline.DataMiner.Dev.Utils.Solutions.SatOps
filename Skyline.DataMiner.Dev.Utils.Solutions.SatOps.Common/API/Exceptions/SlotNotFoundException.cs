namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions
{
    using System;

    /// <summary>The slot selected for update no longer exists.</summary>
    public sealed class SlotNotFoundException : InvalidOperationException
    {
        /// <summary>Initializes a missing-slot failure.</summary>
        /// <param name="slotId">The missing slot identifier.</param>
        public SlotNotFoundException(Guid slotId)
            : base("The selected slot no longer exists. Refresh the slot list and try again.")
        {
            SlotId = slotId;
        }

        /// <summary>Gets the missing slot identifier.</summary>
        public Guid SlotId { get; }
    }
}
