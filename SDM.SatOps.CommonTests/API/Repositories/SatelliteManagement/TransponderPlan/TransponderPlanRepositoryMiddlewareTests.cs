namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.TransponderPlan
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using TransponderPlanObject = Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan.TransponderPlan;

    [TestClass]
    public class TransponderPlanRepositoryMiddlewareTests
    {
        [TestMethod]
        public void Constructor_WhenInnerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new TransponderPlanRepositoryMiddleware(null, middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_WhenMiddlewareIsNull_DoesNotThrow()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();

            // Act
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void Initialize_Always_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Initialize()).Returns((TransponderPlanObject)null);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

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
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Count()).Returns(42L);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

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
            var filter = new Mock<FilterElement<TransponderPlanObject>>().Object;
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

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
            var filter = new Mock<FilterElement<TransponderPlanObject>>().Object;
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);

            var middleware = new Mock<ICountableMiddleware<TransponderPlanObject>>();
            middleware
                .Setup(x => x.OnCount(filter, It.IsAny<Func<FilterElement<TransponderPlanObject>, long>>()))
                .Returns((FilterElement<TransponderPlanObject> f, Func<FilterElement<TransponderPlanObject>, long> next) => next(f) + 1);

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(filter);

            // Assert
            Assert.AreEqual(8L, result);
            middleware.Verify(x => x.OnCount(filter, It.IsAny<Func<FilterElement<TransponderPlanObject>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithQueryAndNonCountableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanObject>>().Object;
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

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
            var query = new Mock<IQuery<TransponderPlanObject>>().Object;
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);

            var middleware = new Mock<ICountableMiddleware<TransponderPlanObject>>();
            middleware
                .Setup(x => x.OnCount(query, It.IsAny<Func<IQuery<TransponderPlanObject>, long>>()))
                .Returns((IQuery<TransponderPlanObject> q, Func<IQuery<TransponderPlanObject>, long> next) => next(q) * 2);

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Count(query);

            // Assert
            Assert.AreEqual(6L, result);
            middleware.Verify(x => x.OnCount(query, It.IsAny<Func<IQuery<TransponderPlanObject>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithNullFilterAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Count((FilterElement<TransponderPlanObject>)null)).Returns(0L);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Count((FilterElement<TransponderPlanObject>)null);

            // Assert
            Assert.AreEqual(0L, result);
            inner.Verify(x => x.Count((FilterElement<TransponderPlanObject>)null), Times.Once);
        }

        [TestMethod]
        public void Count_WithNullQueryAndNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Count((IQuery<TransponderPlanObject>)null)).Returns(0L);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Count((IQuery<TransponderPlanObject>)null);

            // Assert
            Assert.AreEqual(0L, result);
            inner.Verify(x => x.Count((IQuery<TransponderPlanObject>)null), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithNonBulkCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create((IEnumerable<TransponderPlanObject>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(input), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithBulkCreatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var creatable = middleware.As<IBulkCreatableMiddleware<TransponderPlanObject>>();
            creatable
                .Setup(x => x.OnCreate(input, It.IsAny<Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>>>()))
                .Returns((IEnumerable<TransponderPlanObject> o, Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>> next) => next(o));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create((IEnumerable<TransponderPlanObject>)input);

            // Assert
            Assert.AreSame(expected, result);
            creatable.Verify(x => x.OnCreate(input, It.IsAny<Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>>>()), Times.Once);
        }

        [TestMethod]
        public void Create_WithNonCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Create(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(input), Times.Once);
        }

        [TestMethod]
        public void Create_WithCreatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var creatable = middleware.As<ICreatableMiddleware<TransponderPlanObject>>();
            creatable
                .Setup(x => x.OnCreate(input, It.IsAny<Func<TransponderPlanObject, TransponderPlanObject>>()))
                .Returns((TransponderPlanObject o, Func<TransponderPlanObject, TransponderPlanObject> next) => next(o));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Create(input);

            // Assert
            Assert.AreSame(expected, result);
            creatable.Verify(x => x.OnCreate(input, It.IsAny<Func<TransponderPlanObject, TransponderPlanObject>>()), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithNonBulkRepositoryMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.CreateOrUpdate(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.CreateOrUpdate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.CreateOrUpdate(input), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithBulkRepositoryMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.CreateOrUpdate(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var bulk = middleware.As<IBulkRepositoryMiddleware<TransponderPlanObject>>();
            bulk
                .Setup(x => x.OnCreateOrUpdate(input, It.IsAny<Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>>>()))
                .Returns((IEnumerable<TransponderPlanObject> o, Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>> next) => next(o));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.CreateOrUpdate(input);

            // Assert
            Assert.AreSame(expected, result);
            bulk.Verify(x => x.OnCreateOrUpdate(input, It.IsAny<Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>>>()), Times.Once);
        }

        [TestMethod]
        public void Delete_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var inner = new Mock<ITransponderPlanRepository>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            sut.Delete(id);

            // Assert
            inner.Verify(x => x.Delete(id), Times.Once);
        }

        [TestMethod]
        public void Delete_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var inner = new Mock<ITransponderPlanRepository>();
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<Guid>)ids);

            // Assert
            inner.Verify(x => x.Delete(ids), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithNonBulkDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var inner = new Mock<ITransponderPlanRepository>();
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<TransponderPlanObject>)input);

            // Assert
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithBulkDeletableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var inner = new Mock<ITransponderPlanRepository>();

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var deletable = middleware.As<IBulkDeletableMiddleware<TransponderPlanObject>>();
            deletable
                .Setup(x => x.OnDelete(input, It.IsAny<Action<IEnumerable<TransponderPlanObject>>>()))
                .Callback((IEnumerable<TransponderPlanObject> o, Action<IEnumerable<TransponderPlanObject>> next) => next(o));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete((IEnumerable<TransponderPlanObject>)input);

            // Assert
            deletable.Verify(x => x.OnDelete(input, It.IsAny<Action<IEnumerable<TransponderPlanObject>>>()), Times.Once);
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_WithNonDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            sut.Delete(input);

            // Assert
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_WithDeletableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var deletable = middleware.As<IDeletableMiddleware<TransponderPlanObject>>();
            deletable
                .Setup(x => x.OnDelete(input, It.IsAny<Action<TransponderPlanObject>>()))
                .Callback((TransponderPlanObject o, Action<TransponderPlanObject> next) => next(o));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            sut.Delete(input);

            // Assert
            deletable.Verify(x => x.OnDelete(input, It.IsAny<Action<TransponderPlanObject>>()), Times.Once);
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Read_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Read()).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

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
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadByTransponder(transponderId)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadByTransponder(transponderId);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadByTransponder(transponderId), Times.Once);
        }

        [TestMethod]
        public void Read_WithFilterAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<TransponderPlanObject>>().Object;
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(filter), Times.Once);
        }

        [TestMethod]
        public void Read_WithFilterAndReadableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<TransponderPlanObject>>().Object;
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var readable = middleware.As<IReadableMiddleware<TransponderPlanObject>>();
            readable
                .Setup(x => x.OnRead(filter, It.IsAny<Func<FilterElement<TransponderPlanObject>, IEnumerable<TransponderPlanObject>>>()))
                .Returns((FilterElement<TransponderPlanObject> f, Func<FilterElement<TransponderPlanObject>, IEnumerable<TransponderPlanObject>> next) => next(f));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            readable.Verify(x => x.OnRead(filter, It.IsAny<Func<FilterElement<TransponderPlanObject>, IEnumerable<TransponderPlanObject>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_WithQueryAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanObject>>().Object;
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(query), Times.Once);
        }

        [TestMethod]
        public void Read_WithQueryAndReadableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanObject>>().Object;
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var readable = middleware.As<IReadableMiddleware<TransponderPlanObject>>();
            readable
                .Setup(x => x.OnRead(query, It.IsAny<Func<IQuery<TransponderPlanObject>, IEnumerable<TransponderPlanObject>>>()))
                .Returns((IQuery<TransponderPlanObject> q, Func<IQuery<TransponderPlanObject>, IEnumerable<TransponderPlanObject>> next) => next(q));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            readable.Verify(x => x.OnRead(query, It.IsAny<Func<IQuery<TransponderPlanObject>, IEnumerable<TransponderPlanObject>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged()).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

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
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(100)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

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
            var filter = new Mock<FilterElement<TransponderPlanObject>>().Object;
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<TransponderPlanObject>>().Object;
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var pageable = middleware.As<IPageableMiddleware<TransponderPlanObject>>();
            pageable
                .Setup(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<TransponderPlanObject>, IEnumerable<IPagedResult<TransponderPlanObject>>>>()))
                .Returns((FilterElement<TransponderPlanObject> f, Func<FilterElement<TransponderPlanObject>, IEnumerable<IPagedResult<TransponderPlanObject>>> next) => next(f));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<TransponderPlanObject>, IEnumerable<IPagedResult<TransponderPlanObject>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanObject>>().Object;
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanObject>>().Object;
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var pageable = middleware.As<IPageableMiddleware<TransponderPlanObject>>();
            pageable
                .Setup(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<TransponderPlanObject>, IEnumerable<IPagedResult<TransponderPlanObject>>>>()))
                .Returns((IQuery<TransponderPlanObject> q, Func<IQuery<TransponderPlanObject>, IEnumerable<IPagedResult<TransponderPlanObject>>> next) => next(q));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<TransponderPlanObject>, IEnumerable<IPagedResult<TransponderPlanObject>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var filter = new Mock<FilterElement<TransponderPlanObject>>().Object;
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(filter, 50)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter, 50);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter, 50), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterPageSizeAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var filter = new Mock<FilterElement<TransponderPlanObject>>().Object;
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(filter, 25)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var pageable = middleware.As<IPageableMiddleware<TransponderPlanObject>>();
            pageable
                .Setup(x => x.OnReadPaged(filter, 25, It.IsAny<Func<FilterElement<TransponderPlanObject>, int, IEnumerable<IPagedResult<TransponderPlanObject>>>>()))
                .Returns((FilterElement<TransponderPlanObject> f, int p, Func<FilterElement<TransponderPlanObject>, int, IEnumerable<IPagedResult<TransponderPlanObject>>> next) => next(f, p));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(filter, 25);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, 25, It.IsAny<Func<FilterElement<TransponderPlanObject>, int, IEnumerable<IPagedResult<TransponderPlanObject>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanObject>>().Object;
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(query, 10)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.ReadPaged(query, 10);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query, 10), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryPageSizeAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<TransponderPlanObject>>().Object;
            var expected = new List<IPagedResult<TransponderPlanObject>>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadPaged(query, 5)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var pageable = middleware.As<IPageableMiddleware<TransponderPlanObject>>();
            pageable
                .Setup(x => x.OnReadPaged(query, 5, It.IsAny<Func<IQuery<TransponderPlanObject>, int, IEnumerable<IPagedResult<TransponderPlanObject>>>>()))
                .Returns((IQuery<TransponderPlanObject> q, int p, Func<IQuery<TransponderPlanObject>, int, IEnumerable<IPagedResult<TransponderPlanObject>>> next) => next(q, p));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.ReadPaged(query, 5);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, 5, It.IsAny<Func<IQuery<TransponderPlanObject>, int, IEnumerable<IPagedResult<TransponderPlanObject>>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Read(id)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

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
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Read(ids)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

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
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update((IEnumerable<TransponderPlanObject>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithBulkUpdatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var updatable = middleware.As<IBulkUpdatableMiddleware<TransponderPlanObject>>();
            updatable
                .Setup(x => x.OnUpdate(input, It.IsAny<Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>>>()))
                .Returns((IEnumerable<TransponderPlanObject> o, Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>> next) => next(o));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update((IEnumerable<TransponderPlanObject>)input);

            // Assert
            Assert.AreSame(expected, result);
            updatable.Verify(x => x.OnUpdate(input, It.IsAny<Func<IEnumerable<TransponderPlanObject>, IReadOnlyCollection<TransponderPlanObject>>>()), Times.Once);
        }

        [TestMethod]
        public void Update_WithNonUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void Update_WithNullMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Update(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void Update_WithUpdatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var updatable = middleware.As<IUpdatableMiddleware<TransponderPlanObject>>();
            updatable
                .Setup(x => x.OnUpdate(input, It.IsAny<Func<TransponderPlanObject, TransponderPlanObject>>()))
                .Returns((TransponderPlanObject o, Func<TransponderPlanObject, TransponderPlanObject> next) => next(o));

            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Update(input);

            // Assert
            Assert.AreSame(expected, result);
            updatable.Verify(x => x.OnUpdate(input, It.IsAny<Func<TransponderPlanObject, TransponderPlanObject>>()), Times.Once);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void Activate_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Activate(id)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Activate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(id), Times.Once);
        }

        [TestMethod]
        public void Activate_WithObject_DelegatesToInner()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Activate(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Activate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(input), Times.Once);
        }

        [TestMethod]
        public void Activate_WithObjects_DelegatesToInner()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Activate(input)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Activate((IEnumerable<TransponderPlanObject>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(input), Times.Once);
        }

        [TestMethod]
        public void Activate_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Activate(ids)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Activate((IEnumerable<Guid>)ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(ids), Times.Once);
        }

        [TestMethod]
        public void Activate_WithNullObject_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Activate((TransponderPlanObject)null)).Returns((TransponderPlanObject)null);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Activate((TransponderPlanObject)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Activate((TransponderPlanObject)null), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Deprecate(id)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Deprecate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(id), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithObject_DelegatesToInner()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Deprecate(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Deprecate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(input), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithNullObject_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Deprecate((TransponderPlanObject)null)).Returns((TransponderPlanObject)null);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Deprecate((TransponderPlanObject)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Deprecate((TransponderPlanObject)null), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithObjects_DelegatesToInner()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Deprecate(input)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Deprecate((IEnumerable<TransponderPlanObject>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(input), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Deprecate(ids)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Deprecate((IEnumerable<Guid>)ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(ids), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Reactivate(id)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Reactivate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(id), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithEmptyId_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Reactivate(Guid.Empty)).Returns((TransponderPlanObject)null);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Reactivate(Guid.Empty);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Reactivate(Guid.Empty), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithObject_DelegatesToInner()
        {
            // Arrange
            var input = TransponderPlanObject.CreateNewTransponderPlan();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Reactivate(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Reactivate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(input), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithNullObject_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Reactivate((TransponderPlanObject)null)).Returns((TransponderPlanObject)null);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Reactivate((TransponderPlanObject)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Reactivate((TransponderPlanObject)null), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithObjects_DelegatesToInner()
        {
            // Arrange
            var input = new[] { TransponderPlanObject.CreateNewTransponderPlan() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Reactivate(input)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Reactivate((IEnumerable<TransponderPlanObject>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(input), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithEmptyObjects_DelegatesToInner()
        {
            // Arrange
            var input = new TransponderPlanObject[0];
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Reactivate(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = sut.Reactivate((IEnumerable<TransponderPlanObject>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(input), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var expected = new List<TransponderPlanObject>();
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Reactivate(ids)).Returns(expected);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Reactivate((IEnumerable<Guid>)ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(ids), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithNullIds_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.Reactivate((IEnumerable<Guid>)null)).Returns((IReadOnlyCollection<TransponderPlanObject>)null);
            var sut = new TransponderPlanRepositoryMiddleware(inner.Object, null);

            // Act
            var result = sut.Reactivate((IEnumerable<Guid>)null);

            // Assert
            Assert.IsNull(result);
            inner.Verify(x => x.Reactivate((IEnumerable<Guid>)null), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_WithRepositoryAndMiddleware_ReturnsWrappingMiddleware()
        {
            // Arrange
            var inner = new Mock<ITransponderPlanRepository>();
            var expected = TransponderPlanObject.CreateNewTransponderPlan();
            inner.Setup(x => x.Initialize()).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();

            // Act
            var result = inner.Object.WithMiddleware(middleware.Object);

            // Assert
            Assert.IsInstanceOfType(result, typeof(TransponderPlanRepositoryMiddleware));
            Assert.AreSame(expected, result.Initialize());
            inner.Verify(x => x.Initialize(), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_WithNullRepository_ThrowsArgumentNullException()
        {
            // Arrange
            var middleware = new Mock<IMiddlewareMarker<TransponderPlanObject>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => TransponderPlanRepositoryExtensions.WithMiddleware(null, middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }
    }
}
