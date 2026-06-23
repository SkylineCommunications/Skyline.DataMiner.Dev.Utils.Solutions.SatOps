namespace Skyline.DataMiner.SDM.SatOps.Common.Models
{
    using System;
    
    /// <summary>
    /// Represents a row in a transponder plan.
    /// </summary>
    // [GenerateExposers]
    // [SdmDomStorage("(slc)satellite_management")]
    public class TransponderPlanRow : SdmObject<TransponderPlanRow>
    {
        /// <summary>
        /// Gets or sets the unique identifier for this transponder plan row.
        /// </summary>
        public Guid TransponderPlanRowId { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent transponder plan.
        /// </summary>
        public SdmObjectReference<TransponderPlan> TransponderPlan { get; set; }

        /// <summary>
        /// Gets or sets the bandwidth value.
        /// </summary>
        public double Bandwidth { get; set; }

        /// <summary>
        /// Gets or sets the step size value.
        /// </summary>
        public double StepSize { get; set; }

        /// <summary>
        /// Gets or sets the offset value.
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// Gets or sets the limit value.
        /// </summary>
        public double Limit { get; set; }

    }
}
