namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.TransponderRangeReservation
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;

    using Reservation = Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation;

    [TestClass]
    public class TransponderRangeReservationRepositoryMiddlewareTests
    {
        [TestMethod]
        public void Constructor_WhenInnerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new TransponderRangeReservationRepositoryMiddleware(null, middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_WhenMiddlewareIsNull_DoesNotThrow()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();

            // Act
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void Count_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Count()).Returns(42L);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Count();

            // Assert
            Assert.AreEqual(42L, result);
            inner.Verify(x => x.Count(), Times.Once);
        }

        [TestMethod]
        public void Count_WithFilterAndNonCountableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<Reservation>>().Object;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(filter);

            // Assert
            Assert.AreEqual(7L, result);
            inner.Verify(x => x.Count(filter), Times.Once);
        }

        [TestMethod]
        public void Count_WithFilterAndCountableMiddleware_UsesMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<Reservation>>().Object;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);

            var middleware = new Mock<ICountableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnCount(filter, It.IsAny<Func<FilterElement<Reservation>, long>>()))
                .Returns((FilterElement<Reservation> f, Func<FilterElement<Reservation>, long> next) => next(f) + 1);

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(filter);

            // Assert
            Assert.AreEqual(8L, result);
            middleware.Verify(x => x.OnCount(filter, It.IsAny<Func<FilterElement<Reservation>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithQueryAndNonCountableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Reservation>>().Object;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(query);

            // Assert
            Assert.AreEqual(3L, result);
            inner.Verify(x => x.Count(query), Times.Once);
        }

        [TestMethod]
        public void Count_WithQueryAndCountableMiddleware_UsesMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Reservation>>().Object;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);

            var middleware = new Mock<ICountableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnCount(query, It.IsAny<Func<IQuery<Reservation>, long>>()))
                .Returns((IQuery<Reservation> q, Func<IQuery<Reservation>, long> next) => next(q) * 2);

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(query);

            // Assert
            Assert.AreEqual(6L, result);
            middleware.Verify(x => x.OnCount(query, It.IsAny<Func<IQuery<Reservation>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithNullQueryAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Count((IQuery<Reservation>)null)).Returns(0L);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Count((IQuery<Reservation>)null);

            // Assert
            Assert.AreEqual(0L, result);
            inner.Verify(x => x.Count((IQuery<Reservation>)null), Times.Once);
        }

        [TestMethod]
        public void Count_WithNullFilterAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Count((FilterElement<Reservation>)null)).Returns(5L);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Count((FilterElement<Reservation>)null);

            // Assert
            Assert.AreEqual(5L, result);
            inner.Verify(x => x.Count((FilterElement<Reservation>)null), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithNonBulkCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new Reservation[] { null };
            var expected = new List<Reservation>(items);
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Create(items)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create((IEnumerable<Reservation>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(items), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithBulkCreatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new Reservation[] { null };
            var expected = new List<Reservation>(items);
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Create(items)).Returns(expected);

            var middleware = new Mock<IBulkCreatableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnCreate(items, It.IsAny<Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>>>()))
                .Returns((IEnumerable<Reservation> o, Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>> next) => next(o));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create((IEnumerable<Reservation>)items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnCreate(items, It.IsAny<Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>>>()), Times.Once);
            inner.Verify(x => x.Create(items), Times.Once);
        }

        [TestMethod]
        public void CreateSingle_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            Reservation item = null;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Create(item)).Returns(item);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Create(item);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Create(item), Times.Once);
        }

        [TestMethod]
        public void CreateSingle_WithCreatableMiddleware_UsesMiddleware()
        {
            // Arrange
            Reservation item = null;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Create(item)).Returns(item);

            var middleware = new Mock<ICreatableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnCreate(item, It.IsAny<Func<Reservation, Reservation>>()))
                .Returns((Reservation o, Func<Reservation, Reservation> next) => next(o));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create(item);

            // Assert
            Assert.IsNull(result);
            middleware.Verify(x => x.OnCreate(item, It.IsAny<Func<Reservation, Reservation>>()), Times.Once);
            inner.Verify(x => x.Create(item), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithNonBulkRepositoryMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new Reservation[] { null };
            var expected = new List<Reservation>(items);
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.CreateOrUpdate(items)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.CreateOrUpdate(items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.CreateOrUpdate(items), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithBulkRepositoryMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new Reservation[] { null };
            var expected = new List<Reservation>(items);
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.CreateOrUpdate(items)).Returns(expected);

            var middleware = new Mock<IBulkRepositoryMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnCreateOrUpdate(items, It.IsAny<Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>>>()))
                .Returns((IEnumerable<Reservation> o, Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>> next) => next(o));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.CreateOrUpdate(items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnCreateOrUpdate(items, It.IsAny<Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>>>()), Times.Once);
            inner.Verify(x => x.CreateOrUpdate(items), Times.Once);
        }

        [TestMethod]
        public void DeleteById_Always_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            sut.Delete(id);

            // Assert
            inner.Verify(x => x.Delete(id), Times.Once);
        }

        [TestMethod]
        public void DeleteByIds_Always_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<Guid>)ids);

            // Assert
            inner.Verify(x => x.Delete(ids), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithNonBulkDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new Reservation[] { null };
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<Reservation>)items);

            // Assert
            inner.Verify(x => x.Delete(items), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithBulkDeletableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new Reservation[] { null };
            var inner = new Mock<ITransponderRangeReservationRepository>();

            var middleware = new Mock<IBulkDeletableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnDelete(items, It.IsAny<Action<IEnumerable<Reservation>>>()))
                .Callback((IEnumerable<Reservation> o, Action<IEnumerable<Reservation>> next) => next(o));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<Reservation>)items);

            // Assert
            middleware.Verify(x => x.OnDelete(items, It.IsAny<Action<IEnumerable<Reservation>>>()), Times.Once);
            inner.Verify(x => x.Delete(items), Times.Once);
        }

        [TestMethod]
        public void DeleteSingle_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            Reservation item = null;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            sut.Delete(item);

            // Assert
            inner.Verify(x => x.Delete(item), Times.Once);
        }

        [TestMethod]
        public void DeleteSingle_WithDeletableMiddleware_UsesMiddleware()
        {
            // Arrange
            Reservation item = null;
            var inner = new Mock<ITransponderRangeReservationRepository>();

            var middleware = new Mock<IDeletableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnDelete(item, It.IsAny<Action<Reservation>>()))
                .Callback((Reservation o, Action<Reservation> next) => next(o));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(item);

            // Assert
            middleware.Verify(x => x.OnDelete(item, It.IsAny<Action<Reservation>>()), Times.Once);
            inner.Verify(x => x.Delete(item), Times.Once);
        }

        [TestMethod]
        public void Read_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read()).Returns(expected);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read();

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(), Times.Once);
        }

        [TestMethod]
        public void ReadByTransponder_Always_DelegatesToInner()
        {
            // Arrange
            var transponderId = Guid.NewGuid();
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadByTransponder(transponderId)).Returns(expected);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadByTransponder(transponderId);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadByTransponder(transponderId), Times.Once);
        }

        [TestMethod]
        public void ReadByTimeWindow_Always_DelegatesToInner()
        {
            // Arrange
            var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddHours(1);
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadByTimeWindow(start, end)).Returns(expected);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadByTimeWindow(start, end);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadByTimeWindow(start, end), Times.Once);
        }

        [TestMethod]
        public void ReadByTransponderAndTimeWindow_Always_DelegatesToInner()
        {
            // Arrange
            var transponderId = Guid.NewGuid();
            var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddHours(2);
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadByTransponderAndTimeWindow(transponderId, start, end)).Returns(expected);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadByTransponderAndTimeWindow(transponderId, start, end);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadByTransponderAndTimeWindow(transponderId, start, end), Times.Once);
        }

        [TestMethod]
        public void ReadByTransponderAndTimeWindow_WithBoundaryTouchingWindow_DelegatesToInner()
        {
            // Arrange
            var transponderId = Guid.Empty;
            var start = DateTime.MinValue;
            var end = DateTime.MinValue;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadByTransponderAndTimeWindow(transponderId, start, end)).Returns((IEnumerable<Reservation>)null);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadByTransponderAndTimeWindow(transponderId, start, end);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.ReadByTransponderAndTimeWindow(transponderId, start, end), Times.Once);
        }

        [TestMethod]
        public void ReserveRange_WithoutSatelliteName_DelegatesToInner()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            sut.ReserveRange(reservationId, 10.5, 20.5);

            // Assert
            inner.Verify(x => x.ReserveRange(reservationId, 10.5, 20.5), Times.Once);
        }

        [TestMethod]
        public void ReserveRange_WithEqualFrequencies_DelegatesToInner()
        {
            // Arrange
            var reservationId = Guid.Empty;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.ReserveRange(reservationId, 0d, 0d);

            // Assert
            inner.Verify(x => x.ReserveRange(reservationId, 0d, 0d), Times.Once);
        }

        [TestMethod]
        public void ReserveRange_WithSatelliteName_DelegatesToInner()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            sut.ReserveRange(reservationId, 1d, 2d, "Sat-1");

            // Assert
            inner.Verify(x => x.ReserveRange(reservationId, 1d, 2d, "Sat-1"), Times.Once);
        }

        [TestMethod]
        public void ReserveRange_WithNullSatelliteName_DelegatesToInner()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.ReserveRange(reservationId, -1d, 2d, null);

            // Assert
            inner.Verify(x => x.ReserveRange(reservationId, -1d, 2d, null), Times.Once);
        }

        [TestMethod]
        public void GetSlotName_Always_ReturnsInnerValue()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.GetSlotName(reservationId)).Returns("SlotA");
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.GetSlotName(reservationId);

            // Assert
            Assert.AreEqual("SlotA", result);
            inner.Verify(x => x.GetSlotName(reservationId), Times.Once);
        }

        [TestMethod]
        public void GetSlotName_WhenInnerReturnsNull_ReturnsNull()
        {
            // Arrange
            var reservationId = Guid.Empty;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.GetSlotName(reservationId)).Returns((string)null);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.GetSlotName(reservationId);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.GetSlotName(reservationId), Times.Once);
        }

        [TestMethod]
        public void RefreshSlotNameNodeLink_Always_DelegatesToInner()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            sut.RefreshSlotNameNodeLink(reservationId);

            // Assert
            inner.Verify(x => x.RefreshSlotNameNodeLink(reservationId), Times.Once);
        }

        [TestMethod]
        public void RefreshSlotNameNodeLink_WhenInnerThrows_PropagatesException()
        {
            // Arrange
            var reservationId = Guid.Empty;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.RefreshSlotNameNodeLink(reservationId)).Throws(new InvalidOperationException("boom"));
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => sut.RefreshSlotNameNodeLink(reservationId));
            inner.Verify(x => x.RefreshSlotNameNodeLink(reservationId), Times.Once);
        }

        [TestMethod]
        public void GetFirstNodeId_Always_ReturnsInnerValue()
        {
            // Arrange
            var reservationId = Guid.NewGuid();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.GetFirstNodeId(reservationId)).Returns("node-1");
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.GetFirstNodeId(reservationId);

            // Assert
            Assert.AreEqual("node-1", result);
            inner.Verify(x => x.GetFirstNodeId(reservationId), Times.Once);
        }

        [TestMethod]
        public void GetFirstNodeId_WhenInnerReturnsNull_ReturnsNull()
        {
            // Arrange
            var reservationId = Guid.Empty;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.GetFirstNodeId(reservationId)).Returns((string)null);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.GetFirstNodeId(reservationId);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.GetFirstNodeId(reservationId), Times.Once);
        }

        [TestMethod]
        public void Read_WithFilterAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<Reservation>>().Object;
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(filter), Times.Once);
        }

        [TestMethod]
        public void Read_WithFilterAndReadableMiddleware_UsesMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<Reservation>>().Object;
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);

            var middleware = new Mock<IReadableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnRead(filter, It.IsAny<Func<FilterElement<Reservation>, IEnumerable<Reservation>>>()))
                .Returns((FilterElement<Reservation> f, Func<FilterElement<Reservation>, IEnumerable<Reservation>> next) => next(f));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnRead(filter, It.IsAny<Func<FilterElement<Reservation>, IEnumerable<Reservation>>>()), Times.Once);
            inner.Verify(x => x.Read(filter), Times.Once);
        }

        [TestMethod]
        public void Read_WithNullFilterAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read((FilterElement<Reservation>)null)).Returns((IEnumerable<Reservation>)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read((FilterElement<Reservation>)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Read((FilterElement<Reservation>)null), Times.Once);
        }

        [TestMethod]
        public void Read_WithQueryAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Reservation>>().Object;
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(query), Times.Once);
        }

        [TestMethod]
        public void Read_WithQueryAndReadableMiddleware_UsesMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Reservation>>().Object;
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);

            var middleware = new Mock<IReadableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnRead(query, It.IsAny<Func<IQuery<Reservation>, IEnumerable<Reservation>>>()))
                .Returns((IQuery<Reservation> q, Func<IQuery<Reservation>, IEnumerable<Reservation>> next) => next(q));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnRead(query, It.IsAny<Func<IQuery<Reservation>, IEnumerable<Reservation>>>()), Times.Once);
            inner.Verify(x => x.Read(query), Times.Once);
        }

        [TestMethod]
        public void Read_WithNullQueryAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read((IQuery<Reservation>)null)).Returns((IEnumerable<Reservation>)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read((IQuery<Reservation>)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Read((IQuery<Reservation>)null), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged()).Returns(expected);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadPaged();

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithPageSize_DelegatesToInner()
        {
            // Arrange
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(100)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(100);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(100), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithZeroPageSize_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(0)).Returns((IEnumerable<IPagedResult<Reservation>>)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadPaged(0);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.ReadPaged(0), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<Reservation>>().Object;
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageableMiddleware_UsesMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<Reservation>>().Object;
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var pageable = middleware.As<IPageableMiddleware<Reservation>>();
            pageable
                .Setup(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<Reservation>, IEnumerable<IPagedResult<Reservation>>>>()))
                .Returns((FilterElement<Reservation> f, Func<FilterElement<Reservation>, IEnumerable<IPagedResult<Reservation>>> next) => next(f));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<Reservation>, IEnumerable<IPagedResult<Reservation>>>>()), Times.Once);
            inner.Verify(x => x.ReadPaged(filter), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithNullFilterAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged((FilterElement<Reservation>)null)).Returns((IEnumerable<IPagedResult<Reservation>>)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadPaged((FilterElement<Reservation>)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.ReadPaged((FilterElement<Reservation>)null), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Reservation>>().Object;
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageableMiddleware_UsesMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Reservation>>().Object;
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var pageable = middleware.As<IPageableMiddleware<Reservation>>();
            pageable
                .Setup(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<Reservation>, IEnumerable<IPagedResult<Reservation>>>>()))
                .Returns((IQuery<Reservation> q, Func<IQuery<Reservation>, IEnumerable<IPagedResult<Reservation>>> next) => next(q));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<Reservation>, IEnumerable<IPagedResult<Reservation>>>>()), Times.Once);
            inner.Verify(x => x.ReadPaged(query), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithNullQueryAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged((IQuery<Reservation>)null)).Returns((IEnumerable<IPagedResult<Reservation>>)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadPaged((IQuery<Reservation>)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.ReadPaged((IQuery<Reservation>)null), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<Reservation>>().Object;
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(filter, 25)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter, 25);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter, 25), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageSizeAndPageableMiddleware_UsesMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<Reservation>>().Object;
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(filter, 10)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var pageable = middleware.As<IPageableMiddleware<Reservation>>();
            pageable
                .Setup(x => x.OnReadPaged(filter, 10, It.IsAny<Func<FilterElement<Reservation>, int, IEnumerable<IPagedResult<Reservation>>>>()))
                .Returns((FilterElement<Reservation> f, int p, Func<FilterElement<Reservation>, int, IEnumerable<IPagedResult<Reservation>>> next) => next(f, p));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter, 10);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, 10, It.IsAny<Func<FilterElement<Reservation>, int, IEnumerable<IPagedResult<Reservation>>>>()), Times.Once);
            inner.Verify(x => x.ReadPaged(filter, 10), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithNullFilterAndZeroPageSizeAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged((FilterElement<Reservation>)null, 0)).Returns((IEnumerable<IPagedResult<Reservation>>)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadPaged((FilterElement<Reservation>)null, 0);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.ReadPaged((FilterElement<Reservation>)null, 0), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Reservation>>().Object;
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(query, 15)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query, 15);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query, 15), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageSizeAndPageableMiddleware_UsesMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Reservation>>().Object;
            var expected = new List<IPagedResult<Reservation>>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged(query, 5)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var pageable = middleware.As<IPageableMiddleware<Reservation>>();
            pageable
                .Setup(x => x.OnReadPaged(query, 5, It.IsAny<Func<IQuery<Reservation>, int, IEnumerable<IPagedResult<Reservation>>>>()))
                .Returns((IQuery<Reservation> q, int p, Func<IQuery<Reservation>, int, IEnumerable<IPagedResult<Reservation>>> next) => next(q, p));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query, 5);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, 5, It.IsAny<Func<IQuery<Reservation>, int, IEnumerable<IPagedResult<Reservation>>>>()), Times.Once);
            inner.Verify(x => x.ReadPaged(query, 5), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithNullQueryAndNegativePageSizeAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.ReadPaged((IQuery<Reservation>)null, -1)).Returns((IEnumerable<IPagedResult<Reservation>>)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadPaged((IQuery<Reservation>)null, -1);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.ReadPaged((IQuery<Reservation>)null, -1), Times.Once);
        }

        [TestMethod]
        public void Read_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = (Reservation)null;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read(id)).Returns(expected);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read(id);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Read(id), Times.Once);
        }

        [TestMethod]
        public void Read_WithEmptyIdAndMiddleware_ReturnsInnerValue()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read(Guid.Empty)).Returns((Reservation)null);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(Guid.Empty);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Read(Guid.Empty), Times.Once);
        }

        [TestMethod]
        public void Read_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid(), Guid.Empty };
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read(ids)).Returns(expected);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read(ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(ids), Times.Once);
        }

        [TestMethod]
        public void Read_WithNullIdsAndMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Read((IEnumerable<Guid>)null)).Returns((IEnumerable<Reservation>)null);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read((IEnumerable<Guid>)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Read((IEnumerable<Guid>)null), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithNonBulkUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new List<Reservation>();
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Update(items)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update(items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(items), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Update((IEnumerable<Reservation>)null)).Returns((IReadOnlyCollection<Reservation>)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Update((IEnumerable<Reservation>)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Update((IEnumerable<Reservation>)null), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithBulkUpdatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new List<Reservation>();
            var expected = new List<Reservation>();
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Update(items)).Returns(expected);

            var middleware = new Mock<IBulkUpdatableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnUpdate(items, It.IsAny<Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>>>()))
                .Returns((IEnumerable<Reservation> o, Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>> next) => next(o));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update(items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnUpdate(items, It.IsAny<Func<IEnumerable<Reservation>, IReadOnlyCollection<Reservation>>>()), Times.Once);
            inner.Verify(x => x.Update(items), Times.Once);
        }

        [TestMethod]
        public void UpdateSingle_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            Reservation item = null;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Update(item)).Returns((Reservation)null);
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Update(item);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Update(item), Times.Once);
        }

        [TestMethod]
        public void UpdateSingle_WithNonUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            Reservation item = null;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Update(item)).Returns((Reservation)null);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();
            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update(item);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Update(item), Times.Once);
        }

        [TestMethod]
        public void UpdateSingle_WithUpdatableMiddleware_UsesMiddleware()
        {
            // Arrange
            Reservation item = null;
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Update(item)).Returns((Reservation)null);

            var middleware = new Mock<IUpdatableMiddleware<Reservation>>();
            middleware
                .Setup(x => x.OnUpdate(item, It.IsAny<Func<Reservation, Reservation>>()))
                .Returns((Reservation o, Func<Reservation, Reservation> next) => next(o));

            var sut = new TransponderRangeReservationRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update(item);

            // Assert
            Assert.IsNull(result);
            middleware.Verify(x => x.OnUpdate(item, It.IsAny<Func<Reservation, Reservation>>()), Times.Once);
            inner.Verify(x => x.Update(item), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_Always_ReturnsMiddlewareWrappedRepository()
        {
            // Arrange
            var inner = new Mock<ITransponderRangeReservationRepository>();
            inner.Setup(x => x.Count()).Returns(3L);
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();

            // Act
            var result = inner.Object.WithMiddleware(middleware.Object);

            // Assert
            Assert.IsInstanceOfType(result, typeof(TransponderRangeReservationRepositoryMiddleware));
            Assert.AreEqual(3L, result.Count());
            inner.Verify(x => x.Count(), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_WithNullRepository_ThrowsArgumentNullException()
        {
            // Arrange
            ITransponderRangeReservationRepository repository = null;
            var middleware = new Mock<IMiddlewareMarker<Reservation>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => repository.WithMiddleware(middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }
    }
}
