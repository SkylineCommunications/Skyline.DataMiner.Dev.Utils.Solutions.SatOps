namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying.Satellite
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using DomModel = DOM.Model;

    internal class SatelliteFilterTranslator : DomInstanceFilterTranslator<Satellite>
    {
        private readonly FilterElement<DomInstance> satelliteDefinitionFilter = DomInstanceExposers.DomDefinitionId.Equal
            (DomModel.SlcSatellite_ManagementIds.Definitions.Satellites.Id);

        private readonly Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>> handlers = new Dictionary<string, Func<Comparer, object, FilterElement<DomInstance>>>
        {
            [SatelliteExposers.SatelliteName.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.SatelliteName), comparer, (string)value),
            [SatelliteExposers.SatelliteAbbreviation.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.SatelliteAbbreviation), comparer, (string)value),
            [SatelliteExposers.SatelliteOrbit.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.Orbit), comparer, ConvertOrbit((OrbitType)value)),
            [SatelliteExposers.SatelliteLongitudeForGEODegrees.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.LongitudeForGEODegrees), comparer, (double)value),
            [SatelliteExposers.SatelliteInclinationDegrees.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.InclinationDegrees), comparer, (double)value),
            [SatelliteExposers.SatelliteHemisphere.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.General.Hemisphere), comparer, ConvertHemisphere((HemisphereType)value)),
            [SatelliteExposers.SatelliteOperator.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Satellite.Operator), comparer, (string)value),
            [SatelliteExposers.SatelliteCoverage.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Satellite.Coverage), comparer, (string)value),
            [SatelliteExposers.SatelliteApplications.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Satellite.Applications), comparer, (string)value),
            [SatelliteExposers.SatelliteInfo.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Satellite.Info), comparer, (string)value),
            [SatelliteExposers.SatelliteManufacturer.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Origin.Manufacturer), comparer, (string)value),
            [SatelliteExposers.SatelliteCountry.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.Origin.Country), comparer, (string)value),
            [SatelliteExposers.SatelliteLaunchInfo.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
                (DomModel.SlcSatellite_ManagementIds.Sections.LaunchInformation.LaunchInfo), comparer, (string)value),
            [SatelliteExposers.SatelliteLaunchInServiceDate.fieldName] = (comparer, value) => FilterElementFactory.Create(DomInstanceExposers.FieldValues.DomInstanceField
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

        private static string ConvertHemisphere(HemisphereType hemisphereType)
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
