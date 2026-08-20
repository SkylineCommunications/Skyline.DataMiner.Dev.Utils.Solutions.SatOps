namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlanRow;

    [TestClass]
    public class TransponderPlanRowFilterTranslatorTests
    {
        [TestMethod]
        public void Translate_TransponderPlanFilter_ReturnsAndWithDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderPlanRowFilterTranslator();

            // Act
            var result = translator.Translate(TransponderPlanRowExposers.TransponderPlan.Equal(Guid.NewGuid()));

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_NumericFilters_AreSupported()
        {
            // Arrange
            var translator = new TransponderPlanRowFilterTranslator();

            // Act & Assert
            AssertTranslated(translator.Translate(TransponderPlanRowExposers.Bandwidth.Equal(36.0)));
            AssertTranslated(translator.Translate(TransponderPlanRowExposers.StepSize.Equal(1.5)));
            AssertTranslated(translator.Translate(TransponderPlanRowExposers.Offset.Equal(-2.5)));
            AssertTranslated(translator.Translate(TransponderPlanRowExposers.Limit.Equal(0.0)));
        }

        [TestMethod]
        public void Translate_TrueFilter_StillAppliesDefinitionFilter()
        {
            // Arrange
            var translator = new TransponderPlanRowFilterTranslator();

            // Act
            var result = translator.Translate(new TRUEFilterElement<TransponderPlanRow>());

            // Assert
            AssertTranslated(result);
        }

        [TestMethod]
        public void Translate_PlanRowIdFilter_IsSupported()
        {
            // Arrange
            var translator = new TransponderPlanRowFilterTranslator();

            // Act
            var result = translator.Translate(TransponderPlanRowExposers.PlanRowId.Equal(Guid.NewGuid()));

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
