namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying;

    [TestClass]
    public class FilterElementFactoryTests
    {
        private const Comparer UnsupportedComparer = (Comparer)999;

        [TestMethod]
        public void Create_NullExposer_ThrowsArgumentNullException()
        {
            // Arrange
            Exposer<DomInstance, Guid> exposer = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => FilterElementFactory.Create(exposer, Comparer.Equals, Guid.NewGuid()));
        }

        [TestMethod]
        public void Create_StringExposerContains_ReturnsFilter()
        {
            // Act
            var result = FilterElementFactory.Create(DomInstanceExposers.Name, Comparer.Contains, "abc");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_StringExposerNotContains_ReturnsFilter()
        {
            // Act
            var result = FilterElementFactory.Create(DomInstanceExposers.Name, Comparer.NotContains, "abc");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_StringExposerRegex_ReturnsFilter()
        {
            // Act
            var result = FilterElementFactory.Create(DomInstanceExposers.Name, Comparer.Regex, "a.*");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_StringExposerNotRegex_ReturnsFilter()
        {
            // Act
            var result = FilterElementFactory.Create(DomInstanceExposers.Name, Comparer.NotRegex, "a.*");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_StringExposerNullValueContains_ReturnsFilter()
        {
            // Act
            var result = FilterElementFactory.Create(DomInstanceExposers.Name, Comparer.Contains, null);

            // Assert
            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [DataRow(Comparer.Equals)]
        [DataRow(Comparer.NotEquals)]
        [DataRow(Comparer.GT)]
        [DataRow(Comparer.GTE)]
        [DataRow(Comparer.LT)]
        [DataRow(Comparer.LTE)]
        public void Create_StringExposerManagedComparer_ReturnsManagedFilter(Comparer comparer)
        {
            // Act
            var result = FilterElementFactory.Create(DomInstanceExposers.Name, comparer, "abc");

            // Assert
            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [DataRow(Comparer.Equals)]
        [DataRow(Comparer.NotEquals)]
        [DataRow(Comparer.GT)]
        [DataRow(Comparer.GTE)]
        [DataRow(Comparer.LT)]
        [DataRow(Comparer.LTE)]
        public void Create_GuidExposerManagedComparer_ReturnsManagedFilter(Comparer comparer)
        {
            // Act
            var result = FilterElementFactory.Create(DomInstanceExposers.Id, comparer, Guid.NewGuid());

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_UnsupportedComparer_ThrowsNotSupportedException()
        {
            // Act & Assert
            Assert.ThrowsException<NotSupportedException>(() => FilterElementFactory.Create(DomInstanceExposers.Id, UnsupportedComparer, Guid.NewGuid()));
        }

        [TestMethod]
        public void Create_NonStringExposerRegex_ThrowsNotSupportedException()
        {
            // Act & Assert
            Assert.ThrowsException<NotSupportedException>(() => FilterElementFactory.Create(DomInstanceExposers.Id, Comparer.Regex, Guid.NewGuid()));
        }

        [TestMethod]
        public void Create_ListExposerNull_ThrowsArgumentNullException()
        {
            // Arrange
            Exposer<DomInstance, List<string>> exposer = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => FilterElementFactory.Create(exposer, Comparer.Contains, "abc"));
        }

        [TestMethod]
        public void Create_ListExposerContains_ReturnsFilter()
        {
            // Arrange
            var exposer = new Exposer<DomInstance, List<string>>(d => new List<string>(), new[] { "listField" });

            // Act
            var result = FilterElementFactory.Create(exposer, Comparer.Contains, "abc");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_ListExposerNotContains_ReturnsFilter()
        {
            // Arrange
            var exposer = new Exposer<DomInstance, List<string>>(d => new List<string>(), new[] { "listField" });

            // Act
            var result = FilterElementFactory.Create(exposer, Comparer.NotContains, "abc");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_ListExposerUnsupportedComparer_ThrowsNotSupportedException()
        {
            // Arrange
            var exposer = new Exposer<DomInstance, List<string>>(d => new List<string>(), new[] { "listField" });

            // Act & Assert
            Assert.ThrowsException<NotSupportedException>(() => FilterElementFactory.Create(exposer, Comparer.Equals, "abc"));
        }

        [TestMethod]
        public void Create_DynamicListExposerNull_ThrowsArgumentNullException()
        {
            // Arrange
            DynamicListExposer<DomInstance, object> exposer = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => FilterElementFactory.Create(exposer, Comparer.Equals, (object)"abc"));
        }

        [DataTestMethod]
        [DataRow(Comparer.Equals)]
        [DataRow(Comparer.NotEquals)]
        [DataRow(Comparer.GT)]
        [DataRow(Comparer.GTE)]
        [DataRow(Comparer.LT)]
        [DataRow(Comparer.LTE)]
        [DataRow(Comparer.Contains)]
        [DataRow(Comparer.NotContains)]
        public void Create_DynamicListExposerSupportedComparer_ReturnsFilter(Comparer comparer)
        {
            // Arrange
            var exposer = DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(Guid.NewGuid()));

            // Act
            var result = FilterElementFactory.Create(exposer, comparer, "abc");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_DynamicListExposerUnsupportedComparer_ThrowsNotSupportedException()
        {
            // Arrange
            var exposer = DomInstanceExposers.FieldValues.DomInstanceField(new FieldDescriptorID(Guid.NewGuid()));

            // Act & Assert
            Assert.ThrowsException<NotSupportedException>(() => FilterElementFactory.Create(exposer, UnsupportedComparer, "abc"));
        }
    }
}
