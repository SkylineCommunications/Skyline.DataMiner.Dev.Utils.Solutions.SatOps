namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderSlot
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using System;

    /// <summary>
    /// Provides exposers for querying <see cref="TransponderSlot"/> objects.
    /// </summary>
    /// <remarks>
    /// Exposers define how properties of <see cref="TransponderSlot"/> objects are exposed for use in queries.
    /// </remarks>
    public static class TransponderSlotExposers
    {
        /// <summary>
        /// Exposes the <see cref="TransponderSlot.TransponderPlan"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderSlot, Guid> TransponderPlan = new Exposer<TransponderSlot, Guid>((obj) => obj.TransponderPlan.Value, "TransponderPlan");

        /// <summary>
        /// Exposes the <see cref="TransponderSlot.Name"/> property for querying as "SlotName".
        /// </summary>
        public static readonly Exposer<TransponderSlot, string> SlotName = new Exposer<TransponderSlot, string>((obj) => obj.Name, "SlotName");

        /// <summary>
        /// Exposes the <see cref="TransponderSlot.SlotStartFrequency"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderSlot, double> SlotStartFrequency = new Exposer<TransponderSlot, double>((obj) => obj.SlotStartFrequency.Value, "SlotStartFrequency");

        /// <summary>
        /// Exposes the <see cref="TransponderSlot.SlotEndFrequency"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderSlot, double> SlotEndFrequency = new Exposer<TransponderSlot, double>((obj) => obj.SlotEndFrequency.Value, "SlotEndFrequency");

        /// <summary>
        /// Exposes the <see cref="TransponderSlot.Bandwidth"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderSlot, double> Bandwidth = new Exposer<TransponderSlot, double>((obj) => obj.Bandwidth.Value, "Bandwidth");

        /// <summary>
        /// Exposes the <see cref="TransponderSlot.UplinkFreq"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderSlot, double> UplinkFreq = new Exposer<TransponderSlot, double>((obj) => obj.UplinkFreq.Value, "UplinkFreq");

        /// <summary>
        /// Exposes the <see cref="TransponderSlot.DownlinkFreq"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderSlot, double> DownlinkFreq = new Exposer<TransponderSlot, double>((obj) => obj.DownlinkFreq.Value, "DownlinkFreq");
    }
}
