namespace Skyline.DataMiner.SDM.SatOps.Common.API.Constants
{
    using System;

    internal static class PredefinedGuids
    {
        /// <summary>
        /// GUID representing the Satellite capability entity.
        /// </summary>
        public static readonly Guid SatelliteCapabilityGuid = new Guid("65a3e6d9-d7e1-4c1c-9738-91e01694675f");

        /// <summary>
        /// GUID representing the Transponder Bandwidth entity.
        /// </summary>
        public static readonly Guid TransponderBandwidthGuid = new Guid("e303cf10-5f49-4a61-af8e-43196961a230");

        /// <summary>
        /// GUID representing the Bandwidth Size parameter.
        /// </summary>
        public static readonly Guid BandwidthSizeGuid = new Guid("55c05c03-6229-49d3-bc5f-9404c26125a4");

        /// <summary>
        /// GUID representing the Transponder Resource Pool entity.
        /// </summary>
        public static readonly Guid TransponderResourcePoolGuid = new Guid("91ba20a5-4c5b-479b-b441-48697c3461a8");
    }
}
