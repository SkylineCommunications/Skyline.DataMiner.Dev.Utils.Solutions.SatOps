namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Constants;

    [TestClass]
    public class SatelliteManagementConstantTests
    {
        [TestMethod]
        public void SatelliteManagementConstant_ExposesModuleId()
        {
            Assert.AreEqual("(slc)satellite_management", SatelliteManagementConstant.SatelliteManagementModuleId);
        }

        [TestMethod]
        public void SatelliteManagementConstant_ExposesPredefinedGuids()
        {
            Assert.AreEqual(new Guid("65a3e6d9-d7e1-4c1c-9738-91e01694675f"), SatelliteManagementConstant.SatelliteCapabilityGuid);
            Assert.AreEqual(new Guid("e303cf10-5f49-4a61-af8e-43196961a230"), SatelliteManagementConstant.TransponderBandwidthGuid);
            Assert.AreEqual(new Guid("55c05c03-6229-49d3-bc5f-9404c26125a4"), SatelliteManagementConstant.BandwidthSizeGuid);
            Assert.AreEqual(new Guid("91ba20a5-4c5b-479b-b441-48697c3461a8"), SatelliteManagementConstant.TransponderResourcePoolGuid);
        }

        [TestMethod]
        public void SatelliteManagementConstant_ExposesNamingConstants()
        {
            Assert.AreEqual("Transponders", SatelliteManagementConstant.Transponders);
            Assert.AreEqual("Transponder Bandwidth", SatelliteManagementConstant.TransponderBandwidthCapacityName);
            Assert.AreEqual("Satellite", SatelliteManagementConstant.SatelliteCapabilityName);
            Assert.AreEqual("Bandwidth Size", SatelliteManagementConstant.BandwidthSizeParameterName);
            Assert.AreEqual("Transponder Slot", SatelliteManagementConstant.PropertyInfoName);
            Assert.AreEqual("MediaOps", SatelliteManagementConstant.PropertyInfoScope);
            Assert.AreEqual("Satellite Transponder", SatelliteManagementConstant.PropertyInfoSectionName);
        }
    }
}