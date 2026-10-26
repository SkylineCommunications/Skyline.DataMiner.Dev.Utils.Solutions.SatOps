namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Satellite
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using System;

    /// <summary>
    /// Provides exposers for querying <see cref="Satellite"/> objects.
    /// </summary>
    /// <remarks>
    /// Exposers define how properties of <see cref="Satellite"/> objects are exposed for use in queries.
    /// </remarks>
    public static class SatelliteExposers
    {
        /// <summary>
        /// Exposes the <see cref="ApiObject.Id"/> property for querying as "Guid".
        /// </summary>
        public static readonly Exposer<Satellite, Guid> Guid = new Exposer<Satellite, Guid>((obj) => obj.Id, "Guid");

        /// <summary>
        /// Exposes the <see cref="Satellite.Name"/> property for querying as "Name".
        /// </summary>
        public static readonly Exposer<Satellite, string> Name = new Exposer<Satellite, string>((obj) => obj.Name, "Name");

        /// <summary>
        /// Exposes the <see cref="Satellite.Abbreviation"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> Abbreviation = new Exposer<Satellite, string>((obj) => obj.Abbreviation, "Abbreviation");

        /// <summary>
        /// Exposes the <see cref="Satellite.Orbit"/> property for querying as "Orbit".
        /// </summary>
        public static readonly Exposer<Satellite, OrbitType> Orbit = new Exposer<Satellite, OrbitType>((obj) => obj.Orbit.Value, "Orbit");

        /// <summary>
        /// Exposes the <see cref="Satellite.LongitudeForGEODegrees"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, double> LongitudeForGeoDegrees = new Exposer<Satellite, double>((obj) => obj.LongitudeForGEODegrees.Value, "LongitudeForGEODegrees");

        /// <summary>
        /// Exposes the <see cref="Satellite.InclinationDegrees"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, double> InclinationDegrees = new Exposer<Satellite, double>((obj) => obj.InclinationDegrees.Value, "InclinationDegrees");

        /// <summary>
        /// Exposes the <see cref="Satellite.Hemisphere"/> property for querying as "Hemisphere".
        /// </summary>
        public static readonly Exposer<Satellite, HemisphereType> Hemisphere = new Exposer<Satellite, HemisphereType>((obj) => obj.Hemisphere.Value, "Hemisphere");

        /// <summary>
        /// Exposes the <see cref="Satellite.Operator"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> Operator = new Exposer<Satellite, string>((obj) => obj.Operator, "Operator");

        /// <summary>
        /// Exposes the <see cref="Satellite.Coverage"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> Coverage = new Exposer<Satellite, string>((obj) => obj.Coverage, "Coverage");

        /// <summary>
        /// Exposes the <see cref="Satellite.Applications"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> Applications = new Exposer<Satellite, string>((obj) => obj.Applications, "Applications");

        /// <summary>
        /// Exposes the <see cref="Satellite.Info"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> Info = new Exposer<Satellite, string>((obj) => obj.Info, "Info");

        /// <summary>
        /// Exposes the <see cref="Satellite.Manufacturer"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> Manufacturer = new Exposer<Satellite, string>((obj) => obj.Manufacturer, "Manufacturer");

        /// <summary>
        /// Exposes the <see cref="Satellite.Country"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> Country = new Exposer<Satellite, string>((obj) => obj.Country, "Country");

        /// <summary>
        /// Exposes the <see cref="Satellite.LaunchInfo"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> LaunchInfo = new Exposer<Satellite, string>((obj) => obj.LaunchInfo, "LaunchInfo");

        /// <summary>
        /// Exposes the <see cref="Satellite.LaunchInServiceDate"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, DateTime> LaunchInServiceDate = new Exposer<Satellite, DateTime>((obj) => obj.LaunchInServiceDate.Value, "LaunchInServiceDate");
    }
}
