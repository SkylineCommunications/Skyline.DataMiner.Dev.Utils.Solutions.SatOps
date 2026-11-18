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
            var result = translator.Translate(SatelliteExposers.Name.Equal("MySat"));

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
            AssertTranslated(translator.Translate(SatelliteExposers.Abbreviation.Equal("ABC")));
            AssertTranslated(translator.Translate(SatelliteExposers.Operator.Equal("Op")));
            AssertTranslated(translator.Translate(SatelliteExposers.Coverage.Equal("Cov")));
            AssertTranslated(translator.Translate(SatelliteExposers.Applications.Equal("App")));
            AssertTranslated(translator.Translate(SatelliteExposers.Info.Equal("Info")));
            AssertTranslated(translator.Translate(SatelliteExposers.Manufacturer.Equal("Man")));
            AssertTranslated(translator.Translate(SatelliteExposers.Country.Equal("BE")));
            AssertTranslated(translator.Translate(SatelliteExposers.LaunchInfo.Equal("Launch")));
        }

        [TestMethod]
        public void Translate_NumericAndDateFilters_AreSupported()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(SatelliteExposers.LongitudeForGeoDegrees.Equal(12.5)));
            AssertTranslated(translator.Translate(SatelliteExposers.InclinationDegrees.Equal(0.5)));
            AssertTranslated(translator.Translate(SatelliteExposers.LaunchInServiceDate.Equal(new DateTime(2020, 1, 1))));
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
            var result = translator.Translate(SatelliteExposers.Orbit.Equal(orbitType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownOrbitFilter_Throws()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();
            var filter = SatelliteExposers.Orbit.Equal((OrbitType)999);

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
            var result = translator.Translate(SatelliteExposers.Hemisphere.Equal(hemisphereType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownHemisphereFilter_Throws()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();
            var filter = SatelliteExposers.Hemisphere.Equal((HemisphereType)999);

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

        [TestMethod]
        public void Translate_SatelliteIdFilter_IsSupported()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act
            var result = translator.Translate(SatelliteExposers.Guid.Equal(Guid.NewGuid()));

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
