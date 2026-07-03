namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying.TransponderPlan
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using System;

    /// <summary>
    /// Provides exposers for querying <see cref="TransponderPlan"/> objects.
    /// </summary>
    /// <remarks>
    /// Exposers define how properties of <see cref="TransponderPlan"/> objects are exposed for use in queries.
    /// </remarks>
    public static class TransponderPlanExposers
    {
        /// <summary>
        /// Exposes the <see cref="TransponderPlan.Name"/> property for querying as "PlanName".
        /// </summary>
        public static readonly Exposer<TransponderPlan, string> PlanName = new Exposer<TransponderPlan, string>((obj) => obj.Name, "PlanName");

        /// <summary>
        /// Exposes the <see cref="TransponderPlan.StartTime"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlan, DateTime> StartTime = new Exposer<TransponderPlan, DateTime>((obj) => obj.StartTime.Value, "StartTime");

        /// <summary>
        /// Exposes the <see cref="TransponderPlan.EndTime"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlan, DateTime> EndTime = new Exposer<TransponderPlan, DateTime>((obj) => obj.EndTime.Value, "EndTime");

        /// <summary>
        /// Exposes the <see cref="TransponderPlan.IsPermanent"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlan, bool> IsPermanent = new Exposer<TransponderPlan, bool>((obj) => obj.IsPermanent.Value, "IsPermanent");

        /// <summary>
        /// Exposes the <see cref="TransponderPlan.DefaultSlotSize"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlan, double> DefaultSlotSize = new Exposer<TransponderPlan, double>((obj) => obj.DefaultSlotSize.Value, "DefaultSlotSize");

        /// <summary>
        /// Exposes the <see cref="TransponderPlan.Transponder"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderPlan, Guid> Transponder = new Exposer<TransponderPlan, Guid>((obj) => obj.Transponder.Value, "Transponder");
    }
}
