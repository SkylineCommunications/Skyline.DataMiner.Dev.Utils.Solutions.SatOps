namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;

    [TestClass]
    public class TransponderFilterTranslatorTests
    {
        [TestMethod]
        public void Translate_NameFilter_ReturnsAndWithDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act
            var result = translator.Translate(TransponderExposers.Name.Equal("TP1"));

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
            AssertTranslated(translator.Translate(TransponderExposers.Satellite.Equal(id)));
            AssertTranslated(translator.Translate(TransponderExposers.Beam.Equal(id)));
            AssertTranslated(translator.Translate(TransponderExposers.DOMResource.Equal(id)));
        }

        [TestMethod]
        public void Translate_NumericAndDateAndStringFilters_AreSupported()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(TransponderExposers.Bandwidth.Equal(36.0)));
            AssertTranslated(translator.Translate(TransponderExposers.StartFrequency.Equal(10.0)));
            AssertTranslated(translator.Translate(TransponderExposers.StopFrequency.Equal(20.0)));
            AssertTranslated(translator.Translate(TransponderExposers.DownlinkStartFreq.Equal(1.0)));
            AssertTranslated(translator.Translate(TransponderExposers.DownlinkEndFreq.Equal(2.0)));
            AssertTranslated(translator.Translate(TransponderExposers.RollingWindow.Equal(3.0)));
            AssertTranslated(translator.Translate(TransponderExposers.HardEndDate.Equal(new DateTime(2020, 1, 1))));
            AssertTranslated(translator.Translate(TransponderExposers.PhoneNumber.Equal("123")));
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
            var result = translator.Translate(TransponderExposers.Band.Equal(bandType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownBandFilter_Throws()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var filter = TransponderExposers.Band.Equal((TransponderBandType)999);

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
            var result = translator.Translate(TransponderExposers.Polarization.Equal(polarizationType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownPolarizationFilter_Throws()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var filter = TransponderExposers.Polarization.Equal((TransponderPolarizationType)999);

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
            var result = translator.Translate(TransponderExposers.UplinkPolarization.Equal(type));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownUplinkPolarizationFilter_Throws()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var filter = TransponderExposers.UplinkPolarization.Equal((TransponderUplinkPolarizationType)999);

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
            var result = translator.Translate(TransponderExposers.DownlinkPolarization.Equal(type));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownDownlinkPolarizationFilter_Throws()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();
            var filter = TransponderExposers.DownlinkPolarization.Equal((TransponderDownlinkPolarizationType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        [TestMethod]
        public void Translate_TransponderIdFilter_IsSupported()
        {
            // Arrange
            var translator = new TransponderFilterTranslator();

            // Act
            var result = translator.Translate(TransponderExposers.Guid.Equal(Guid.NewGuid()));

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
