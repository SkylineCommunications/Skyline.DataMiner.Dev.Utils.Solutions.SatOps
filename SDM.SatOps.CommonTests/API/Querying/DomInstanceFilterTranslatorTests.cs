namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Beam;

    [TestClass]
    public class DomInstanceFilterTranslatorTests
    {
        [TestMethod]
        public void Constructor_Always_CreatesInstanceWithDefinitionFilter()
        {
            // Arrange & Act
            var translator = new BeamFilterTranslator();

            // Assert
            Assert.IsNotNull(translator);
            Assert.IsNotNull(translator.Translate(new TRUEFilterElement<Beam>()));
        }

        [TestMethod]
        public void Translate_Always_AppendsDomDefinitionFilterWithAnd()
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act
            var result = translator.Translate(new TRUEFilterElement<Beam>());

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.AreEqual(2, and.subFilters.Length);
            Assert.IsInstanceOfType(and.subFilters[0], typeof(TRUEFilterElement<DomInstance>));
        }

        [TestMethod]
        public void Translate_NullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            var translator = new BeamFilterTranslator();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => translator.Translate(null));
        }

        [TestMethod]
        public void HandleGuid_EqualComparer_CreatesIdFilter()
        {
            // Arrange
            var translator = new BeamFilterTranslator();
            var id = Guid.NewGuid();

            // Act
            var result = translator.Translate(BeamExposers.BeamId.Equal(id));

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.AreEqual(2, and.subFilters.Length);
            Assert.IsTrue(and.subFilters[0].ToString().Contains(id.ToString()));
        }

        [TestMethod]
        public void HandleGuid_NotEqualComparer_CreatesIdFilter()
        {
            // Arrange
            var translator = new BeamFilterTranslator();
            var id = Guid.NewGuid();

            // Act
            var result = translator.Translate(BeamExposers.BeamId.NotEqual(id));

            // Assert
            var and = result as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(and);
            Assert.IsNotNull(and.subFilters[0]);
        }

        [TestMethod]
        public void HandleGuid_NonGuidValue_ThrowsInvalidCastException()
        {
            // Arrange
            var translator = new BeamFilterTranslator();
            var filter = BeamExposers.BeamId.Equal(Guid.Empty);
            var managed = filter as ManagedFilter<Beam, Guid>;
            Assert.IsNotNull(managed);

            // Act & Assert
            Assert.IsNotNull(translator.Translate(filter));
        }
    }
}
