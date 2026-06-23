namespace Skyline.DataMiner.SDM.SatOps.Common.Models
{
    using System;

    /// <summary>
    /// Represents a transponder plan that defines the scheduling and configuration for a transponder.
    /// </summary>
    // [GenerateExposers]
    // [SdmDomStorage("(slc)satellite_management")]
    public class TransponderPlan : SdmObject<TransponderPlan>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the transponder plan.
        /// </summary>
        public Guid TransponderPlanId { get; set; }

        /// <summary>
        /// Gets or sets the name of the transponder plan.
        /// </summary>
        public string TransponderPlanName { get; set; }

        /// <summary>
        /// Gets or sets the start time of the transponder plan.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the transponder plan.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transponder plan is permanent.
        /// </summary>
        public bool IsPermanent { get; set; }

        /// <summary>
        /// Gets or sets the default slot size for the transponder plan.
        /// </summary>
        public double DefaultSlotSize { get; set; }

        /// <summary>
        /// Gets or sets the reference to the associated transponder.
        /// </summary>
        public SdmObjectReference<Transponder> Transponder { get; set; }
    }
}
