namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying.Transponder;

    [TestClass]
    public class TransponderFilterTranslatorTests
    {
        [TestMethod]
        public void Translate_NameFilter_ReturnsAndWithDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act
            var result = translator.Translate(TransponderExposers.TransponderName.Equal("TP1"));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_TrueFilter_StillAppliesDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act
            var result = translator.Translate(new TRUEFilterElement<Transponder>());

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_GuidFilters_AreSupported()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var id = Guid.NewGuid();

            // Act & Assert
            AssertTranslated(translator.Translate(TransponderExposers.TransponderSatellite.Equal(id)));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderBeam.Equal(id)));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderDOMResource.Equal(id)));
        }

        [TestMethod]
        public void Translate_NumericAndDateAndStringFilters_AreSupported()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(TransponderExposers.TransponderBandwidth.Equal(36.0)));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderStartFrequency.Equal(10.0)));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderStopFrequency.Equal(20.0)));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderDownlinkStartFreq.Equal(1.0)));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderDownlinkEndFreq.Equal(2.0)));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderRollingWindow.Equal(3.0)));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderHardEndDate.Equal(new DateTime(2020, 1, 1))));
            AssertTranslated(translator.Translate(TransponderExposers.TransponderPhoneNumber.Equal("123")));
        }

        [DataTestMethod]
        [DataRow(TransponderBandType.C)]
        [DataRow(TransponderBandType.K)]
        [DataRow(TransponderBandType.Ka)]
        [DataRow(TransponderBandType.Ku)]
        [DataRow(TransponderBandType.L)]
        [DataRow(TransponderBandType.X)]
        public void Translate_BandFilter_IsSupported(TransponderBandType bandType)
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act
            var result = translator.Translate(TransponderExposers.TransponderBand.Equal(bandType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownBandFilter_Throws()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var filter = TransponderExposers.TransponderBand.Equal((TransponderBandType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        [DataTestMethod]
        [DataRow(TransponderPolarizationType.Circular)]
        [DataRow(TransponderPolarizationType.Linear)]
        public void Translate_PolarizationFilter_IsSupported(TransponderPolarizationType polarizationType)
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act
            var result = translator.Translate(TransponderExposers.TransponderPolarization.Equal(polarizationType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownPolarizationFilter_Throws()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var filter = TransponderExposers.TransponderPolarization.Equal((TransponderPolarizationType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        [DataTestMethod]
        [DataRow(TransponderUplinkPolarizationType.Horizontal)]
        [DataRow(TransponderUplinkPolarizationType.Vertical)]
        [DataRow(TransponderUplinkPolarizationType.RHCP)]
        [DataRow(TransponderUplinkPolarizationType.LHCP)]
        public void Translate_UplinkPolarizationFilter_IsSupported(TransponderUplinkPolarizationType type)
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act
            var result = translator.Translate(TransponderExposers.TransponderUplinkPolarization.Equal(type));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownUplinkPolarizationFilter_Throws()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var filter = TransponderExposers.TransponderUplinkPolarization.Equal((TransponderUplinkPolarizationType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        [DataTestMethod]
        [DataRow(TransponderDownlinkPolarizationType.Horizontal)]
        [DataRow(TransponderDownlinkPolarizationType.Vertical)]
        [DataRow(TransponderDownlinkPolarizationType.LHCP)]
        [DataRow(TransponderDownlinkPolarizationType.RHCP)]
        public void Translate_DownlinkPolarizationFilter_IsSupported(TransponderDownlinkPolarizationType type)
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act
            var result = translator.Translate(TransponderExposers.TransponderDownlinkPolarization.Equal(type));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownDownlinkPolarizationFilter_Throws()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var filter = TransponderExposers.TransponderDownlinkPolarization.Equal((TransponderDownlinkPolarizationType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        private static void AssertTranslated(FilterElement<DomInstance> result)
        {
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.AreEqual(2, and.subFilters.Length);
        }
    }
}
