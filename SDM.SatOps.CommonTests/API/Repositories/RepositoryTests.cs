namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories;

    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void Constructor_WhenSatOpsApiIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new TestRepository(null));
            Assert.AreEqual("satOpsApi", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_WhenSatOpsApiIsProvided_DoesNotThrow()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var repository = new TestRepository(api);

            // Assert
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void SatOpsApi_WhenConstructedWithApi_ReturnsSameInstance()
        {
            // Arrange
            var api = CreateApi();
            var repository = new TestRepository(api);

            // Act
            var result = repository.SatOpsApi;

            // Assert
            Assert.AreSame(api, result);
        }

        [TestMethod]
        public void SatOpsApi_WhenCalledMultipleTimes_ReturnsSameInstance()
        {
            // Arrange
            var api = CreateApi();
            var repository = new TestRepository(api);

            // Act
            var first = repository.SatOpsApi;
            var second = repository.SatOpsApi;

            // Assert
            Assert.AreSame(first, second);
        }

        private static SatOpsApi CreateApi()
        {
            var connection = new Mock<IConnection>();
            return new SatOpsApi(connection.Object);
        }

        private sealed class TestRepository : Repository
        {
            public TestRepository(SatOpsApi satOpsApi)
                : base(satOpsApi)
            {
            }
        }
    }
}
