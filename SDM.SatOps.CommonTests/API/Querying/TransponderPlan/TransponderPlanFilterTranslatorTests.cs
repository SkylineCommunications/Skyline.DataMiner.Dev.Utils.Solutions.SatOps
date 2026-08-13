namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying.TransponderPlan;

    [TestClass]
    public class TransponderPlanFilterTranslatorTests
    {
        [TestMethod]
        public void Translate_TrueFilter_StillAppliesDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderPlanFilterTranslator();

            // Act
            var result = translator.Translate(new TRUEFilterElement<TransponderPlan>());

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_PlanNameFilter_ReturnsAndWithDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderPlanFilterTranslator();

            // Act
            var result = translator.Translate(TransponderPlanExposers.PlanName.Equal("plan"));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_TimeFilters_AreSupported()
        {
            // Arrange
            var translator = new TransponderPlanFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(TransponderPlanExposers.StartTime.Equal(DateTime.UtcNow)));
            AssertTranslated(translator.Translate(TransponderPlanExposers.EndTime.Equal(DateTime.UtcNow.AddDays(1))));
        }

        [TestMethod]
        public void Translate_OtherFilters_AreSupported()
        {
            // Arrange
            var translator = new TransponderPlanFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(TransponderPlanExposers.IsPermanent.Equal(true)));
            AssertTranslated(translator.Translate(TransponderPlanExposers.DefaultSlotSize.Equal(2.5)));
            AssertTranslated(translator.Translate(TransponderPlanExposers.Transponder.Equal(Guid.NewGuid())));
        }

        private static void AssertTranslated(FilterElement<DomInstance> result)
        {
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.AreEqual(2, and.subFilters.Length);
        }
    }
}
