namespace Skyline.DataMiner.SDM.SatOps.Common.API.Constants
{
    using System;

    /// <summary>
    /// Public facade for Satellite Management constants.
    /// </summary>
    public static class SatelliteManagementConstant
    {
        /// <summary>
        /// Gets the DOM module identifier for Satellite Management.
        /// </summary>
        public static string ModuleId
        {
            get { return Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.SlcSatellite_ManagementIds.ModuleId; }
        }

        /// <summary>
        /// Gets the Transponders collection or entity name.
        /// </summary>
        public static string Transponders
        {
            get { return NamingConstants.Transponders; }
        }

        /// <summary>
        /// Gets the transponder bandwidth capacity name.
        /// </summary>
        public static string TransponderBandwidthCapacityName
        {
            get { return NamingConstants.TransponderBandwidthCapacityName; }
        }

        /// <summary>
        /// Gets the satellite capability name.
        /// </summary>
        public static string SatelliteCapabilityName
        {
            get { return NamingConstants.SatelliteCapabilityName; }
        }

        /// <summary>
        /// Gets the bandwidth size parameter name.
        /// </summary>
        public static string BandwidthSizeParameterName
        {
            get { return NamingConstants.BandwidthSizeParameterName; }
        }

        /// <summary>
        /// Gets the transponder slot property info name.
        /// </summary>
        public static string PropertyInfoName
        {
            get { return NamingConstants.PropertyInfoName; }
        }

        /// <summary>
        /// Gets the transponder slot property info scope.
        /// </summary>
        public static string PropertyInfoScope
        {
            get { return NamingConstants.PropertyInfoScope; }
        }

        /// <summary>
        /// Gets the satellite transponder property info section name.
        /// </summary>
        public static string PropertyInfoSectionName
        {
            get { return NamingConstants.PropertyInfoSectionName; }
        }

        /// <summary>
        /// Gets the GUID representing the Satellite capability entity.
        /// </summary>
        public static Guid SatelliteCapabilityGuid
        {
            get { return PredefinedGuids.SatelliteCapabilityGuid; }
        }

        /// <summary>
        /// Gets the GUID representing the Transponder Bandwidth entity.
        /// </summary>
        public static Guid TransponderBandwidthGuid
        {
            get { return PredefinedGuids.TransponderBandwidthGuid; }
        }

        /// <summary>
        /// Gets the GUID representing the Bandwidth Size parameter.
        /// </summary>
        public static Guid BandwidthSizeGuid
        {
            get { return PredefinedGuids.BandwidthSizeGuid; }
        }

        /// <summary>
        /// Gets the GUID representing the Transponder Resource Pool entity.
        /// </summary>
        public static Guid TransponderResourcePoolGuid
        {
            get { return PredefinedGuids.TransponderResourcePoolGuid; }
        }
    }
}