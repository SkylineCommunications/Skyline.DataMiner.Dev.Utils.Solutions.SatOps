namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Exceptions;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;

    /// <summary>
    /// Tests for the <c>TransponderResourceCreationMiddleware</c>.
    /// </summary>
    [TestClass]
    public class TransponderResourceCreationMiddlewareTests
    {
        [TestMethod]
        public void Constructor_NullConnection_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderResourceCreationMiddleware(null, id => Satellite.CreateNewSatellite()));
        }

        [TestMethod]
        public void Constructor_NullResolver_ThrowsArgumentNullException()
        {
            // Arrange
            var connection = new Mock<IConnection>();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderResourceCreationMiddleware(connection.Object, null));
        }

        [TestMethod]
        public void Constructor_ValidArguments_CreatesInstance()
        {
            // Arrange
            var connection = new Mock<IConnection>();

            // Act
            var sut = new TransponderResourceCreationMiddleware(connection.Object, id => Satellite.CreateNewSatellite());

            // Assert
            Assert.IsNotNull(sut);
        }

        
        [TestMethod]
        public void OnCreateSingle_ResourceReadFails_DoesNotCallNext()
        {
            // Arrange
            var sut = CreateSut();
            var called = false;
            var transponder = Transponder.CreateNewTransponder();
            transponder.Name = "TP1";

            // Act & Assert
            Assert.ThrowsException<DataMinerException>(() => sut.OnCreate(transponder, t => { called = true; return t; }));
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void OnCreateBatch_ResourceReadFails_DoesNotCallNext()
        {
            // Arrange
            var sut = CreateSut();
            var called = false;
            var transponder = Transponder.CreateNewTransponder();
            transponder.Name = "TP1";
            var transponders = new List<Transponder> { transponder };

            // Act & Assert
            Assert.ThrowsException<DataMinerException>(() => sut.OnCreate(transponders, t => { called = true; return new List<Transponder>(t); }));
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void OnUpdateSingle_WithoutResource_CallsNextAndReturnsResult()
        {
            // Arrange
            var sut = CreateSut();
            var transponder = Transponder.CreateNewTransponder();
            transponder.Name = "TP1";
            Transponder passed = null;

            // Act
            var result = sut.OnUpdate(transponder, t => { passed = t; return t; });

            // Assert
            Assert.AreSame(transponder, passed);
            Assert.AreSame(transponder, result);
        }

        [TestMethod]
        public void OnUpdateBatch_WithoutResources_CallsNextWithAllTransponders()
        {
            // Arrange
            var sut = CreateSut();
            var first = Transponder.CreateNewTransponder();
            first.Name = "TP1";
            var second = Transponder.CreateNewTransponder();
            second.Name = "TP2";
            List<Transponder> passed = null;

            // Act
            var result = sut.OnUpdate(new[] { first, second }, t =>
            {
                passed = new List<Transponder>(t);
                return passed;
            });

            // Assert
            Assert.IsNotNull(passed);
            Assert.AreEqual(2, passed.Count);
            Assert.AreSame(first, passed[0]);
            Assert.AreSame(second, passed[1]);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateBatch_EmptyCollection_CallsNextWithEmptyCollection()
        {
            // Arrange
            var sut = CreateSut();
            var called = false;

            // Act
            var result = sut.OnUpdate(new List<Transponder>(), t =>
            {
                called = true;
                return new List<Transponder>(t);
            });

            // Assert
            Assert.IsTrue(called);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void OnDeleteSingle_NullTransponder_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((Transponder)null, t => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullNext_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = CreateSut();
            var transponder = Transponder.CreateNewTransponder();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(transponder, (Action<Transponder>)null));
        }

        [TestMethod]
        public void OnDeleteSingle_ValidTransponder_CallsNext()
        {
            // Arrange
            var sut = CreateSut();
            var transponder = Transponder.CreateNewTransponder();
            transponder.Name = "TP1";
            Transponder passed = null;

            // Act
            sut.OnDelete(transponder, t => passed = t);

            // Assert
            Assert.AreSame(transponder, passed);
        }

        [TestMethod]
        public void OnDeleteBatch_NullCollection_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((IEnumerable<Transponder>)null, t => { }));
        }

        [TestMethod]
        public void OnDeleteBatch_NullNext_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = CreateSut();
            var transponders = new List<Transponder> { Transponder.CreateNewTransponder() };

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(transponders, (Action<IEnumerable<Transponder>>)null));
        }

        [TestMethod]
        public void OnDeleteBatch_ValidTransponders_CallsNextWithAllTransponders()
        {
            // Arrange
            var sut = CreateSut();
            var first = Transponder.CreateNewTransponder();
            first.Name = "TP1";
            var second = Transponder.CreateNewTransponder();
            second.Name = "TP2";
            List<Transponder> passed = null;

            // Act
            sut.OnDelete(new[] { first, second }, t => passed = new List<Transponder>(t));

            // Assert
            Assert.IsNotNull(passed);
            Assert.AreEqual(2, passed.Count);
            Assert.AreSame(first, passed[0]);
            Assert.AreSame(second, passed[1]);
        }

        [TestMethod]
        public void OnDeleteBatch_EmptyCollection_CallsNextWithEmptyCollection()
        {
            // Arrange
            var sut = CreateSut();
            List<Transponder> passed = null;

            // Act
            sut.OnDelete(new List<Transponder>(), t => passed = new List<Transponder>(t));

            // Assert
            Assert.IsNotNull(passed);
            Assert.AreEqual(0, passed.Count);
        }

        private static TransponderResourceCreationMiddleware CreateSut()
        {
            var connection = new Mock<IConnection>();
            return new TransponderResourceCreationMiddleware(connection.Object, id => Satellite.CreateNewSatellite());
        }
    }
}
