namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderSlot;

    [TestClass]
    public class TransponderSlotFilterTranslatorTests
    {
        [TestMethod]
        public void Translate_SlotNameFilter_ReturnsAndWithDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderSlotFilterTranslator();

            // Act
            var result = translator.Translate(TransponderSlotExposers.Name.Equal("Slot1"));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_TrueFilter_StillAppliesDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderSlotFilterTranslator();

            // Act
            var result = translator.Translate(new TRUEFilterElement<TransponderSlot>());

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_TransponderPlanGuidFilter_IsSupported()
        {
            // Arrange
            var translator = new TransponderSlotFilterTranslator();
            var id = Guid.NewGuid();

            // Act
            var result = translator.Translate(TransponderSlotExposers.TransponderPlan.Equal(id));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_DoubleFilters_AreSupported()
        {
            // Arrange
            var translator = new TransponderSlotFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(TransponderSlotExposers.StartFrequency.Equal(10.0)));
            AssertTranslated(translator.Translate(TransponderSlotExposers.EndFrequency.Equal(20.0)));
            AssertTranslated(translator.Translate(TransponderSlotExposers.Bandwidth.Equal(36.0)));
            AssertTranslated(translator.Translate(TransponderSlotExposers.UplinkFreq.Equal(1.0)));
            AssertTranslated(translator.Translate(TransponderSlotExposers.DownlinkFreq.Equal(2.0)));
        }

        [TestMethod]
        public void Translate_CombinedFilter_ReturnsAndWithDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderSlotFilterTranslator();
            var filter = new ANDFilterElement<TransponderSlot>(
                TransponderSlotExposers.Name.Equal("Slot1"),
                TransponderSlotExposers.StartFrequency.GreaterThan(5.0));

            // Act
            var result = translator.Translate(filter);

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.AreEqual(3, and.subFilters.Length);
        }

        [TestMethod]
        public void Translate_SlotIdFilter_IsSupported()
        {
            // Arrange
            var translator = new TransponderSlotFilterTranslator();

            // Act
            var result = translator.Translate(TransponderSlotExposers.Guid.Equal(Guid.NewGuid()));

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
