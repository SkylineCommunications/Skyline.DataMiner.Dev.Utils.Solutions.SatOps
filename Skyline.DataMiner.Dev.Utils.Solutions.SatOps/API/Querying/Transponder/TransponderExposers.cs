namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using System;

    /// <summary>
    /// Provides exposers for querying <see cref="Transponder"/> objects.
    /// </summary>
    /// <remarks>
    /// Exposers define how properties of <see cref="Transponder"/> objects are exposed for use in queries.
    /// </remarks>
    public static class TransponderExposers
    {
        /// <summary>
        /// Exposes the <see cref="ApiObject.Id"/> property for querying as "Guid".
        /// </summary>
        public static readonly Exposer<Transponder, Guid> Guid = new Exposer<Transponder, Guid>((obj) => obj.Id, "Guid");

        /// <summary>
        /// Exposes the <see cref="Transponder.Name"/> property for querying as "Name".
        /// </summary>
        public static readonly Exposer<Transponder, string> Name = new Exposer<Transponder, string>((obj) => obj.Name, "Name");

        /// <summary>
        /// Exposes the <see cref="Transponder.TransponderSatellite"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, Guid> Satellite = new Exposer<Transponder, Guid>((obj) => obj.TransponderSatellite.Value, "Satellite");

        /// <summary>
        /// Exposes the <see cref="Transponder.Beam"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, Guid> Beam = new Exposer<Transponder, Guid>((obj) => obj.Beam.Value, "Beam");

        /// <summary>
        /// Exposes the <see cref="Transponder.Band"/> property for querying as "Band".
        /// </summary>
        public static readonly Exposer<Transponder, TransponderBandType> Band = new Exposer<Transponder, TransponderBandType>((obj) => obj.Band.Value, "Band");

        /// <summary>
        /// Exposes the <see cref="Transponder.Bandwidth"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> Bandwidth = new Exposer<Transponder, double>((obj) => obj.Bandwidth.Value, "Bandwidth");

        /// <summary>
        /// Exposes the <see cref="Transponder.StartFrequency"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> StartFrequency = new Exposer<Transponder, double>((obj) => obj.StartFrequency.Value, "StartFrequency");

        /// <summary>
        /// Exposes the <see cref="Transponder.StopFrequency"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> StopFrequency = new Exposer<Transponder, double>((obj) => obj.StopFrequency.Value, "StopFrequency");

        /// <summary>
        /// Exposes the <see cref="Transponder.Polarization"/> property for querying as "Polarization".
        /// </summary>
        public static readonly Exposer<Transponder, TransponderPolarizationType> Polarization = new Exposer<Transponder, TransponderPolarizationType>((obj) => obj.Polarization.Value, "Polarization");

        /// <summary>
        /// Exposes the <see cref="Transponder.DownlinkStartFreq"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> DownlinkStartFreq = new Exposer<Transponder, double>((obj) => obj.DownlinkStartFreq.Value, "DownlinkStartFreq");

        /// <summary>
        /// Exposes the <see cref="Transponder.DownlinkEndFreq"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> DownlinkEndFreq = new Exposer<Transponder, double>((obj) => obj.DownlinkEndFreq.Value, "DownlinkEndFreq");

        /// <summary>
        /// Exposes the <see cref="Transponder.RollingWindow"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> RollingWindow = new Exposer<Transponder, double>((obj) => obj.RollingWindow.Value, "RollingWindow");

        /// <summary>
        /// Exposes the <see cref="Transponder.HardEndDate"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, DateTime> HardEndDate = new Exposer<Transponder, DateTime>((obj) => obj.HardEndDate.Value, "HardEndDate");

        /// <summary>
        /// Exposes the <see cref="Transponder.PhoneNumber"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, string> PhoneNumber = new Exposer<Transponder, string>((obj) => obj.PhoneNumber, "PhoneNumber");

        /// <summary>
        /// Exposes the <see cref="Transponder.UplinkPolarization"/> property for querying as "UplinkPolarization".
        /// </summary>
        public static readonly Exposer<Transponder, TransponderUplinkPolarizationType> UplinkPolarization = new Exposer<Transponder, TransponderUplinkPolarizationType>((obj) => obj.UplinkPolarization.Value, "UplinkPolarization");

        /// <summary>
        /// Exposes the <see cref="Transponder.DownlinkPolarization"/> property for querying as "DownlinkPolarization".
        /// </summary>
        public static readonly Exposer<Transponder, TransponderDownlinkPolarizationType> DownlinkPolarization = new Exposer<Transponder, TransponderDownlinkPolarizationType>((obj) => obj.DownlinkPolarization.Value, "DownlinkPolarization");

        /// <summary>
        /// Exposes the <see cref="Transponder.DOMResource"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, Guid> DOMResource = new Exposer<Transponder, Guid>((obj) => obj.DOMResource.Value, "DOMResource");
    }
}
