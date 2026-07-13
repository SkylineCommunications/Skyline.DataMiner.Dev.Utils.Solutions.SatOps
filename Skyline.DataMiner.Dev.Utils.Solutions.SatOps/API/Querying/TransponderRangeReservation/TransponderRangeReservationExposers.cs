namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying.TransponderRangeReservation
{
    using System;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;

    /// <summary>
    /// Provides exposers for querying <see cref="TransponderRangeReservation"/> objects.
    /// </summary>
    public static class TransponderRangeReservationExposers
    {
        /// <summary>
        /// Exposes the <see cref="TransponderRangeReservation.Name"/> property for querying as "ReservationName".
        /// </summary>
        public static readonly Exposer<TransponderRangeReservation, string> ReservationName = new Exposer<TransponderRangeReservation, string>((obj) => obj.Name, "ReservationName");

        /// <summary>
        /// Exposes the <see cref="TransponderRangeReservation.Transponder"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderRangeReservation, Guid> Transponder = new Exposer<TransponderRangeReservation, Guid>((obj) => obj.Transponder.Value, "Transponder");

        /// <summary>
        /// Exposes the <see cref="TransponderRangeReservation.RelativeStartFrequency"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderRangeReservation, double> RelativeStartFrequency = new Exposer<TransponderRangeReservation, double>((obj) => obj.RelativeStartFrequency.Value, "RelativeStartFrequency");

        /// <summary>
        /// Exposes the <see cref="TransponderRangeReservation.RelativeEndFrequency"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderRangeReservation, double> RelativeEndFrequency = new Exposer<TransponderRangeReservation, double>((obj) => obj.RelativeEndFrequency.Value, "RelativeEndFrequency");

        /// <summary>
        /// Exposes the <see cref="TransponderRangeReservation.StartTime"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderRangeReservation, DateTime> StartTime = new Exposer<TransponderRangeReservation, DateTime>((obj) => obj.StartTime.Value, "StartTime");

        /// <summary>
        /// Exposes the <see cref="TransponderRangeReservation.EndTime"/> property for querying.
        /// </summary>
        public static readonly Exposer<TransponderRangeReservation, DateTime> EndTime = new Exposer<TransponderRangeReservation, DateTime>((obj) => obj.EndTime.Value, "EndTime");
    }
}
