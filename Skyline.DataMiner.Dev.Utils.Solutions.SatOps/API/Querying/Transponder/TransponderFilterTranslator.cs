namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying.Transponder
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using DomModel = DOM.Model;

    internal class TransponderFilterTranslator : DomInstanceFilterTranslator<Transponder>
    {
        private readonly FilterElement<DomInstance> transponderDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal
            (DomModel.SlcSatellite_ManagementIds.Definitions.Transponders.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [TransponderExposers.TransponderName.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.TransponderName), comparer, (string)value),
            [TransponderExposers.TransponderSatellite.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.TransponderSatellite), comparer, (Guid)value),
            [TransponderExposers.TransponderBeam.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.Beam), comparer, (Guid)value),
            [TransponderExposers.TransponderBand.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.Band), comparer, ConvertBand((TransponderBandType)value)),
            [TransponderExposers.TransponderBandwidth.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.Bandwidth), comparer, (double)value),
            [TransponderExposers.TransponderStartFrequency.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.StartFrequency), comparer, (double)value),
            [TransponderExposers.TransponderStopFrequency.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.StopFrequency), comparer, (double)value),
            [TransponderExposers.TransponderPolarization.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.Polarization), comparer, ConvertPolarization((TransponderPolarizationType)value)),
            [TransponderExposers.TransponderDownlinkStartFreq.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.DownlinkStartFreq), comparer, (double)value),
            [TransponderExposers.TransponderDownlinkEndFreq.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.DownlinkEndFreq), comparer, (double)value),
            [TransponderExposers.TransponderRollingWindow.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.RollingWindow), comparer, (double)value),
            [TransponderExposers.TransponderHardEndDate.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.HardEndDate), comparer, (DateTime)value),
            [TransponderExposers.TransponderPhoneNumber.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.PhoneNumber), comparer, (string)value),
            [TransponderExposers.TransponderUplinkPolarization.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.UplinkPolarization), comparer, ConvertUplinkPolarization((TransponderUplinkPolarizationType)value)),
            [TransponderExposers.TransponderDownlinkPolarization.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.DownlinkPolarization), comparer, ConvertDownlinkPolarization((TransponderDownlinkPolarizationType)value)),
            [TransponderExposers.TransponderDOMResource.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Transponder.DOMResource), comparer, (Guid)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => transponderDefinitionFilter;

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;

        private static string ConvertBand(TransponderBandType bandType)
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

        private static string ConvertPolarization(TransponderPolarizationType polarizationType)
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
