namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Beam;

    [TestClass]
    public class BeamFilterTranslatorTests
    {
        [TestMethod]
        public void Translate_NameFilter_ReturnsAndWithDefinitionFilter()
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act
            var result = translator.Translate(BeamExposers.BeamName.Equal("MyBeam"));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_FootprintFileFilter_IsSupported()
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act
            var result = translator.Translate(BeamExposers.FootprintFile.Equal("file.txt"));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_SatelliteGuidFilter_IsSupported()
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act
            var result = translator.Translate(BeamExposers.BeamSatellite.Equal(Guid.NewGuid()));

            // Assert
            AssertTranslated(result);
        }

        [DataTestMethod]
        [DataRow(BeamLinkType.Downlink)]
        [DataRow(BeamLinkType.Uplink)]
        public void Translate_LinkTypeFilter_IsSupported(BeamLinkType linkType)
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act
            var result = translator.Translate(BeamExposers.BeamLinkType.Equal(linkType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownLinkTypeFilter_Throws()
        {
            // Arrange
            var translator = new BeamFilterTranslator();
            var filter = BeamExposers.BeamLinkType.Equal((BeamLinkType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        [DataTestMethod]
        [DataRow(BeamTransmissionType.CarrierInCarrier)]
        [DataRow(BeamTransmissionType.RX)]
        [DataRow(BeamTransmissionType.TX)]
        public void Translate_TransmissionTypeFilter_IsSupported(BeamTransmissionType transmissionType)
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act
            var result = translator.Translate(BeamExposers.BeamTransmissionType.Equal(transmissionType));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_UnknownTransmissionTypeFilter_Throws()
        {
            // Arrange
            var translator = new BeamFilterTranslator();
            var filter = BeamExposers.BeamTransmissionType.Equal((BeamTransmissionType)999);

            // Act & Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => translator.Translate(filter));
        }

        [TestMethod]
        public void Translate_TrueFilter_StillAppliesDefinitionFilter()
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act
            var result = translator.Translate(new TRUEFilterElement<Beam>());

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_BeamIdFilter_IsSupported()
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act
            var result = translator.Translate(BeamExposers.BeamId.Equal(Guid.NewGuid()));

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
