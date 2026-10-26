namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying;
    using System;
    using System.Collections.Generic;
    using DomModel = DOM.Model;

    internal class TransponderFilterTranslator : DomInstanceFilterTranslator<Transponder>
    {
        private readonly FilterElement<DomInstance> transponderDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal
            (DomModel.SlcSatellite_ManagementIds.Definitions.Transponders.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [TransponderExposers.Guid.fieldName] = HandleGuid,
            [TransponderExposers.Name.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.TransponderName), comparer, (string)value),
            [TransponderExposers.Satellite.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.TransponderSatellite), comparer, (Guid)value),
            [TransponderExposers.Beam.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.Beam), comparer, (Guid)value),
            [TransponderExposers.Band.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.Band), comparer, ConvertBand((TransponderBandType)value)),
            [TransponderExposers.Bandwidth.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.Bandwidth), comparer, (double)value),
            [TransponderExposers.StartFrequency.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.StartFrequency), comparer, (double)value),
            [TransponderExposers.StopFrequency.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.StopFrequency), comparer, (double)value),
            [TransponderExposers.Polarization.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.Polarization), comparer, ConvertPolarization((TransponderPolarizationType)value)),
            [TransponderExposers.DownlinkStartFreq.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.DownlinkStartFreq), comparer, (double)value),
            [TransponderExposers.DownlinkEndFreq.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.DownlinkEndFreq), comparer, (double)value),
            [TransponderExposers.RollingWindow.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.RollingWindow), comparer, (double)value),
            [TransponderExposers.HardEndDate.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.HardEndDate), comparer, (DateTime)value),
            [TransponderExposers.PhoneNumber.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.PhoneNumber), comparer, (string)value),
            [TransponderExposers.UplinkPolarization.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.UplinkPolarization), comparer, ConvertUplinkPolarization((TransponderUplinkPolarizationType)value)),
            [TransponderExposers.DownlinkPolarization.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.DownlinkPolarization), comparer, ConvertDownlinkPolarization((TransponderDownlinkPolarizationType)value)),
            [TransponderExposers.DOMResource.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.DOMResource), comparer, (Guid)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => transponderDefinitionFilter;

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;

        internal static string ConvertBand(TransponderBandType bandType)
        {
            switch (bandType)
            {
                case TransponderBandType.C:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Band.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.C);
                case TransponderBandType.K:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Band.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.K);
                case TransponderBandType.Ka:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Band.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.Ka);
                case TransponderBandType.Ku:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Band.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.Ku);
                case TransponderBandType.L:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Band.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.L);
                case TransponderBandType.X:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Band.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.BandEnum.X);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bandType), bandType, null);
            }
        }

        internal static string ConvertPolarization(TransponderPolarizationType polarizationType)
        {
            switch (polarizationType)
            {
                case TransponderPolarizationType.Circular:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Polarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.PolarizationEnum.Circular);
                case TransponderPolarizationType.Linear:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Polarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.PolarizationEnum.Linear);
                default:
                    throw new ArgumentOutOfRangeException(nameof(polarizationType), polarizationType, null);
            }
        }

        private static string ConvertUplinkPolarization(TransponderUplinkPolarizationType uplinkPolarizationType)
        {
            switch (uplinkPolarizationType)
            {
                case TransponderUplinkPolarizationType.Horizontal:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Uppolarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.Horizontal);
                case TransponderUplinkPolarizationType.Vertical:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Uppolarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.Vertical);
                case TransponderUplinkPolarizationType.RHCP:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Uppolarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.RHCP);
                case TransponderUplinkPolarizationType.LHCP:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Uppolarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.UppolarizationEnum.LHCP);
                default:
                    throw new ArgumentOutOfRangeException(nameof(uplinkPolarizationType), uplinkPolarizationType, null);
            }
        }

        private static string ConvertDownlinkPolarization(TransponderDownlinkPolarizationType downlinkPolarizationType)
        {
            switch (downlinkPolarizationType)
            {
                case TransponderDownlinkPolarizationType.Horizontal:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Downpolarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.Horizontal);
                case TransponderDownlinkPolarizationType.Vertical:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Downpolarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.Vertical);
                case TransponderDownlinkPolarizationType.LHCP:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Downpolarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.LHCP);
                case TransponderDownlinkPolarizationType.RHCP:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Downpolarization.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum.RHCP);
                default:
                    throw new ArgumentOutOfRangeException(nameof(downlinkPolarizationType), downlinkPolarizationType, null);
            }
        }
    }
}
