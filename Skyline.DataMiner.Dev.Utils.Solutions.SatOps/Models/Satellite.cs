namespace Skyline.DataMiner.SDM.SatOps.Common.Models
{
    using System;

    /// <summary>
    /// Represents a satellite entity with general information, satellite details, origin, and launch data.
    /// </summary>
    // [GenerateExposers]
    // [SdmDomStorage("(slc)satellite_management")]
    public class Satellite : SdmObject<Satellite>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the satellite.
        /// </summary>
        public Guid SatelliteId { get; set; }

        /// <summary>
        /// Gets or sets the general information about the satellite.
        /// </summary>
        public GeneralInfo General { get; set; } = new GeneralInfo();

        /// <summary>
        /// Gets or sets the satellite-specific information including operator, coverage, and application details.
        /// </summary>
        public SatelliteInfo SatelliteInformation { get; set; } = new SatelliteInfo();

        /// <summary>
        /// Gets or sets the origin information including country and manufacturer.
        /// </summary>
        public OriginInfo Origin { get; set; } = new OriginInfo();

        /// <summary>
        /// Gets or sets the launch information including launch date and related details.
        /// </summary>
        public LaunchInfo Launch { get; set; } = new LaunchInfo();

        /// <summary>
        /// Contains general information about the satellite including name, orbit, and positioning details.
        /// </summary>
        public sealed class GeneralInfo : IEquatable<GeneralInfo>
        {
            /// <summary>
            /// Gets or sets the name of the satellite.
            /// </summary>
            public string SatelliteName { get; set; }

            /// <summary>
            /// Gets or sets the abbreviated name of the satellite.
            /// </summary>
            public string SatelliteAbbreviation { get; set; }

            /// <summary>
            /// Gets or sets the orbit type of the satellite.
            /// </summary>
            public SharedMappers.Satellite_Management.SlcSatellite_ManagementIds.Enums.OrbitEnum Orbit { get; set; }

            /// <summary>
            /// Gets or sets the hemisphere in which the satellite operates.
            /// </summary>
            public SharedMappers.Satellite_Management.SlcSatellite_ManagementIds.Enums.HemisphereEnum Hemisphere { get; set; }

            /// <summary>
            /// Gets or sets the longitude position for geostationary satellites.
            /// </summary>
            public double LongitudeForGeo { get; set; }

            /// <summary>
            /// Gets or sets the inclination angle of the satellite's orbit in degrees.
            /// </summary>
            public double Inclination { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="GeneralInfo"/> is equal to the current instance.
            /// </summary>
            /// <param name="other">The <see cref="GeneralInfo"/> to compare with the current instance.</param>
            /// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
            public bool Equals(GeneralInfo other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return
                    SatelliteName == other.SatelliteName &&
                    SatelliteAbbreviation == other.SatelliteAbbreviation &&
                    Orbit == other.Orbit && Hemisphere == other.Hemisphere &&
                    LongitudeForGeo.Equals(other.LongitudeForGeo) &&
                    Inclination.Equals(other.Inclination);
            }
        }

        /// <summary>
        /// Contains detailed information about satellite operations and coverage.
        /// </summary>
        public sealed class SatelliteInfo : IEquatable<SatelliteInfo>
        {
            /// <summary>
            /// Gets or sets the satellite operator or owner.
            /// </summary>
            public string Operator { get; set; }

            /// <summary>
            /// Gets or sets the coverage area of the satellite.
            /// </summary>
            public string Coverage { get; set; }

            /// <summary>
            /// Gets or sets the primary application or purpose of the satellite.
            /// </summary>
            public string Application { get; set; }

            /// <summary>
            /// Gets or sets additional information about the satellite.
            /// </summary>
            public string Information { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="SatelliteInfo"/> is equal to the current instance.
            /// </summary>
            /// <param name="other">The <see cref="SatelliteInfo"/> to compare with the current instance.</param>
            /// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
            public bool Equals(SatelliteInfo other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return
                    Operator == other.Operator &&
                    Coverage == other.Coverage &&
                    Application == other.Application &&
                    Information == other.Information;
            }
        }

        /// <summary>
        /// Contains origin information about the satellite's manufacturing and country of origin.
        /// </summary>
        public sealed class OriginInfo : IEquatable<OriginInfo>
        {
            /// <summary>
            /// Gets or sets the country of origin for the satellite.
            /// </summary>
            public string Country { get; set; }

            /// <summary>
            /// Gets or sets the manufacturer of the satellite.
            /// </summary>
            public string Manufacturer { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="OriginInfo"/> is equal to the current instance.
            /// </summary>
            /// <param name="other">The <see cref="OriginInfo"/> to compare with the current instance.</param>
            /// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
            public bool Equals(OriginInfo other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return
                    Country == other.Country &&
                    Manufacturer == other.Manufacturer;
            }
        }

        /// <summary>
        /// Contains information about the satellite's launch and in-service date.
        /// </summary>
        public sealed class LaunchInfo : IEquatable<LaunchInfo>
        {
            /// <summary>
            /// Gets or sets the launch information description.
            /// </summary>
            public string LaunchInformation { get; set; }

            /// <summary>
            /// Gets or sets the date when the satellite was launched and put into service.
            /// </summary>
            public DateTime? LaunchInServiceDate { get; set; }

            /// <summary>
            /// Determines whether the specified <see cref="LaunchInfo"/> is equal to the current instance.
            /// </summary>
            /// <param name="other">The <see cref="LaunchInfo"/> to compare with the current instance.</param>
            /// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
            public bool Equals(LaunchInfo other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return
                    LaunchInformation == other.LaunchInformation &&
                    Nullable.Equals(LaunchInServiceDate, other.LaunchInServiceDate);
            }
        }
    }
}