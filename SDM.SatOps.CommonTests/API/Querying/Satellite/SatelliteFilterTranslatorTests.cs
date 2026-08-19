namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Satellite;

    [TestClass]
    public class SatelliteFilterTranslatorTests
    {
        [TestMethod]
        public void Translate_NameFilter_ReturnsAndWithDefinitionFilter()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act
            var result = translator.Translate(SatelliteExposers.SatelliteName.Equal("MySat"));

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.AreEqual(2, and.subFilters.Length);
        }

        [TestMethod]
        public void Translate_StringFilters_AreSupported()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteAbbreviation.Equal("ABC")));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteOperator.Equal("Op")));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteCoverage.Equal("Cov")));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteApplications.Equal("App")));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteInfo.Equal("Info")));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteManufacturer.Equal("Man")));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteCountry.Equal("BE")));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteLaunchInfo.Equal("Launch")));
        }

        [TestMethod]
        public void Translate_NumericAndDateFilters_AreSupported()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteLongitudeForGEODegrees.Equal(12.5)));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteInclinationDegrees.Equal(0.5)));
            AssertTranslated(translator.Translate(SatelliteExposers.SatelliteLaunchInServiceDate.Equal(new DateTime(2020, 1, 1))));
        }

        [DataTestMethod]
        [DataRow(OrbitType.GEO)]
        [DataRow(OrbitType.MEO)]
        [DataRow(OrbitType.LEO)]
        public void Translate_OrbitFilter_IsSupported(OrbitType orbitType)
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act
            var result = translator.Translate(SatelliteExposers.SatelliteOrbit.Equal(orbitType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownOrbitFilter_Throws()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();
            var filter = SatelliteExposers.SatelliteOrbit.Equal((OrbitType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        [DataTestMethod]
        [DataRow(HemisphereType.Western)]
        [DataRow(HemisphereType.Eastern)]
        public void Translate_HemisphereFilter_IsSupported(HemisphereType hemisphereType)
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act
            var result = translator.Translate(SatelliteExposers.SatelliteHemisphere.Equal(hemisphereType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownHemisphereFilter_Throws()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();
            var filter = SatelliteExposers.SatelliteHemisphere.Equal((HemisphereType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        [TestMethod]
        public void Translate_TrueFilter_StillAppliesDefinitionFilter()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act
            var result = translator.Translate(new TRUEFilterElement<Satellite>());

            // Assert
            AssertTranslated(result);
        }

        private static void AssertTranslated(FilterElement<DomInstance> result)
        {
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.AreEqual(2, and.subFilters.Length);
        }
    }
}
