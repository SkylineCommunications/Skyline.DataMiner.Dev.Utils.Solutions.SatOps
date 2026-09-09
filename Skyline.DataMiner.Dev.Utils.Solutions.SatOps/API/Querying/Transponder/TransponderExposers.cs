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
        /// Exposes the <see cref="Transponder.Id"/> property for querying as "TransponderId".
        /// </summary>
        public static readonly Exposer<Transponder, Guid> TransponderId = new Exposer<Transponder, Guid>((obj) => obj.Id, "TransponderId");

        /// <summary>
        /// Exposes the <see cref="Transponder.Name"/> property for querying as "TransponderName".
        /// </summary>
        public static readonly Exposer<Transponder, string> TransponderName = new Exposer<Transponder, string>((obj) => obj.Name, "TransponderName");

        /// <summary>
        /// Exposes the <see cref="Transponder.TransponderSatellite"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, Guid> TransponderSatellite = new Exposer<Transponder, Guid>((obj) => obj.TransponderSatellite.Value, "TransponderSatellite");

        /// <summary>
        /// Exposes the <see cref="Transponder.Beam"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, Guid> TransponderBeam = new Exposer<Transponder, Guid>((obj) => obj.Beam.Value, "Beam");

        /// <summary>
        /// Exposes the <see cref="Transponder.Band"/> property for querying as "Band".
        /// </summary>
        public static readonly Exposer<Transponder, TransponderBandType> TransponderBand = new Exposer<Transponder, TransponderBandType>((obj) => obj.Band.Value, "Band");

        /// <summary>
        /// Exposes the <see cref="Transponder.Bandwidth"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> TransponderBandwidth = new Exposer<Transponder, double>((obj) => obj.Bandwidth.Value, "Bandwidth");

        /// <summary>
        /// Exposes the <see cref="Transponder.StartFrequency"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> TransponderStartFrequency = new Exposer<Transponder, double>((obj) => obj.StartFrequency.Value, "StartFrequency");

        /// <summary>
        /// Exposes the <see cref="Transponder.StopFrequency"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> TransponderStopFrequency = new Exposer<Transponder, double>((obj) => obj.StopFrequency.Value, "StopFrequency");

        /// <summary>
        /// Exposes the <see cref="Transponder.Polarization"/> property for querying as "Polarization".
        /// </summary>
        public static readonly Exposer<Transponder, TransponderPolarizationType> TransponderPolarization = new Exposer<Transponder, TransponderPolarizationType>((obj) => obj.Polarization.Value, "Polarization");

        /// <summary>
        /// Exposes the <see cref="Transponder.DownlinkStartFreq"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> TransponderDownlinkStartFreq = new Exposer<Transponder, double>((obj) => obj.DownlinkStartFreq.Value, "DownlinkStartFreq");

        /// <summary>
        /// Exposes the <see cref="Transponder.DownlinkEndFreq"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> TransponderDownlinkEndFreq = new Exposer<Transponder, double>((obj) => obj.DownlinkEndFreq.Value, "DownlinkEndFreq");

        /// <summary>
        /// Exposes the <see cref="Transponder.RollingWindow"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, double> TransponderRollingWindow = new Exposer<Transponder, double>((obj) => obj.RollingWindow.Value, "RollingWindow");

        /// <summary>
        /// Exposes the <see cref="Transponder.HardEndDate"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, DateTime> TransponderHardEndDate = new Exposer<Transponder, DateTime>((obj) => obj.HardEndDate.Value, "HardEndDate");

        /// <summary>
        /// Exposes the <see cref="Transponder.PhoneNumber"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, string> TransponderPhoneNumber = new Exposer<Transponder, string>((obj) => obj.PhoneNumber, "PhoneNumber");

        /// <summary>
        /// Exposes the <see cref="Transponder.UplinkPolarization"/> property for querying as "UplinkPolarization".
        /// </summary>
        public static readonly Exposer<Transponder, TransponderUplinkPolarizationType> TransponderUplinkPolarization = new Exposer<Transponder, TransponderUplinkPolarizationType>((obj) => obj.UplinkPolarization.Value, "UplinkPolarization");

        /// <summary>
        /// Exposes the <see cref="Transponder.DownlinkPolarization"/> property for querying as "DownlinkPolarization".
        /// </summary>
        public static readonly Exposer<Transponder, TransponderDownlinkPolarizationType> TransponderDownlinkPolarization = new Exposer<Transponder, TransponderDownlinkPolarizationType>((obj) => obj.DownlinkPolarization.Value, "DownlinkPolarization");

        /// <summary>
        /// Exposes the <see cref="Transponder.DOMResource"/> property for querying.
        /// </summary>
        public static readonly Exposer<Transponder, Guid> TransponderDOMResource = new Exposer<Transponder, Guid>((obj) => obj.DOMResource.Value, "DOMResource");
    }
}
