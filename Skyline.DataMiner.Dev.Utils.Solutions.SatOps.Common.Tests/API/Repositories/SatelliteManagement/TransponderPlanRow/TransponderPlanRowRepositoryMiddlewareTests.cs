namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.TransponderPlanRow
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class TransponderPlanRowRepositoryMiddlewareTests
    {
        [TestMethod]
        public void Constructor_WhenInnerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new TransponderPlanRowRepositoryMiddleware(null, middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_WhenMiddlewareIsNull_DoesNotThrow()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRowRepository>();

            // Act
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void Count_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Count()).Returns(42L);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

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
            var filter = new Mock<FilterElement<TransponderPlanRow>>().Object;
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(filter);

            // Assert
            Assert.AreEqual(7L, result);
            inner.Verify(x => x.Count(filter), Times.Once);
        }

        [TestMethod]
        public void Count_WithFilterAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Count((FilterElement<TransponderPlanRow>)null)).Returns(0L);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Count((FilterElement<TransponderPlanRow>)null);

            // Assert
            Assert.AreEqual(0L, result);
            inner.Verify(x => x.Count((FilterElement<TransponderPlanRow>)null), Times.Once);
        }

        [TestMethod]
        public void Count_WithFilterAndCountableMiddleware_UsesMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<TransponderPlanRow>>().Object;
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);

            var middleware = new Mock<ICountableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnCount(filter, It.IsAny<Func<FilterElement<TransponderPlanRow>, long>>()))
                .Returns((FilterElement<TransponderPlanRow> f, Func<FilterElement<TransponderPlanRow>, long> next) => next(f) + 1);

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(filter);

            // Assert
            Assert.AreEqual(8L, result);
            middleware.Verify(x => x.OnCount(filter, It.IsAny<Func<FilterElement<TransponderPlanRow>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithQueryAndNonCountableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanRow>>().Object;
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

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
            var query = new Mock<IQuery<TransponderPlanRow>>().Object;
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);

            var middleware = new Mock<ICountableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnCount(query, It.IsAny<Func<IQuery<TransponderPlanRow>, long>>()))
                .Returns((IQuery<TransponderPlanRow> q, Func<IQuery<TransponderPlanRow>, long> next) => next(q) * 2);

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(query);

            // Assert
            Assert.AreEqual(6L, result);
            middleware.Verify(x => x.OnCount(query, It.IsAny<Func<IQuery<TransponderPlanRow>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithNullQueryAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Count((IQuery<TransponderPlanRow>)null)).Returns(0L);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Count((IQuery<TransponderPlanRow>)null);

            // Assert
            Assert.AreEqual(0L, result);
            inner.Verify(x => x.Count((IQuery<TransponderPlanRow>)null), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithNonBulkCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new[] { new TransponderPlanRow() };
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Create(items)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create((IEnumerable<TransponderPlanRow>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(items), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithBulkCreatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new[] { new TransponderPlanRow() };
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Create(items)).Returns(expected);

            var middleware = new Mock<IBulkCreatableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnCreate(items, It.IsAny<Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>>>()))
                .Returns((IEnumerable<TransponderPlanRow> o, Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>> next) => next(o));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create((IEnumerable<TransponderPlanRow>)items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnCreate(items, It.IsAny<Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>>>()), Times.Once);
        }

        [TestMethod]
        public void Create_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var item = new TransponderPlanRow();
            var expected = new TransponderPlanRow();
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Create(item)).Returns(expected);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Create(item);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(item), Times.Once);
        }

        [TestMethod]
        public void Create_WithCreatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var item = new TransponderPlanRow();
            var expected = new TransponderPlanRow();
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Create(item)).Returns(expected);

            var middleware = new Mock<ICreatableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnCreate(item, It.IsAny<Func<TransponderPlanRow, TransponderPlanRow>>()))
                .Returns((TransponderPlanRow o, Func<TransponderPlanRow, TransponderPlanRow> next) => next(o));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create(item);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnCreate(item, It.IsAny<Func<TransponderPlanRow, TransponderPlanRow>>()), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithNonBulkRepositoryMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new[] { new TransponderPlanRow() };
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.CreateOrUpdate(items)).Returns(expected);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

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
            var items = new[] { new TransponderPlanRow() };
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.CreateOrUpdate(items)).Returns(expected);

            var middleware = new Mock<IBulkRepositoryMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnCreateOrUpdate(items, It.IsAny<Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>>>()))
                .Returns((IEnumerable<TransponderPlanRow> o, Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>> next) => next(o));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.CreateOrUpdate(items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnCreateOrUpdate(items, It.IsAny<Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>>>()), Times.Once);
        }

        [TestMethod]
        public void Delete_WithGuid_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var inner = new Mock<ITransponderPlanRowRepository>();
            var middleware = new Mock<IBulkDeletableMiddleware<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(id);

            // Assert
            inner.Verify(x => x.Delete(id), Times.Once);
        }

        [TestMethod]
        public void Delete_WithGuidCollection_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Act
            sut.Delete((IEnumerable<Guid>)ids);

            // Assert
            inner.Verify(x => x.Delete(ids), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithNonBulkDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<TransponderPlanRow>)items);

            // Assert
            inner.Verify(x => x.Delete(items), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithBulkDeletableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();

            var middleware = new Mock<IBulkDeletableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnDelete(items, It.IsAny<Action<IEnumerable<TransponderPlanRow>>>()))
                .Callback((IEnumerable<TransponderPlanRow> o, Action<IEnumerable<TransponderPlanRow>> next) => next(o));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<TransponderPlanRow>)items);

            // Assert
            middleware.Verify(x => x.OnDelete(items, It.IsAny<Action<IEnumerable<TransponderPlanRow>>>()), Times.Once);
            inner.Verify(x => x.Delete(items), Times.Once);
        }

        [TestMethod]
        public void Delete_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var item = new TransponderPlanRow();
            var inner = new Mock<ITransponderPlanRowRepository>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Act
            sut.Delete(item);

            // Assert
            inner.Verify(x => x.Delete(item), Times.Once);
        }

        [TestMethod]
        public void Delete_WithDeletableMiddleware_UsesMiddleware()
        {
            // Arrange
            var item = new TransponderPlanRow();
            var inner = new Mock<ITransponderPlanRowRepository>();

            var middleware = new Mock<IDeletableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnDelete(item, It.IsAny<Action<TransponderPlanRow>>()))
                .Callback((TransponderPlanRow o, Action<TransponderPlanRow> next) => next(o));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(item);

            // Assert
            middleware.Verify(x => x.OnDelete(item, It.IsAny<Action<TransponderPlanRow>>()), Times.Once);
            inner.Verify(x => x.Delete(item), Times.Once);
        }

        [TestMethod]
        public void Read_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Read()).Returns(expected);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read();

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(), Times.Once);
        }

        [TestMethod]
        public void Read_WithFilterAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<TransponderPlanRow>>().Object;
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

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
            var filter = new Mock<FilterElement<TransponderPlanRow>>().Object;
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);

            var middleware = new Mock<IReadableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnRead(filter, It.IsAny<Func<FilterElement<TransponderPlanRow>, IEnumerable<TransponderPlanRow>>>()))
                .Returns((FilterElement<TransponderPlanRow> f, Func<FilterElement<TransponderPlanRow>, IEnumerable<TransponderPlanRow>> next) => next(f));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnRead(filter, It.IsAny<Func<FilterElement<TransponderPlanRow>, IEnumerable<TransponderPlanRow>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_WithQueryAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanRow>>().Object;
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

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
            var query = new Mock<IQuery<TransponderPlanRow>>().Object;
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);

            var middleware = new Mock<IReadableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnRead(query, It.IsAny<Func<IQuery<TransponderPlanRow>, IEnumerable<TransponderPlanRow>>>()))
                .Returns((IQuery<TransponderPlanRow> q, Func<IQuery<TransponderPlanRow>, IEnumerable<TransponderPlanRow>> next) => next(q));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnRead(query, It.IsAny<Func<IQuery<TransponderPlanRow>, IEnumerable<TransponderPlanRow>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged()).Returns(expected);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

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
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(100)).Returns(expected);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

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
            var filter = new Mock<FilterElement<TransponderPlanRow>>().Object;
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

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
            var filter = new Mock<FilterElement<TransponderPlanRow>>().Object;
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);

            var middleware = new Mock<IPageableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<TransponderPlanRow>, IEnumerable<IPagedResult<TransponderPlanRow>>>>()))
                .Returns((FilterElement<TransponderPlanRow> f, Func<FilterElement<TransponderPlanRow>, IEnumerable<IPagedResult<TransponderPlanRow>>> next) => next(f));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<TransponderPlanRow>, IEnumerable<IPagedResult<TransponderPlanRow>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanRow>>().Object;
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

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
            var query = new Mock<IQuery<TransponderPlanRow>>().Object;
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);

            var middleware = new Mock<IPageableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<TransponderPlanRow>, IEnumerable<IPagedResult<TransponderPlanRow>>>>()))
                .Returns((IQuery<TransponderPlanRow> q, Func<IQuery<TransponderPlanRow>, IEnumerable<IPagedResult<TransponderPlanRow>>> next) => next(q));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<TransponderPlanRow>, IEnumerable<IPagedResult<TransponderPlanRow>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<TransponderPlanRow>>().Object;
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(filter, 10)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

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
            var filter = new Mock<FilterElement<TransponderPlanRow>>().Object;
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(filter, 25)).Returns(expected);

            var middleware = new Mock<IPageableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnReadPaged(filter, 25, It.IsAny<Func<FilterElement<TransponderPlanRow>, int, IEnumerable<IPagedResult<TransponderPlanRow>>>>()))
                .Returns((FilterElement<TransponderPlanRow> f, int p, Func<FilterElement<TransponderPlanRow>, int, IEnumerable<IPagedResult<TransponderPlanRow>>> next) => next(f, p));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter, 25);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnReadPaged(filter, 25, It.IsAny<Func<FilterElement<TransponderPlanRow>, int, IEnumerable<IPagedResult<TransponderPlanRow>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanRow>>().Object;
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(query, 10)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query, 10);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query, 10), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageSizeAndPageableMiddleware_UsesMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanRow>>().Object;
            var expected = new IPagedResult<TransponderPlanRow>[0];
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadPaged(query, 25)).Returns(expected);

            var middleware = new Mock<IPageableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnReadPaged(query, 25, It.IsAny<Func<IQuery<TransponderPlanRow>, int, IEnumerable<IPagedResult<TransponderPlanRow>>>>()))
                .Returns((IQuery<TransponderPlanRow> q, int p, Func<IQuery<TransponderPlanRow>, int, IEnumerable<IPagedResult<TransponderPlanRow>>> next) => next(q, p));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query, 25);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnReadPaged(query, 25, It.IsAny<Func<IQuery<TransponderPlanRow>, int, IEnumerable<IPagedResult<TransponderPlanRow>>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new TransponderPlanRow();
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Read(id)).Returns(expected);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Read(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(id), Times.Once);
        }

        [TestMethod]
        public void Read_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Read(ids)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read((IEnumerable<Guid>)ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(ids), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithNonBulkUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var items = new[] { new TransponderPlanRow() };
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Update(items)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update((IEnumerable<TransponderPlanRow>)items);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(items), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithBulkUpdatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var items = new[] { new TransponderPlanRow() };
            var expected = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Update(items)).Returns(expected);

            var middleware = new Mock<IBulkUpdatableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnUpdate(items, It.IsAny<Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>>>()))
                .Returns((IEnumerable<TransponderPlanRow> o, Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>> next) => next(o));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update((IEnumerable<TransponderPlanRow>)items);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnUpdate(items, It.IsAny<Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>>>()), Times.Once);
        }

        [TestMethod]
        public void Update_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var item = new TransponderPlanRow();
            var expected = new TransponderPlanRow();
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Update(item)).Returns(expected);
            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Update(item);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(item), Times.Once);
        }

        [TestMethod]
        public void Update_WithUpdatableMiddleware_UsesMiddleware()
        {
            // Arrange
            var item = new TransponderPlanRow();
            var expected = new TransponderPlanRow();
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Update(item)).Returns(expected);

            var middleware = new Mock<IUpdatableMiddleware<TransponderPlanRow>>();
            middleware
                .Setup(x => x.OnUpdate(item, It.IsAny<Func<TransponderPlanRow, TransponderPlanRow>>()))
                .Returns((TransponderPlanRow o, Func<TransponderPlanRow, TransponderPlanRow> next) => next(o));

            var sut = new TransponderPlanRowRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update(item);

            // Assert
            Assert.AreSame(expected, result);
            middleware.Verify(x => x.OnUpdate(item, It.IsAny<Func<TransponderPlanRow, TransponderPlanRow>>()), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_WithValidRepository_ReturnsMiddlewareWrapper()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRowRepository>();
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();

            // Act
            var result = inner.Object.WithMiddleware(middleware.Object);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(TransponderPlanRowRepositoryMiddleware));
            Assert.AreNotSame(inner.Object, result);
        }

        [TestMethod]
        public void WithMiddleware_WithNullMiddleware_ReturnsWrapperDelegatingToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.Count()).Returns(7L);

            // Act
            var result = inner.Object.WithMiddleware(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(TransponderPlanRowRepositoryMiddleware));
            Assert.AreEqual(7L, result.Count());
            inner.Verify(x => x.Count(), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_WithNullRepository_ThrowsArgumentNullException()
        {
            // Arrange
            ITransponderPlanRowRepository inner = null;
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanRow>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => inner.WithMiddleware(middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }

    }
}
