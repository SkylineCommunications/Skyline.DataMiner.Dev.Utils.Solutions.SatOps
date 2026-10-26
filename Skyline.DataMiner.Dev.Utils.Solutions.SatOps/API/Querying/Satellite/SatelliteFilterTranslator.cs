namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Satellite
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying;
    using System;
    using System.Collections.Generic;
    using DomModel = DOM.Model;

    internal class SatelliteFilterTranslator : DomInstanceFilterTranslator<Satellite>
    {
        private readonly FilterElement<DomInstance> satelliteDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal
            (DomModel.SlcSatellite_ManagementIds.Definitions.Satellites.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [SatelliteExposers.Guid.fieldName] = HandleGuid,
            [SatelliteExposers.Name.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.SatelliteName), comparer, (string)value),
            [SatelliteExposers.Abbreviation.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.SatelliteAbbreviation), comparer, (string)value),
            [SatelliteExposers.Orbit.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.Orbit), comparer, ConvertOrbit((OrbitType)value)),
            [SatelliteExposers.LongitudeForGeoDegrees.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.LongitudeForGEODegrees), comparer, (double)value),
            [SatelliteExposers.InclinationDegrees.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.InclinationDegrees), comparer, (double)value),
            [SatelliteExposers.Hemisphere.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.Hemisphere), comparer, ConvertHemisphere((HemisphereType)value)),
            [SatelliteExposers.Operator.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Satellite.Operator), comparer, (string)value),
            [SatelliteExposers.Coverage.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Satellite.Coverage), comparer, (string)value),
            [SatelliteExposers.Applications.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Satellite.Applications), comparer, (string)value),
            [SatelliteExposers.Info.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Satellite.Info), comparer, (string)value),
            [SatelliteExposers.Manufacturer.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Origin.Manufacturer), comparer, (string)value),
            [SatelliteExposers.Country.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Origin.Country), comparer, (string)value),
            [SatelliteExposers.LaunchInfo.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.LaunchInformation.LaunchInfo), comparer, (string)value),
            [SatelliteExposers.LaunchInServiceDate.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.LaunchInformation.LaunchInServiceDate), comparer, (DateTime)value),
        };

        protected override FilterElement<DomInstance> DomDefinitionFilter => satelliteDefinitionFilter;

        protected override Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> Handlers => handlers;

        private static string ConvertOrbit(OrbitType orbitType)
        {
            switch (orbitType)
            {
                case OrbitType.GEO:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Orbit.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.GEO);
                case OrbitType.MEO:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Orbit.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.MEO);
                case OrbitType.LEO:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Orbit.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.OrbitEnum.LEO);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orbitType), orbitType, null);
            }
        }

        internal static string ConvertHemisphere(HemisphereType hemisphereType)
        {
            switch (hemisphereType)
            {
                case HemisphereType.Western:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Hemisphere.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.HemisphereEnum.Western);
                case HemisphereType.Eastern:
                    return DomModel.SlcSatellite_ManagementIds.Enums.Hemisphere.ToValue(DomModel.SlcSatellite_ManagementIds.Enums.HemisphereEnum.Eastern);
                default:
                    throw new ArgumentOutOfRangeException(nameof(hemisphereType), hemisphereType, null);
            }
        }
    }
}
