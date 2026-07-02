namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite
{
    using System;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement;
    using DomModel = DOM.Model;

    /// <summary>
    /// Represents a satellite in the SatOps API.
    /// </summary>
    public class Satellite : ApiNamedObject
    {
        private readonly DomModel.SatellitesInstance originalInstance;
        private readonly DomModel.SatellitesInstance updatedInstance;

        private Satellite(DomModel.SatellitesInstance original, DomModel.SatellitesInstance updated)
            : base(original.ID.Id)
        {
            originalInstance = original;
            updatedInstance = updated;
        }

        /// <summary>
        /// Creates a <see cref="Satellite"/> from an existing <see cref="DomModel.SatellitesInstance"/>.
        /// </summary>
        internal static Satellite FromInstance(DomModel.SatellitesInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new Satellite(instance, instance.Clone());
        }

        /// <summary>
        /// Creates a new <see cref="Satellite"/> backed by a new <see cref="DomModel.SatellitesInstance"/>.
        /// </summary>
        internal static Satellite CreateNewSatellite()
        {
            var instance = new DomModel.SatellitesInstance();
            return new Satellite(instance, instance.Clone());
        }

        /// <summary>
        /// Gets the current lifecycle status of the satellite.
        /// </summary>
        public InstanceStatus Status
        {
            get
            {
                switch (originalInstance.Status)
                {
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.SatellitesBehavior.StatusesEnum.Active:
                        return InstanceStatus.Active;
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.SatellitesBehavior.StatusesEnum.Deprecated:
                        return InstanceStatus.Deprecated;
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.SatellitesBehavior.StatusesEnum.Error:
                        return InstanceStatus.Error;
                    default:
                        return InstanceStatus.Draft;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the satellite.
        /// </summary>
        public override string Name
        {
            get => originalInstance.General?.SatelliteName;
            set => updatedInstance.General.SatelliteName = value;
        }

        /// <summary>
        /// Gets or sets the abbreviation of the satellite.
        /// </summary>
        public string Abbreviation
        {
            get => originalInstance.General?.SatelliteAbbreviation;
            set => updatedInstance.General.SatelliteAbbreviation = value;
        }

        /// <summary>
        /// Gets or sets the orbit type of the satellite.
        /// </summary>
        public OrbitType? Orbit
        {
            get
            {
                var domOrbit = originalInstance.General?.Orbit;
                if (domOrbit == null)
                    return null;

                switch (domOrbit.Value)
                {
                    case DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.GEO: return OrbitType.GEO;
                    case DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.MEO: return OrbitType.MEO;
                    case DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.LEO: return OrbitType.LEO;
                    default: return null;
                }
            }
            set
            {
                if (value == null)
                {
                    updatedInstance.General.Orbit = null;
                    return;
                }

                switch (value.Value)
                {
                    case OrbitType.GEO: updatedInstance.General.Orbit = DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.GEO; break;
                    case OrbitType.MEO: updatedInstance.General.Orbit = DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.MEO; break;
                    case OrbitType.LEO: updatedInstance.General.Orbit = DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.LEO; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the longitude for GEO orbit in degrees.
        /// </summary>
        public double? LongitudeForGEODegrees
        {
            get => originalInstance.General?.LongitudeForGEODegrees;
            set => updatedInstance.General.LongitudeForGEODegrees = value;
        }

        /// <summary>
        /// Gets or sets the inclination in degrees.
        /// </summary>
        public double? InclinationDegrees
        {
            get => originalInstance.General?.InclinationDegrees;
            set => updatedInstance.General.InclinationDegrees = value;
        }

        /// <summary>
        /// Gets or sets the operator of the satellite.
        /// </summary>
        public string Operator
        {
            get => originalInstance.Satellite?.Operator;
            set => updatedInstance.Satellite.Operator = value;
        }

        /// <summary>
        /// Gets or sets the coverage area of the satellite.
        /// </summary>
        public string Coverage
        {
            get => originalInstance.Satellite?.Coverage;
            set => updatedInstance.Satellite.Coverage = value;
        }

        /// <summary>
        /// Gets or sets the applications supported by the satellite.
        /// </summary>
        public string Applications
        {
            get => originalInstance.Satellite?.Applications;
            set => updatedInstance.Satellite.Applications = value;
        }

        /// <summary>
        /// Gets or sets additional information about the satellite.
        /// </summary>
        public string Info
        {
            get => originalInstance.Satellite?.Info;
            set => updatedInstance.Satellite.Info = value;
        }

        /// <summary>
        /// Gets or sets the hemisphere of the satellite's orbital position.
        /// </summary>
        public HemisphereType? Hemisphere
        {
            get
            {
                var domHemisphere = originalInstance.General?.Hemisphere;
                if (domHemisphere == null)
                    return null;

                switch (domHemisphere.Value)
                {
                    case DomModel.SlcSatellite_ManagementIds.Enums.HemisphereEnum.Western: return HemisphereType.Western;
                    case DomModel.SlcSatellite_ManagementIds.Enums.HemisphereEnum.Eastern: return HemisphereType.Eastern;
                    default: return null;
                }
            }
            set
            {
                if (value == null)
                {
                    updatedInstance.General.Hemisphere = null;
                    return;
                }

                switch (value.Value)
                {
                    case HemisphereType.Western: updatedInstance.General.Hemisphere = DomModel.SlcSatellite_ManagementIds.Enums.HemisphereEnum.Western; break;
                    case HemisphereType.Eastern: updatedInstance.General.Hemisphere = DomModel.SlcSatellite_ManagementIds.Enums.HemisphereEnum.Eastern; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the manufacturer of the satellite.
        /// </summary>
        public string Manufacturer
        {
            get => originalInstance.Origin?.Manufacturer;
            set => updatedInstance.Origin.Manufacturer = value;
        }

        /// <summary>
        /// Gets or sets the country of origin of the satellite.
        /// </summary>
        public string Country
        {
            get => originalInstance.Origin?.Country;
            set => updatedInstance.Origin.Country = value;
        }

        /// <summary>
        /// Gets or sets launch information for the satellite.
        /// </summary>
        public string LaunchInfo
        {
            get => originalInstance.LaunchInformation?.LaunchInfo;
            set => updatedInstance.LaunchInformation.LaunchInfo = value;
        }

        /// <summary>
        /// Gets or sets the in-service date of the satellite launch.
        /// </summary>
        public DateTime? LaunchInServiceDate
        {
            get => originalInstance.LaunchInformation?.LaunchInServiceDate;
            set => updatedInstance.LaunchInformation.LaunchInServiceDate = value;
        }

        /// <summary>
        /// Returns the updated instance used by the repository to persist changes.
        /// </summary>
        internal DomModel.SatellitesInstance ToUpdatedInstance() => updatedInstance;

        /// <summary>
        /// Returns the original instance used by the repository for reference comparison.
        /// </summary>
        internal DomModel.SatellitesInstance ToOriginalInstance() => originalInstance;
    }
}

