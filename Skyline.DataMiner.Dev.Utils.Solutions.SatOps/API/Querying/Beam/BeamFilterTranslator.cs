namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Beam
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using System;
    using System.Collections.Generic;

    internal class BeamFilterTranslator : DomInstanceFilterTranslator<Beam>
    {
        private readonly FilterElement<DomInstance> beamDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal
            (SlcSatellite_ManagementIds.Definitions.Beams.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [BeamExposers.Guid.fieldName] = HandleGuid,
            [BeamExposers.Status.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.StatusId, comparer,
                InstanceStatusFilterHelper.ToStatusId((InstanceStatus)value,
                    SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.Statuses.Draft,
                    SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.Statuses.Active,
                    SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.Statuses.Deprecated,
                    SlcSatellite_ManagementIds.Behaviors.BeamsBehavior.Statuses.Error)),
            [BeamExposers.Name.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.BeamName), comparer, (string)value),
            [BeamExposers.Satellite.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.BeamSatellite), comparer, (Guid)value),
            [BeamExposers.LinkType.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.LinkType), comparer, ConvertLinkType((BeamLinkType)value)),
            [BeamExposers.TransmissionType.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.TransmissionType), comparer, ConvertTransmissionType((BeamTransmissionType)value)),
            [BeamExposers.FootprintFile.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.FootprintFile), comparer, (string)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => beamDefinitionFilter;

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;

        internal static string ConvertLinkType(BeamLinkType linkType)
        {
            switch (linkType)
            {
                case BeamLinkType.Feeder:
                    return SlcSatellite_ManagementIds.Enums.Linktype.Feeder;
                case BeamLinkType.User:
                    return SlcSatellite_ManagementIds.Enums.Linktype.User;
                case BeamLinkType.Downlink:
                    return SlcSatellite_ManagementIds.Enums.Linktype.Downlink;
                case BeamLinkType.Uplink:
                    return SlcSatellite_ManagementIds.Enums.Linktype.Uplink;
                default:
                    throw new ArgumentOutOfRangeException(nameof(linkType), linkType, null);
            }
        }

        internal static string ConvertTransmissionType(BeamTransmissionType transmissionType)
        {
            switch (transmissionType)
            {
                case BeamTransmissionType.CarrierInCarrier:
                    return SlcSatellite_ManagementIds.Enums.Transmissiontype.CarrierInCarrier;
                case BeamTransmissionType.RX:
                    return SlcSatellite_ManagementIds.Enums.Transmissiontype.RX;
                case BeamTransmissionType.TX:
                    return SlcSatellite_ManagementIds.Enums.Transmissiontype.TX;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transmissionType), transmissionType, null);
            }
        }
    }
}
