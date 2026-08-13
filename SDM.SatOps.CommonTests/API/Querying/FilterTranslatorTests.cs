namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying.Satellite;

    [TestClass]
    public class FilterTranslatorTests
    {
        [TestMethod]
        public void Translate_NullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => translator.Translate(null));
        }

        [TestMethod]
        public void Translate_AndFilter_ReturnsAndFilterElement()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();
            var filter = new ANDFilterElement<Satellite>(
                SatelliteExposers.SatelliteAbbreviation.Equal("A"),
                SatelliteExposers.SatelliteOperator.Equal("B"));

            // Act
            var result = translator.Translate(filter);

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.AreEqual(3, and.subFilters.Length);
        }

        [TestMethod]
        public void Translate_OrFilter_ReturnsOrFilterElement()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();
            var filter = new ORFilterElement<Satellite>(
                SatelliteExposers.SatelliteAbbreviation.Equal("A"),
                SatelliteExposers.SatelliteOperator.Equal("B"));

            // Act
            var result = translator.Translate(filter);

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            var or = and.subFilters[0] as ORFilterElement<DomInstance>;
            Assert.IsNotNull(or);
            Assert.AreEqual(2, or.subFilters.Length);
        }

        [TestMethod]
        public void Translate_TrueFilter_ReturnsTrueFilterElement()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act
            var result = translator.Translate(new TRUEFilterElement<Satellite>());

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.IsInstanceOfType(and.subFilters[0], typeof(TRUEFilterElement<DomInstance>));
        }

        [TestMethod]
        public void Translate_FalseFilter_ReturnsFalseFilterElement()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act
            var result = translator.Translate(new FALSEFilterElement<Satellite>());

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.IsInstanceOfType(and.subFilters[0], typeof(FALSEFilterElement<DomInstance>));
        }

        [TestMethod]
        public void Translate_ManagedFilter_ReturnsTranslatedFilter()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();

            // Act
            var result = translator.Translate(SatelliteExposers.SatelliteAbbreviation.Equal("ABC"));

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Translate_UnsupportedFilter_ThrowsNotSupportedException()
        {
            // Arrange
            var translator = new SatelliteFilterTranslator();
            var filter = new Mock<FilterElement<Satellite>>().Object;

            // Act & Assert
            Assert.ThrowsException<NotSupportedException>(() => translator.Translate(filter));
        }
    }
}
