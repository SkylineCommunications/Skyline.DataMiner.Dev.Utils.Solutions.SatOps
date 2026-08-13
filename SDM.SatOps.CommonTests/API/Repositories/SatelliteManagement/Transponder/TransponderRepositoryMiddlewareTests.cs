namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.Transponder
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class TransponderRepositoryMiddlewareTests
    {
        [TestMethod]
        public void Constructor_WhenInnerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var middleware = new Mock<IMiddlewareMarker<Transponder>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new TransponderRepositoryMiddleware(null, middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_WhenMiddlewareIsNull_DoesNotThrow()
        {
            // Arrange
            var inner = new Mock<ITransponderRepository>();

            // Act
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void Initialize_Always_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Initialize()).Returns((Transponder)null);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Initialize();

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Initialize(), Times.Once);
        }

        [TestMethod]
        public void Count_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Count()).Returns(42L);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

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
            var filter = new Mock<FilterElement<Transponder>>().Object;
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);
            var middleware = new Mock<IMiddlewareMarker<Transponder>>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

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
            var filter = new Mock<FilterElement<Transponder>>().Object;
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);

            var middleware = new Mock<ICountableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnCount(filter, It.IsAny<Func<FilterElement<Transponder>, long>>()))
                .Returns((FilterElement<Transponder> f, Func<FilterElement<Transponder>, long> next) => next(f) + 1);

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(filter);

            // Assert
            Assert.AreEqual(8L, result);
            middleware.Verify(x => x.OnCount(filter, It.IsAny<Func<FilterElement<Transponder>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithQueryAndNonCountableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Transponder>>().Object;
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);
            var middleware = new Mock<IMiddlewareMarker<Transponder>>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

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
            var query = new Mock<IQuery<Transponder>>().Object;
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);

            var middleware = new Mock<ICountableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnCount(query, It.IsAny<Func<IQuery<Transponder>, long>>()))
                .Returns((IQuery<Transponder> q, Func<IQuery<Transponder>, long> next) => next(q) * 2);

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(query);

            // Assert
            Assert.AreEqual(6L, result);
            middleware.Verify(x => x.OnCount(query, It.IsAny<Func<IQuery<Transponder>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithNullQueryAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Count((IQuery<Transponder>)null)).Returns(0L);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Count((IQuery<Transponder>)null);

            // Assert
            Assert.AreEqual(0L, result);
            inner.Verify(x => x.Count((IQuery<Transponder>)null), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithNonCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            var expected = new List<Transponder> { Transponder.CreateNewTransponder() };
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Create(items)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Create((IEnumerable<Transponder>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(items), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithBulkCreatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder> { Transponder.CreateNewTransponder() };
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Create(items)).Returns(expected);

            var middleware = new Mock<IBulkCreatableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnCreate(items, It.IsAny<Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>>>()))
                .Returns((IEnumerable<Transponder> i, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next) => next(i));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create((IEnumerable<Transponder>)items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnCreate(items, It.IsAny<Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>>>()), Times.Once);
        }

        [TestMethod]
        public void CreateSingle_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Create(item)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Create(item);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(item), Times.Once);
        }

        [TestMethod]
        public void CreateSingle_WithCreatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Create(item)).Returns(expected);

            var middleware = new Mock<ICreatableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnCreate(item, It.IsAny<Func<Transponder, Transponder>>()))
                .Returns((Transponder i, Func<Transponder, Transponder> next) => next(i));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create(item);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnCreate(item, It.IsAny<Func<Transponder, Transponder>>()), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithNonBulkRepositoryMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.CreateOrUpdate(items)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

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
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.CreateOrUpdate(items)).Returns(expected);

            var middleware = new Mock<IBulkRepositoryMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnCreateOrUpdate(items, It.IsAny<Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>>>()))
                .Returns((IEnumerable<Transponder> i, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next) => next(i));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.CreateOrUpdate(items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnCreateOrUpdate(items, It.IsAny<Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>>>()), Times.Once);
        }

        [TestMethod]
        public void DeleteById_WithNonDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var inner = new Mock<ITransponderRepository>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            sut.Delete(id);

            // Assert
            inner.Verify(x => x.Delete(id), Times.Once);
            inner.Verify(x => x.Read(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void DeleteById_WithDeletableMiddlewareAndExistingObject_UsesMiddleware()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(id)).Returns(existing);

            var middleware = new Mock<IDeletableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnDelete(existing, It.IsAny<Action<Transponder>>()))
                .Callback((Transponder t, Action<Transponder> next) => next(t));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(id);

            // Assert
            middleware.Verify(x => x.OnDelete(existing, It.IsAny<Action<Transponder>>()), Times.Once);
            inner.Verify(x => x.Delete(existing), Times.Once);
            inner.Verify(x => x.Delete(id), Times.Never);
        }

        [TestMethod]
        public void DeleteById_WithDeletableMiddlewareAndMissingObject_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(id)).Returns((Transponder)null);
            var middleware = new Mock<IDeletableMiddleware<Transponder>>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(id);

            // Assert
            inner.Verify(x => x.Delete(id), Times.Once);
            middleware.Verify(x => x.OnDelete(It.IsAny<Transponder>(), It.IsAny<Action<Transponder>>()), Times.Never);
        }

        [TestMethod]
        public void DeleteByIds_WithNonBulkDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            var inner = new Mock<ITransponderRepository>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            sut.Delete(ids);

            // Assert
            inner.Verify(x => x.Delete(ids), Times.Once);
            inner.Verify(x => x.Read(It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [TestMethod]
        public void DeleteByIds_WithBulkDeletableMiddlewareAndExistingObjects_UsesMiddleware()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            var existing = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(ids)).Returns(new[] { existing });

            var middleware = new Mock<IBulkDeletableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnDelete(It.IsAny<IEnumerable<Transponder>>(), It.IsAny<Action<IEnumerable<Transponder>>>()))
                .Callback((IEnumerable<Transponder> t, Action<IEnumerable<Transponder>> next) => next(t));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(ids);

            // Assert
            middleware.Verify(x => x.OnDelete(It.IsAny<IEnumerable<Transponder>>(), It.IsAny<Action<IEnumerable<Transponder>>>()), Times.Once);
            inner.Verify(x => x.Delete(It.Is<IEnumerable<Transponder>>(c => c.Contains(existing))), Times.Once);
            inner.Verify(x => x.Delete(ids), Times.Never);
        }

        [TestMethod]
        public void DeleteByIds_WithBulkDeletableMiddlewareAndNoExistingObjects_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(ids)).Returns(new Transponder[0]);
            var middleware = new Mock<IBulkDeletableMiddleware<Transponder>>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(ids);

            // Assert
            inner.Verify(x => x.Delete(ids), Times.Once);
            middleware.Verify(x => x.OnDelete(It.IsAny<IEnumerable<Transponder>>(), It.IsAny<Action<IEnumerable<Transponder>>>()), Times.Never);
        }

        [TestMethod]
        public void DeleteObjects_WithNonBulkDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            var inner = new Mock<ITransponderRepository>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            sut.Delete((IEnumerable<Transponder>)items);

            // Assert
            inner.Verify(x => x.Delete(items), Times.Once);
        }

        [TestMethod]
        public void DeleteObjects_WithBulkDeletableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            var inner = new Mock<ITransponderRepository>();

            var middleware = new Mock<IBulkDeletableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnDelete(items, It.IsAny<Action<IEnumerable<Transponder>>>()))
                .Callback((IEnumerable<Transponder> t, Action<IEnumerable<Transponder>> next) => next(t));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<Transponder>)items);

            // Assert
            middleware.Verify(x => x.OnDelete(items, It.IsAny<Action<IEnumerable<Transponder>>>()), Times.Once);
            inner.Verify(x => x.Delete(items), Times.Once);
        }

        [TestMethod]
        public void DeleteObject_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            sut.Delete(item);

            // Assert
            inner.Verify(x => x.Delete(item), Times.Once);
        }

        [TestMethod]
        public void DeleteObject_WithDeletableMiddleware_UsesMiddleware()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();

            var middleware = new Mock<IDeletableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnDelete(item, It.IsAny<Action<Transponder>>()))
                .Callback((Transponder t, Action<Transponder> next) => next(t));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(item);

            // Assert
            middleware.Verify(x => x.OnDelete(item, It.IsAny<Action<Transponder>>()), Times.Once);
            inner.Verify(x => x.Delete(item), Times.Once);
        }

        [TestMethod]
        public void ReadBySatellite_Always_DelegatesToInner()
        {
            // Arrange
            var satelliteId = Guid.NewGuid();
            var expected = new List<Transponder> { Transponder.CreateNewTransponder() };
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadBySatellite(satelliteId)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.ReadBySatellite(satelliteId);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadBySatellite(satelliteId), Times.Once);
        }

        [TestMethod]
        public void ReadWithFilter_WithNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<Transponder>>().Object;
            var expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(filter), Times.Once);
        }

        [TestMethod]
        public void ReadWithFilter_WithReadableMiddleware_UsesMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<Transponder>>().Object;
            IEnumerable<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);

            var middleware = new Mock<IReadableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnRead(filter, It.IsAny<Func<FilterElement<Transponder>, IEnumerable<Transponder>>>()))
                .Returns((FilterElement<Transponder> f, Func<FilterElement<Transponder>, IEnumerable<Transponder>> next) => next(f));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnRead(filter, It.IsAny<Func<FilterElement<Transponder>, IEnumerable<Transponder>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadWithQuery_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Transponder>>().Object;
            var expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(query), Times.Once);
        }

        [TestMethod]
        public void ReadWithQuery_WithReadableMiddleware_UsesMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Transponder>>().Object;
            IEnumerable<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);

            var middleware = new Mock<IReadableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnRead(query, It.IsAny<Func<IQuery<Transponder>, IEnumerable<Transponder>>>()))
                .Returns((IQuery<Transponder> q, Func<IQuery<Transponder>, IEnumerable<Transponder>> next) => next(q));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnRead(query, It.IsAny<Func<IQuery<Transponder>, IEnumerable<Transponder>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged()).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

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
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(100)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadPaged(100);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(100), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<Transponder>>().Object;
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Transponder>>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

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
            var filter = new Mock<FilterElement<Transponder>>().Object;
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);

            var middleware = new Mock<IPageableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<Transponder>, IEnumerable<IPagedResult<Transponder>>>>()))
                .Returns((FilterElement<Transponder> f, Func<FilterElement<Transponder>, IEnumerable<IPagedResult<Transponder>>> next) => next(f));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<Transponder>, IEnumerable<IPagedResult<Transponder>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Transponder>>().Object;
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

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
            var query = new Mock<IQuery<Transponder>>().Object;
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);

            var middleware = new Mock<IPageableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<Transponder>, IEnumerable<IPagedResult<Transponder>>>>()))
                .Returns((IQuery<Transponder> q, Func<IQuery<Transponder>, IEnumerable<IPagedResult<Transponder>>> next) => next(q));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<Transponder>, IEnumerable<IPagedResult<Transponder>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<Transponder>>().Object;
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(filter, 10)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Transponder>>();
            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter, 10);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter, 10), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageSizeAndPageableMiddleware_UsesMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<Transponder>>().Object;
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(filter, 25)).Returns(expected);

            var middleware = new Mock<IPageableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnReadPaged(filter, 25, It.IsAny<Func<FilterElement<Transponder>, int, IEnumerable<IPagedResult<Transponder>>>>()))
                .Returns((FilterElement<Transponder> f, int p, Func<FilterElement<Transponder>, int, IEnumerable<IPagedResult<Transponder>>> next) => next(f, p));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter, 25);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnReadPaged(filter, 25, It.IsAny<Func<FilterElement<Transponder>, int, IEnumerable<IPagedResult<Transponder>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Transponder>>().Object;
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(query, 15)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

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
            var query = new Mock<IQuery<Transponder>>().Object;
            IEnumerable<IPagedResult<Transponder>> expected = new List<IPagedResult<Transponder>>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.ReadPaged(query, 15)).Returns(expected);

            var middleware = new Mock<IPageableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnReadPaged(query, 15, It.IsAny<Func<IQuery<Transponder>, int, IEnumerable<IPagedResult<Transponder>>>>()))
                .Returns((IQuery<Transponder> q, int p, Func<IQuery<Transponder>, int, IEnumerable<IPagedResult<Transponder>>> next) => next(q, p));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query, 15);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnReadPaged(query, 15, It.IsAny<Func<IQuery<Transponder>, int, IEnumerable<IPagedResult<Transponder>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadById_Always_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(id)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(id), Times.Once);
        }

        [TestMethod]
        public void ReadByIds_Always_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            IEnumerable<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read(ids)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Read(ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(ids), Times.Once);
        }

        [TestMethod]
        public void ReadAll_Always_DelegatesToInner()
        {
            // Arrange
            IEnumerable<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Read()).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read();

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithNonBulkUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Update(items)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Update((IEnumerable<Transponder>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(items), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithBulkUpdatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Update(items)).Returns(expected);

            var middleware = new Mock<IBulkUpdatableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnUpdate(items, It.IsAny<Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>>>()))
                .Returns((IEnumerable<Transponder> i, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next) => next(i));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update((IEnumerable<Transponder>)items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnUpdate(items, It.IsAny<Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>>>()), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new List<Transponder>();
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Update(items)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Update((IEnumerable<Transponder>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(items), Times.Once);
        }

        [TestMethod]
        public void UpdateSingle_WithNonUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Update(item)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Update(item);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(item), Times.Once);
        }

        [TestMethod]
        public void UpdateSingle_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Update(item)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Update(item);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(item), Times.Once);
        }

        [TestMethod]
        public void UpdateSingle_WithUpdatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Update(item)).Returns(expected);

            var middleware = new Mock<IUpdatableMiddleware<Transponder>>();
            middleware
                .Setup(x => x.OnUpdate(item, It.IsAny<Func<Transponder, Transponder>>()))
                .Returns((Transponder i, Func<Transponder, Transponder> next) => next(i));

            var sut = new TransponderRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update(item);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnUpdate(item, It.IsAny<Func<Transponder, Transponder>>()), Times.Once);
            inner.Verify(x => x.Update(item), Times.Once);
        }

        [TestMethod]
        public void ActivateById_Always_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Activate(id)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Activate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(id), Times.Once);
        }

        [TestMethod]
        public void ActivateObject_Always_DelegatesToInner()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Activate(item)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Activate(item);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(item), Times.Once);
        }

        [TestMethod]
        public void ActivateObjects_Always_DelegatesToInner()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Activate(items)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Activate((IEnumerable<Transponder>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(items), Times.Once);
        }

        [TestMethod]
        public void ActivateByIds_Always_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Activate(ids)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Activate((IEnumerable<Guid>)ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(ids), Times.Once);
        }

        [TestMethod]
        public void DeprecateById_Always_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Deprecate(id)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Deprecate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(id), Times.Once);
        }

        [TestMethod]
        public void DeprecateObject_Always_DelegatesToInner()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Deprecate(item)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Deprecate(item);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(item), Times.Once);
        }

        [TestMethod]
        public void DeprecateObjects_Always_DelegatesToInner()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Deprecate(items)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Deprecate((IEnumerable<Transponder>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(items), Times.Once);
        }

        [TestMethod]
        public void DeprecateByIds_Always_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Deprecate(ids)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Deprecate((IEnumerable<Guid>)ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(ids), Times.Once);
        }

        [TestMethod]
        public void ReactivateById_Always_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Reactivate(id)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Reactivate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(id), Times.Once);
        }

        [TestMethod]
        public void ReactivateObject_Always_DelegatesToInner()
        {
            // Arrange
            var item = Transponder.CreateNewTransponder();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Reactivate(item)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Reactivate(item);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(item), Times.Once);
        }

        [TestMethod]
        public void ReactivateObjects_Always_DelegatesToInner()
        {
            // Arrange
            var items = new List<Transponder> { Transponder.CreateNewTransponder() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Reactivate(items)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Reactivate((IEnumerable<Transponder>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(items), Times.Once);
        }

        [TestMethod]
        public void ReactivateByIds_Always_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            IReadOnlyCollection<Transponder> expected = new List<Transponder>();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Reactivate(ids)).Returns(expected);
            var sut = new TransponderRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<Transponder>>().Object);

            // Act
            var result = sut.Reactivate((IEnumerable<Guid>)ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(ids), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_Always_ReturnsWrappingMiddlewareRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Transponder.CreateNewTransponder();
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(x => x.Reactivate(id)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Transponder>>().Object;

            // Act
            var result = inner.Object.WithMiddleware(middleware);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TransponderRepositoryMiddleware));
            Assert.AreSame(expected, result.Reactivate(id));
        }

        [TestMethod]
        public void WithMiddleware_WhenRepositoryIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ITransponderRepository repository = null;

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => repository.WithMiddleware(null));
            Assert.AreEqual("inner", exception.ParamName);
        }
    }
}
