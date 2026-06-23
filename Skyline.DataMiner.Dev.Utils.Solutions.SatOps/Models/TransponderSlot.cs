using System;

namespace Skyline.DataMiner.SDM.SatOps.Common.Models
{
    /// <summary>
    /// Represents a transponder slot within a transponder plan.
    /// </summary>
    // [GenerateExposers]
    // [SdmDomStorage("(slc)satellite_management")]
    public class TransponderSlot : SdmObject<TransponderSlot>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the transponder slot.
        /// </summary>
        public Guid TransponderSlotId { get; set; }

        /// <summary>
        /// Gets or sets the reference to the associated transponder plan.
        /// </summary>
        public SdmObjectReference<TransponderPlan> TransponderPlan { get; set; }

        /// <summary>
        /// Gets or sets the name of the transponder slot.
        /// </summary>
        public string TransponderSlotName { get; set; }

        /// <summary>
        /// Gets or sets the start frequency of the slot in MHz.
        /// </summary>
        public double SlotStartFrequency { get; set; }

        /// <summary>
        /// Gets or sets the stop frequency of the slot in MHz.
        /// </summary>
        public double SlotStopFrequency { get; set; }

        /// <summary>
        /// Gets or sets the bandwidth of the transponder slot in MHz.
        /// </summary>
        public double Bandwidth { get; set; }

        /// <summary>
        /// Gets or sets the uplink frequency in MHz.
        /// </summary>
        public double UplinkFrequency { get; set; }

        /// <summary>
        /// Gets or sets the downlink frequency in MHz.
        /// </summary>
        public double DownlinkFrequency { get; set; }
    }
}
