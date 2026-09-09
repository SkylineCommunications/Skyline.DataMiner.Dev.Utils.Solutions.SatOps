namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;

    [TestClass]
    public class SatOpsApiTests
    {
        [TestMethod]
        public void Constructor_WhenConnectionIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new SatOpsApi(null));
            Assert.AreEqual("connection", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_WhenConnectionIsShuttingDown_ThrowsInvalidOperationException()
        {
            // Arrange
            var connection = new Mock<IConnection>();
            connection.SetupGet(c => c.IsShuttingDown).Returns(true);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => new SatOpsApi(connection.Object));
        }

        [TestMethod]
        public void Constructor_WhenConnectionIsValid_SetsConnection()
        {
            // Arrange
            var connection = new Mock<IConnection>();

            // Act
            var api = new SatOpsApi(connection.Object);

            // Assert
            Assert.AreSame(connection.Object, api.Connection);
        }

        [TestMethod]
        public void Satellites_WhenAccessed_ReturnsSameInstanceEachTime()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var first = api.Satellites;
            var second = api.Satellites;

            // Assert
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void Beams_WhenAccessed_ReturnsSameInstanceEachTime()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var first = api.Beams;
            var second = api.Beams;

            // Assert
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void Transponders_WhenAccessed_ReturnsSameInstanceEachTime()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var first = api.Transponders;
            var second = api.Transponders;

            // Assert
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void TransponderPlans_WhenAccessed_ReturnsSameInstanceEachTime()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var first = api.TransponderPlans;
            var second = api.TransponderPlans;

            // Assert
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void TransponderPlanRows_WhenAccessed_ReturnsSameInstanceEachTime()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var first = api.TransponderPlanRows;
            var second = api.TransponderPlanRows;

            // Assert
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void TransponderRangeReservations_WhenAccessed_ReturnsSameInstanceEachTime()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var first = api.TransponderRangeReservations;
            var second = api.TransponderRangeReservations;

            // Assert
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void TransponderSlots_WhenAccessed_ReturnsSameInstanceEachTime()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var first = api.TransponderSlots;
            var second = api.TransponderSlots;

            // Assert
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void Repositories_WhenAccessedOnSameApi_AreDistinctInstances()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var satellites = api.Satellites;
            var beams = api.Beams;

            // Assert
            Assert.AreNotSame((object)satellites, (object)beams);
        }

        [TestMethod]
        public void MediaOpsPlan_WhenAccessed_ReturnsSameInstanceEachTime()
        {
            // Arrange
            var api = CreateApi();

            // Act
            var first = api.MediaOpsPlan;
            var second = api.MediaOpsPlan;

            // Assert
            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void IsInstalled_WhenConnectionCannotHandleMessages_DelegatesToOverloadAndThrows()
        {
            // Arrange
            var api = CreateApi();

            // Act & Assert
            Assert.ThrowsException<Skyline.DataMiner.Net.Exceptions.DataMinerException>(() => api.IsInstalled());
        }

        [TestMethod]
        public void IsInstalledOutVersion_WhenConnectionCannotHandleMessages_Throws()
        {
            // Arrange
            var api = CreateApi();

            // Act & Assert
            Assert.ThrowsException<Skyline.DataMiner.Net.Exceptions.DataMinerException>(() => api.IsInstalled(out _));
        }

        [TestMethod]
        public void SetLogger_WhenLoggerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var api = CreateApi();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => api.SetLogger(null));
            Assert.AreEqual("logger", exception.ParamName);
        }

        [TestMethod]
        public void SetLogger_WhenLoggerIsValid_SetsLogger()
        {
            // Arrange
            var api = CreateApi();
            var logger = new Mock<ILogger>();

            // Act
            api.SetLogger(logger.Object);

            // Assert
            Assert.AreSame(logger.Object, api.Logger);
        }

        [TestMethod]
        public void SetLogger_WhenCalledTwice_KeepsLastLogger()
        {
            // Arrange
            var api = CreateApi();
            var first = new Mock<ILogger>();
            var second = new Mock<ILogger>();

            // Act
            api.SetLogger(first.Object);
            api.SetLogger(second.Object);

            // Assert
            Assert.AreSame(second.Object, api.Logger);
        }

        private static SatOpsApi CreateApi()
        {
            var connection = new Mock<IConnection>();
            return new SatOpsApi(connection.Object);
        }
    }
}
