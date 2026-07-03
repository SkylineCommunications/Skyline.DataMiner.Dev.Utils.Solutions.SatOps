namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement;
    using DomModel = DOM.Model;

    /// <summary>
    /// Represents a transponder in the SatOps API.
    /// </summary>
    public class Transponder : ApiNamedObject
    {
        private readonly DomModel.TranspondersInstance originalInstance;
        private readonly DomModel.TranspondersInstance updatedInstance;

        private Transponder(DomModel.TranspondersInstance original, DomModel.TranspondersInstance updated)
            : base(original.ID.Id)
        {
            originalInstance = original;
            updatedInstance = updated;
        }

        /// <summary>
        /// Creates a <see cref="Transponder"/> from an existing <see cref="DomModel.TranspondersInstance"/>.
        /// </summary>
        internal static Transponder FromInstance(DomModel.TranspondersInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new Transponder(instance, instance.Clone());
        }

        /// <summary>
        /// Creates a new <see cref="Transponder"/> backed by a new <see cref="DomModel.TranspondersInstance"/>.
        /// </summary>
        internal static Transponder CreateNewTransponder()
        {
            var instance = new DomModel.TranspondersInstance();
            return new Transponder(instance, instance.Clone());
        }

        /// <summary>
        /// Gets the current lifecycle status of the transponder.
        /// </summary>
        public InstanceStatus Status
        {
            get
            {
                switch (originalInstance.Status)
                {
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.TranspondersBehavior.StatusesEnum.Active:
                        return InstanceStatus.Active;
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.TranspondersBehavior.StatusesEnum.Deprecated:
                        return InstanceStatus.Deprecated;
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.TranspondersBehavior.StatusesEnum.Error:
                        return InstanceStatus.Error;
                    default:
                        return InstanceStatus.Draft;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the transponder.
        /// </summary>
        public override string Name
        {
            get => originalInstance.Transponder?.TransponderName;
            set => updatedInstance.Transponder.TransponderName = value;
        }

        /// <summary>
        /// Gets or sets the parent satellite identifier.
        /// </summary>
        public Guid? TransponderSatellite
        {
            get => originalInstance.Transponder?.TransponderSatellite;
            set => updatedInstance.Transponder.TransponderSatellite = value;
        }

        /// <summary>
        /// Gets or sets the linked beam identifier.
        /// </summary>
        public Guid? Beam
        {
            get => originalInstance.Transponder?.Beam;
            set => updatedInstance.Transponder.Beam = value;
        }

        /// <summary>
        /// Gets or sets the transponder band.
        /// </summary>
        public TransponderBandType? Band
        {
            get
            {
                var domBand = originalInstance.Transponder?.Band;
                if (domBand == null)
                    return null;

                switch (domBand.Value)
                {
                    case DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.C: return TransponderBandType.C;
                    case DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.K: return TransponderBandType.K;
                    case DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.Ka: return TransponderBandType.Ka;
                    case DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.Ku: return TransponderBandType.Ku;
                    case DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.L: return TransponderBandType.L;
                    case DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.X: return TransponderBandType.X;
                    default: return null;
                }
            }
            set
            {
                if (value == null)
                {
                    updatedInstance.Transponder.Band = null;
                    return;
                }

                switch (value.Value)
                {
                    case TransponderBandType.C: updatedInstance.Transponder.Band = DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.C; break;
                    case TransponderBandType.K: updatedInstance.Transponder.Band = DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.K; break;
                    case TransponderBandType.Ka: updatedInstance.Transponder.Band = DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.Ka; break;
                    case TransponderBandType.Ku: updatedInstance.Transponder.Band = DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.Ku; break;
                    case TransponderBandType.L: updatedInstance.Transponder.Band = DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.L; break;
                    case TransponderBandType.X: updatedInstance.Transponder.Band = DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.X; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the bandwidth.
        /// </summary>
        public double? Bandwidth
        {
            get => originalInstance.Transponder?.Bandwidth;
            set => updatedInstance.Transponder.Bandwidth = value;
        }

        /// <summary>
        /// Gets or sets the start frequency.
        /// </summary>
        public double? StartFrequency
        {
            get => originalInstance.Transponder?.StartFrequency;
            set => updatedInstance.Transponder.StartFrequency = value;
        }

        /// <summary>
        /// Gets or sets the stop frequency.
        /// </summary>
        public double? StopFrequency
        {
            get => originalInstance.Transponder?.StopFrequency;
            set => updatedInstance.Transponder.StopFrequency = value;
        }

        /// <summary>
        /// Gets or sets the polarization.
        /// </summary>
        public TransponderPolarizationType? Polarization
        {
            get
            {
                var domPolarization = originalInstance.Transponder?.Polarization;
                if (domPolarization == null)
                    return null;

                switch (domPolarization.Value)
                {
                    case DomModel.SlcSatellite_ManagementIds.Enums.PolarizationEnum.Circular: return TransponderPolarizationType.Circular;
                    case DomModel.SlcSatellite_ManagementIds.Enums.PolarizationEnum.Linear: return TransponderPolarizationType.Linear;
                    default: return null;
                }
            }
            set
            {
                if (value == null)
                {
                    updatedInstance.Transponder.Polarization = null;
                    return;
                }

                switch (value.Value)
                {
                    case TransponderPolarizationType.Circular: updatedInstance.Transponder.Polarization = DomModel.SlcSatellite_ManagementIds.Enums.PolarizationEnum.Circular; break;
                    case TransponderPolarizationType.Linear: updatedInstance.Transponder.Polarization = DomModel.SlcSatellite_ManagementIds.Enums.PolarizationEnum.Linear; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the downlink start frequency.
        /// </summary>
        public double? DownlinkStartFreq
        {
            get => originalInstance.Transponder?.DownlinkStartFreq;
            set => updatedInstance.Transponder.DownlinkStartFreq = value;
        }

        /// <summary>
        /// Gets or sets the downlink end frequency.
        /// </summary>
        public double? DownlinkEndFreq
        {
            get => originalInstance.Transponder?.DownlinkEndFreq;
            set => updatedInstance.Transponder.DownlinkEndFreq = value;
        }

        /// <summary>
        /// Gets or sets the rolling window.
        /// </summary>
        public double? RollingWindow
        {
            get => originalInstance.Transponder?.RollingWindow;
            set => updatedInstance.Transponder.RollingWindow = value;
        }

        /// <summary>
        /// Gets or sets the hard end date.
        /// </summary>
        public DateTime? HardEndDate
        {
            get => originalInstance.Transponder?.HardEndDate;
            set => updatedInstance.Transponder.HardEndDate = value;
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber
        {
            get => originalInstance.Transponder?.PhoneNumber;
            set => updatedInstance.Transponder.PhoneNumber = value;
        }

        /// <summary>
        /// Gets or sets the uplink polarization.
        /// </summary>
        public TransponderUplinkPolarizationType? UplinkPolarization
        {
            get
            {
                var domUplinkPolarization = originalInstance.Transponder?.UplinkPolarization;
                if (domUplinkPolarization == null)
                    return null;

                switch (domUplinkPolarization.Value)
                {
                    case DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.Horizontal: return TransponderUplinkPolarizationType.Horizontal;
                    case DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.Vertical: return TransponderUplinkPolarizationType.Vertical;
                    case DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.RHCP: return TransponderUplinkPolarizationType.RHCP;
                    case DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.LHCP: return TransponderUplinkPolarizationType.LHCP;
                    default: return null;
                }
            }
            set
            {
                if (value == null)
                {
                    updatedInstance.Transponder.UplinkPolarization = null;
                    return;
                }

                switch (value.Value)
                {
                    case TransponderUplinkPolarizationType.Horizontal: updatedInstance.Transponder.UplinkPolarization = DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.Horizontal; break;
                    case TransponderUplinkPolarizationType.Vertical: updatedInstance.Transponder.UplinkPolarization = DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.Vertical; break;
                    case TransponderUplinkPolarizationType.RHCP: updatedInstance.Transponder.UplinkPolarization = DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.RHCP; break;
                    case TransponderUplinkPolarizationType.LHCP: updatedInstance.Transponder.UplinkPolarization = DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.LHCP; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the downlink polarization.
        /// </summary>
        public TransponderDownlinkPolarizationType? DownlinkPolarization
        {
            get
            {
                var domDownlinkPolarization = originalInstance.Transponder?.DownlinkPolarization;
                if (domDownlinkPolarization == null)
                    return null;

                switch (domDownlinkPolarization.Value)
                {
                    case DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.Horizontal: return TransponderDownlinkPolarizationType.Horizontal;
                    case DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.Vertical: return TransponderDownlinkPolarizationType.Vertical;
                    case DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.LHCP: return TransponderDownlinkPolarizationType.LHCP;
                    case DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.RHCP: return TransponderDownlinkPolarizationType.RHCP;
                    default: return null;
                }
            }
            set
            {
                if (value == null)
                {
                    updatedInstance.Transponder.DownlinkPolarization = null;
                    return;
                }

                switch (value.Value)
                {
                    case TransponderDownlinkPolarizationType.Horizontal: updatedInstance.Transponder.DownlinkPolarization = DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.Horizontal; break;
                    case TransponderDownlinkPolarizationType.Vertical: updatedInstance.Transponder.DownlinkPolarization = DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.Vertical; break;
                    case TransponderDownlinkPolarizationType.LHCP: updatedInstance.Transponder.DownlinkPolarization = DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.LHCP; break;
                    case TransponderDownlinkPolarizationType.RHCP: updatedInstance.Transponder.DownlinkPolarization = DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.RHCP; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the linked DOM resource identifier.
        /// </summary>
        public Guid? DOMResource
        {
            get => originalInstance.Transponder?.DOMResource;
            set => updatedInstance.Transponder.DOMResource = value;
        }

        /// <summary>
        /// Returns the updated instance used by the repository to persist changes.
        /// </summary>
        internal DomModel.TranspondersInstance ToUpdatedInstance() => updatedInstance;

        /// <summary>
        /// Returns the original instance used by the repository for reference comparison.
        /// </summary>
        internal DomModel.TranspondersInstance ToOriginalInstance() => originalInstance;

        internal static IEnumerable<Transponder> InstantiateTransponders(IEnumerable<DomModel.TranspondersInstance> instances)
        {
            if (instances == null)
                throw new ArgumentNullException(nameof(instances));
            if (!instances.Any())
                return Enumerable.Empty<Transponder>();

            return InstantiateTranspondersIterator(instances);
        }

        private static IEnumerable<Transponder> InstantiateTranspondersIterator(IEnumerable<DomModel.TranspondersInstance> instances)
        {
            foreach (var instance in instances)
            {
                yield return new Transponder(instance, instance.Clone());
            }
        }
    }
}
