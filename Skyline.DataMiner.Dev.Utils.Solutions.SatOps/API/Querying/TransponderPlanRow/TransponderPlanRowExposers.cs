namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlanRow
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using System;

    /// <summary>
    /// Provides exposers for querying <see cref="TransponderPlanRow"/> objects.
    /// </summary>
    /// <remarks>
    /// Exposers define how properties of <see cref="TransponderPlanRow"/> objects are exposed for use in queries.
    /// </remarks>
    public static class TransponderPlanRowExposers
    {
        /// <summary>
        /// Exposes the <see cref="TransponderPlanRow.TransponderPlan"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlanRow, Guid> TransponderPlan = new Exposer<TransponderPlanRow, Guid>((obj) => obj.TransponderPlan.Value, "TransponderPlan");

        /// <summary>
        /// Exposes the <see cref="TransponderPlanRow.Bandwidth"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlanRow, double> Bandwidth = new Exposer<TransponderPlanRow, double>((obj) => obj.Bandwidth.Value, "Bandwidth");

        /// <summary>
        /// Exposes the <see cref="TransponderPlanRow.StepSize"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlanRow, double> StepSize = new Exposer<TransponderPlanRow, double>((obj) => obj.StepSize.Value, "StepSize");

        /// <summary>
        /// Exposes the <see cref="TransponderPlanRow.Offset"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlanRow, double> Offset = new Exposer<TransponderPlanRow, double>((obj) => obj.Offset.Value, "Offset");

        /// <summary>
        /// Exposes the <see cref="TransponderPlanRow.Limit"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlanRow, double> Limit = new Exposer<TransponderPlanRow, double>((obj) => obj.Limit.Value, "Limit");
    }
}
