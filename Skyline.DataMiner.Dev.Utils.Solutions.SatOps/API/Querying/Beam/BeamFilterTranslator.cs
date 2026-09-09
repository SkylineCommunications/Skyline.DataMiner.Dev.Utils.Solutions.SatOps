namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Beam
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
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
            [BeamExposers.BeamId.fieldName] = HandleGuid,
            [BeamExposers.BeamName.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.BeamName), comparer, (string)value),
            [BeamExposers.BeamSatellite.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.BeamSatellite), comparer, (Guid)value),
            [BeamExposers.BeamLinkType.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.LinkType), comparer, ConvertLinkType((BeamLinkType)value)),
            [BeamExposers.BeamTransmissionType.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.TransmissionType), comparer, ConvertTransmissionType((BeamTransmissionType)value)),
            [BeamExposers.FootprintFile.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (SlcSatellite_ManagementIds.Sections.Beam.FootprintFile), comparer, (string)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => beamDefinitionFilter;

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;

        private static string ConvertLinkType(BeamLinkType linkType)
        {
            switch (linkType)
            {
                case BeamLinkType.Downlink:
                    return SlcSatellite_ManagementIds.Enums.Linktype.Downlink;
                case BeamLinkType.Uplink:
                    return SlcSatellite_ManagementIds.Enums.Linktype.Uplink;
                default:
                    throw new ArgumentOutOfRangeException(nameof(linkType), linkType, null);
            }
        }

        private static string ConvertTransmissionType(BeamTransmissionType transmissionType)
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
