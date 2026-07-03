namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying.Satellite
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
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
        /// Exposes the <see cref="Satellite.Name"/> property for querying as "SatelliteName".
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteName = new Exposer<Satellite, string>((obj) => obj.Name, "SatelliteName");

        /// <summary>
        /// Exposes the <see cref="Satellite.Abbreviation"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteAbbreviation = new Exposer<Satellite, string>((obj) => obj.Abbreviation, "SatelliteAbbreviation");

        /// <summary>
        /// Exposes the <see cref="Satellite.Orbit"/> property for querying as "Orbit".
        /// </summary>
        public static readonly Exposer<Satellite, OrbitType> SatelliteOrbit = new Exposer<Satellite, OrbitType>((obj) => obj.Orbit.Value, "Orbit");

        /// <summary>
        /// Exposes the <see cref="Satellite.LongitudeForGEODegrees"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, double> SatelliteLongitudeForGEODegrees = new Exposer<Satellite, double>((obj) => obj.LongitudeForGEODegrees.Value, "LongitudeForGEODegrees");

        /// <summary>
        /// Exposes the <see cref="Satellite.InclinationDegrees"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, double> SatelliteInclinationDegrees = new Exposer<Satellite, double>((obj) => obj.InclinationDegrees.Value, "InclinationDegrees");

        /// <summary>
        /// Exposes the <see cref="Satellite.Hemisphere"/> property for querying as "Hemisphere".
        /// </summary>
        public static readonly Exposer<Satellite, HemisphereType> SatelliteHemisphere = new Exposer<Satellite, HemisphereType>((obj) => obj.Hemisphere.Value, "Hemisphere");

        /// <summary>
        /// Exposes the <see cref="Satellite.Operator"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteOperator = new Exposer<Satellite, string>((obj) => obj.Operator, "Operator");

        /// <summary>
        /// Exposes the <see cref="Satellite.Coverage"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteCoverage = new Exposer<Satellite, string>((obj) => obj.Coverage, "Coverage");

        /// <summary>
        /// Exposes the <see cref="Satellite.Applications"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteApplications = new Exposer<Satellite, string>((obj) => obj.Applications, "Applications");

        /// <summary>
        /// Exposes the <see cref="Satellite.Info"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteInfo = new Exposer<Satellite, string>((obj) => obj.Info, "Info");

        /// <summary>
        /// Exposes the <see cref="Satellite.Manufacturer"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteManufacturer = new Exposer<Satellite, string>((obj) => obj.Manufacturer, "Manufacturer");

        /// <summary>
        /// Exposes the <see cref="Satellite.Country"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteCountry = new Exposer<Satellite, string>((obj) => obj.Country, "Country");

        /// <summary>
        /// Exposes the <see cref="Satellite.LaunchInfo"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, string> SatelliteLaunchInfo = new Exposer<Satellite, string>((obj) => obj.LaunchInfo, "LaunchInfo");

        /// <summary>
        /// Exposes the <see cref="Satellite.LaunchInServiceDate"/> property for querying.
        /// </summary>
        public static readonly Exposer<Satellite, DateTime> SatelliteLaunchInServiceDate = new Exposer<Satellite, DateTime>((obj) => obj.LaunchInServiceDate.Value, "LaunchInServiceDate");
    }
}
