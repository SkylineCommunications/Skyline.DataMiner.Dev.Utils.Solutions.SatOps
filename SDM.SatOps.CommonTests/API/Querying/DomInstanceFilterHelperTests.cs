namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying;

    [TestClass]
    public class DomInstanceFilterHelperTests
    {
        [TestMethod]
        public void BuildOrFilter_NullField_ThrowsArgumentNullException()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid() };

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => DomInstanceFilterHelper.BuildOrFilter(null, ids));
        }

        [TestMethod]
        public void BuildOrFilter_NullIds_ReturnsNull()
        {
            // Arrange
            var field = new FieldDescriptorID(Guid.NewGuid());

            // Act
            var result = DomInstanceFilterHelper.BuildOrFilter(field, null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BuildOrFilter_EmptyIds_ReturnsNull()
        {
            // Arrange
            var field = new FieldDescriptorID(Guid.NewGuid());

            // Act
            var result = DomInstanceFilterHelper.BuildOrFilter(field, new List<Guid>());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BuildOrFilter_SingleId_ReturnsSingleFilterNotOrFilter()
        {
            // Arrange
            var field = new FieldDescriptorID(Guid.NewGuid());

            // Act
            var result = DomInstanceFilterHelper.BuildOrFilter(field, new List<Guid> { Guid.NewGuid() });

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result as ORFilterElement<DomInstance>);
        }

        [TestMethod]
        public void BuildOrFilter_MultipleIds_ReturnsOrFilterWithAllSubFilters()
        {
            // Arrange
            var field = new FieldDescriptorID(Guid.NewGuid());
            var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // Act
            var result = DomInstanceFilterHelper.BuildOrFilter(field, ids);

            // Assert
            var or = result as ORFilterElement<DomInstance>;
            Assert.IsNotNull(or);
            Assert.AreEqual(3, or.subFilters.Length);
        }
    }
}
