namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using DomModel = DOM.Model;

    /// <summary>
    /// Represents a beam in the SatOps API.
    /// </summary>
    public class Beam : ApiNamedObject
    {
        private readonly DomModel.BeamsInstance originalInstance;
        private readonly DomModel.BeamsInstance updatedInstance;

        private Beam(DomModel.BeamsInstance original, DomModel.BeamsInstance updated)
            : base(original.ID.Id)
        {
            originalInstance = original;
            updatedInstance = updated;
        }

        /// <summary>
        /// Creates a <see cref="Beam"/> from an existing <see cref="DomModel.BeamsInstance"/>.
        /// </summary>
        internal static Beam FromInstance(DomModel.BeamsInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new Beam(instance, instance.Clone());
        }

        /// <summary>
        /// Creates a new <see cref="Beam"/> backed by a new <see cref="DomModel.BeamsInstance"/>.
        /// </summary>
        internal static Beam CreateNewBeam()
        {
            var instance = new DomModel.BeamsInstance();
            return new Beam(instance, instance.Clone());
        }

        /// <summary>
        /// Gets the current lifecycle status of the beam.
        /// </summary>
        public InstanceStatus Status
        {
            get
            {
                switch (originalInstance.Status)
                {
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.StatusesEnum.Active:
                        return InstanceStatus.Active;
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.StatusesEnum.Deprecated:
                        return InstanceStatus.Deprecated;
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.StatusesEnum.Error:
                        return InstanceStatus.Error;
                    default:
                        return InstanceStatus.Draft;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the beam.
        /// </summary>
        public override string Name
        {
            get => updatedInstance.Beam?.BeamName;
            set => updatedInstance.Beam.BeamName = value;
        }

        /// <summary>
        /// Gets or sets the related satellite ID for this beam.
        /// </summary>
        public Guid? BeamSatellite
        {
            get => updatedInstance.Beam?.BeamSatellite;
            set => updatedInstance.Beam.BeamSatellite = value;
        }

        /// <summary>
        /// Gets or sets the link type of the beam.
        /// </summary>
        public BeamLinkType? LinkType
        {
            get
            {
                var domLinkType = updatedInstance.Beam?.LinkType;
                if (domLinkType == null)
                    return null;

                switch (domLinkType.Value)
                {
                    case DomModel.SlcSatellite_ManagementIds.Enums.LinktypeEnum.Feeder: return BeamLinkType.Feeder;
                    case DomModel.SlcSatellite_ManagementIds.Enums.LinktypeEnum.User: return BeamLinkType.User;
                    case DomModel.SlcSatellite_ManagementIds.Enums.LinktypeEnum.Uplink: return BeamLinkType.Uplink;
                    case DomModel.SlcSatellite_ManagementIds.Enums.LinktypeEnum.Downlink: return BeamLinkType.Downlink;
                    default: return null;
                }
            }
            set
            {
                if (value == null)
                {
                    updatedInstance.Beam.LinkType = null;
                    return;
                }

                switch (value.Value)
                {
                    case BeamLinkType.Feeder: updatedInstance.Beam.LinkType = DomModel.SlcSatellite_ManagementIds.Enums.LinktypeEnum.Feeder; break;
                    case BeamLinkType.User: updatedInstance.Beam.LinkType = DomModel.SlcSatellite_ManagementIds.Enums.LinktypeEnum.User; break;
                    case BeamLinkType.Uplink: updatedInstance.Beam.LinkType = DomModel.SlcSatellite_ManagementIds.Enums.LinktypeEnum.Uplink; break;
                    case BeamLinkType.Downlink: updatedInstance.Beam.LinkType = DomModel.SlcSatellite_ManagementIds.Enums.LinktypeEnum.Downlink; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the transmission type of the beam.
        /// </summary>
        public BeamTransmissionType? TransmissionType
        {
            get
            {
                var domTransmissionType = updatedInstance.Beam?.TransmissionType;
                if (domTransmissionType == null)
                    return null;

                switch (domTransmissionType.Value)
                {
                    case DomModel.SlcSatellite_ManagementIds.Enums.TransmissiontypeEnum.RX: return BeamTransmissionType.RX;
                    case DomModel.SlcSatellite_ManagementIds.Enums.TransmissiontypeEnum.TX: return BeamTransmissionType.TX;
                    case DomModel.SlcSatellite_ManagementIds.Enums.TransmissiontypeEnum.CarrierInCarrier: return BeamTransmissionType.CarrierInCarrier;
                    default: return null;
                }
            }
            set
            {
                if (value == null)
                {
                    updatedInstance.Beam.TransmissionType = null;
                    return;
                }

                switch (value.Value)
                {
                    case BeamTransmissionType.RX: updatedInstance.Beam.TransmissionType = DomModel.SlcSatellite_ManagementIds.Enums.TransmissiontypeEnum.RX; break;
                    case BeamTransmissionType.TX: updatedInstance.Beam.TransmissionType = DomModel.SlcSatellite_ManagementIds.Enums.TransmissiontypeEnum.TX; break;
                    case BeamTransmissionType.CarrierInCarrier: updatedInstance.Beam.TransmissionType = DomModel.SlcSatellite_ManagementIds.Enums.TransmissiontypeEnum.CarrierInCarrier; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the footprint file path for the beam.
        /// </summary>
        public string FootprintFile
        {
            get => updatedInstance.Beam?.FootprintFile;
            set => updatedInstance.Beam.FootprintFile = value;
        }

        /// <summary>
        /// Returns the updated instance used by the repository to persist changes.
        /// </summary>
        internal DomModel.BeamsInstance ToUpdatedInstance() => updatedInstance;

        /// <summary>
        /// Returns the original instance used by the repository for reference comparison.
        /// </summary>
        internal DomModel.BeamsInstance ToOriginalInstance() => originalInstance;

        internal static IEnumerable<Beam> InstantiateBeams(IEnumerable<BeamsInstance> instances)
        {
            if(instances == null)
                throw new ArgumentNullException(nameof(instances));
            if(!instances.Any())
                return Enumerable.Empty<Beam>();

           return InstantiateBeamsIterator(instances);
        }

        private static IEnumerable<Beam> InstantiateBeamsIterator(IEnumerable<BeamsInstance> instances)
        {
            foreach (var instance in instances)
            {
                yield return new Beam(instance, instance.Clone());
            }
        }
    }
}
