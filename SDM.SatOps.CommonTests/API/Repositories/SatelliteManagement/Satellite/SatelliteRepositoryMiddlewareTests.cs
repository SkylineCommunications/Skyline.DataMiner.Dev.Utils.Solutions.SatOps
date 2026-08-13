namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.Satellite
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;

    using SLDataGateway.API.Types.Querying;

    [TestClass]
    public class SatelliteRepositoryMiddlewareTests
    {
        [TestMethod]
        public void Constructor_WhenInnerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new SatelliteRepositoryMiddleware(null, middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_WhenMiddlewareIsNull_DoesNotThrow()
        {
            // Arrange
            var inner = new Mock<ISatelliteRepository>();

            // Act
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Assert
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void Initialize_Always_DelegatesToInner()
        {
            // Arrange
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Initialize()).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Initialize();

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Initialize(), Times.Once);
        }

        [TestMethod]
        public void Count_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Count()).Returns(42L);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Count();

            // Assert
            Assert.AreEqual(42L, result);
            inner.Verify(x => x.Count(), Times.Once);
        }

        [TestMethod]
        public void Count_WithFilterAndNonCountableMiddleware_DelegatesToInner()
        {
            // Arrange
            FilterElement<Satellite> filter = null;
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Count(filter);

            // Assert
            Assert.AreEqual(7L, result);
            inner.Verify(x => x.Count(filter), Times.Once);
        }

        [TestMethod]
        public void Count_WithFilterAndCountableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            FilterElement<Satellite> filter = null;
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var countable = middleware.As<ICountableMiddleware<Satellite>>();
            countable
                .Setup(x => x.OnCount(filter, It.IsAny<Func<FilterElement<Satellite>, long>>()))
                .Returns((FilterElement<Satellite> f, Func<FilterElement<Satellite>, long> next) => next(f) + 1);

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Count(filter);

            // Assert
            Assert.AreEqual(8L, result);
            countable.Verify(x => x.OnCount(filter, It.IsAny<Func<FilterElement<Satellite>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithQueryAndNonCountableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Satellite>>().Object;
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Count(query);

            // Assert
            Assert.AreEqual(3L, result);
            inner.Verify(x => x.Count(query), Times.Once);
        }

        [TestMethod]
        public void Count_WithQueryAndCountableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Satellite>>().Object;
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var countable = middleware.As<ICountableMiddleware<Satellite>>();
            countable
                .Setup(x => x.OnCount(query, It.IsAny<Func<IQuery<Satellite>, long>>()))
                .Returns((IQuery<Satellite> q, Func<IQuery<Satellite>, long> next) => next(q) * 2);

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Count(query);

            // Assert
            Assert.AreEqual(6L, result);
            countable.Verify(x => x.OnCount(query, It.IsAny<Func<IQuery<Satellite>, long>>()), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithNonBulkCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Create((IEnumerable<Satellite>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(input), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithBulkCreatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var creatable = middleware.As<IBulkCreatableMiddleware<Satellite>>();
            creatable
                .Setup(x => x.OnCreate(input, It.IsAny<Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>>>()))
                .Returns((IEnumerable<Satellite> o, Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>> next) => next(o));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Create((IEnumerable<Satellite>)input);

            // Assert
            Assert.AreSame(expected, result);
            creatable.Verify(x => x.OnCreate(input, It.IsAny<Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>>>()), Times.Once);
        }

        [TestMethod]
        public void Create_WithNonCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = Satellite.CreateNewSatellite();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Create(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(input), Times.Once);
        }

        [TestMethod]
        public void Create_WithCreatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = Satellite.CreateNewSatellite();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var creatable = middleware.As<ICreatableMiddleware<Satellite>>();
            creatable
                .Setup(x => x.OnCreate(input, It.IsAny<Func<Satellite, Satellite>>()))
                .Returns((Satellite o, Func<Satellite, Satellite> next) => next(o));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Create(input);

            // Assert
            Assert.AreSame(expected, result);
            creatable.Verify(x => x.OnCreate(input, It.IsAny<Func<Satellite, Satellite>>()), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithNonBulkRepositoryMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.CreateOrUpdate(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.CreateOrUpdate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.CreateOrUpdate(input), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithBulkRepositoryMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.CreateOrUpdate(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var bulk = middleware.As<IBulkRepositoryMiddleware<Satellite>>();
            bulk
                .Setup(x => x.OnCreateOrUpdate(input, It.IsAny<Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>>>()))
                .Returns((IEnumerable<Satellite> o, Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>> next) => next(o));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.CreateOrUpdate(input);

            // Assert
            Assert.AreSame(expected, result);
            bulk.Verify(x => x.OnCreateOrUpdate(input, It.IsAny<Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>>>()), Times.Once);
        }

        [TestMethod]
        public void Delete_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var inner = new Mock<ISatelliteRepository>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            repository.Delete(id);

            // Assert
            inner.Verify(x => x.Delete(id), Times.Once);
        }

        [TestMethod]
        public void Delete_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var inner = new Mock<ISatelliteRepository>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            repository.Delete(ids);

            // Assert
            inner.Verify(x => x.Delete(ids), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithNonBulkDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var inner = new Mock<ISatelliteRepository>();
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            repository.Delete((IEnumerable<Satellite>)input);

            // Assert
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithBulkDeletableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var inner = new Mock<ISatelliteRepository>();

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var deletable = middleware.As<IBulkDeletableMiddleware<Satellite>>();
            deletable
                .Setup(x => x.OnDelete(input, It.IsAny<Action<IEnumerable<Satellite>>>()))
                .Callback((IEnumerable<Satellite> o, Action<IEnumerable<Satellite>> next) => next(o));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            repository.Delete((IEnumerable<Satellite>)input);

            // Assert
            deletable.Verify(x => x.OnDelete(input, It.IsAny<Action<IEnumerable<Satellite>>>()), Times.Once);
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_WithNonDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            repository.Delete(input);

            // Assert
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_WithDeletableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var deletable = middleware.As<IDeletableMiddleware<Satellite>>();
            deletable
                .Setup(x => x.OnDelete(input, It.IsAny<Action<Satellite>>()))
                .Callback((Satellite o, Action<Satellite> next) => next(o));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            repository.Delete(input);

            // Assert
            deletable.Verify(x => x.OnDelete(input, It.IsAny<Action<Satellite>>()), Times.Once);
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Read_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Read(id)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Read(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(id), Times.Once);
        }

        [TestMethod]
        public void Read_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid() };
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Read(ids)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Read(ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(ids), Times.Once);
        }

        [TestMethod]
        public void Read_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Read()).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Read();

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(), Times.Once);
        }

        [TestMethod]
        public void Read_WithFilterAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            FilterElement<Satellite> filter = null;
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(filter), Times.Once);
        }

        [TestMethod]
        public void Read_WithFilterAndReadableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            FilterElement<Satellite> filter = null;
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var readable = middleware.As<IReadableMiddleware<Satellite>>();
            readable
                .Setup(x => x.OnRead(filter, It.IsAny<Func<FilterElement<Satellite>, IEnumerable<Satellite>>>()))
                .Returns((FilterElement<Satellite> f, Func<FilterElement<Satellite>, IEnumerable<Satellite>> next) => next(f));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            readable.Verify(x => x.OnRead(filter, It.IsAny<Func<FilterElement<Satellite>, IEnumerable<Satellite>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_WithQueryAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(query), Times.Once);
        }

        [TestMethod]
        public void Read_WithQueryAndReadableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var readable = middleware.As<IReadableMiddleware<Satellite>>();
            readable
                .Setup(x => x.OnRead(query, It.IsAny<Func<IQuery<Satellite>, IEnumerable<Satellite>>>()))
                .Returns((IQuery<Satellite> q, Func<IQuery<Satellite>, IEnumerable<Satellite>> next) => next(q));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            readable.Verify(x => x.OnRead(query, It.IsAny<Func<IQuery<Satellite>, IEnumerable<Satellite>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged()).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.ReadPaged();

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithPageSize_DelegatesToInner()
        {
            // Arrange
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(100)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.ReadPaged(100);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(100), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            FilterElement<Satellite> filter = null;
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            FilterElement<Satellite> filter = null;
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var pageable = middleware.As<IPageableMiddleware<Satellite>>();
            pageable
                .Setup(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<Satellite>, IEnumerable<IPagedResult<Satellite>>>>()))
                .Returns((FilterElement<Satellite> f, Func<FilterElement<Satellite>, IEnumerable<IPagedResult<Satellite>>> next) => next(f));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<Satellite>, IEnumerable<IPagedResult<Satellite>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var pageable = middleware.As<IPageableMiddleware<Satellite>>();
            pageable
                .Setup(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<Satellite>, IEnumerable<IPagedResult<Satellite>>>>()))
                .Returns((IQuery<Satellite> q, Func<IQuery<Satellite>, IEnumerable<IPagedResult<Satellite>>> next) => next(q));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<Satellite>, IEnumerable<IPagedResult<Satellite>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            FilterElement<Satellite> filter = null;
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(filter, 50)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(filter, 50);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter, 50), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterPageSizeAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            FilterElement<Satellite> filter = null;
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(filter, 50)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var pageable = middleware.As<IPageableMiddleware<Satellite>>();
            pageable
                .Setup(x => x.OnReadPaged(filter, 50, It.IsAny<Func<FilterElement<Satellite>, int, IEnumerable<IPagedResult<Satellite>>>>()))
                .Returns((FilterElement<Satellite> f, int p, Func<FilterElement<Satellite>, int, IEnumerable<IPagedResult<Satellite>>> next) => next(f, p));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(filter, 50);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, 50, It.IsAny<Func<FilterElement<Satellite>, int, IEnumerable<IPagedResult<Satellite>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(query, 25)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(query, 25);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query, 25), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryPageSizeAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new List<IPagedResult<Satellite>>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.ReadPaged(query, 25)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var pageable = middleware.As<IPageableMiddleware<Satellite>>();
            pageable
                .Setup(x => x.OnReadPaged(query, 25, It.IsAny<Func<IQuery<Satellite>, int, IEnumerable<IPagedResult<Satellite>>>>()))
                .Returns((IQuery<Satellite> q, int p, Func<IQuery<Satellite>, int, IEnumerable<IPagedResult<Satellite>>> next) => next(q, p));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(query, 25);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, 25, It.IsAny<Func<IQuery<Satellite>, int, IEnumerable<IPagedResult<Satellite>>>>()), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithNonBulkUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Update((IEnumerable<Satellite>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithBulkUpdatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var expected = new List<Satellite>();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var updatable = middleware.As<IBulkUpdatableMiddleware<Satellite>>();
            updatable
                .Setup(x => x.OnUpdate(input, It.IsAny<Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>>>()))
                .Returns((IEnumerable<Satellite> o, Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>> next) => next(o));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Update((IEnumerable<Satellite>)input);

            // Assert
            Assert.AreSame(expected, result);
            updatable.Verify(x => x.OnUpdate(input, It.IsAny<Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>>>()), Times.Once);
        }

        [TestMethod]
        public void Update_WithNonUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = Satellite.CreateNewSatellite();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Update(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void Update_WithUpdatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = Satellite.CreateNewSatellite();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Satellite>>();
            var updatable = middleware.As<IUpdatableMiddleware<Satellite>>();
            updatable
                .Setup(x => x.OnUpdate(input, It.IsAny<Func<Satellite, Satellite>>()))
                .Returns((Satellite o, Func<Satellite, Satellite> next) => next(o));

            var repository = new SatelliteRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Update(input);

            // Assert
            Assert.AreSame(expected, result);
            updatable.Verify(x => x.OnUpdate(input, It.IsAny<Func<Satellite, Satellite>>()), Times.Once);
        }

        [TestMethod]
        public void Activate_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Activate(id)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Activate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(id), Times.Once);
        }

        [TestMethod]
        public void Activate_WithSatellite_DelegatesToInner()
        {
            // Arrange
            var input = Satellite.CreateNewSatellite();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Activate(input)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Activate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(input), Times.Once);
        }

        [TestMethod]
        public void Activate_WithSatelliteCollection_DelegatesToInner()
        {
            // Arrange
            var input = new List<Satellite> { Satellite.CreateNewSatellite() };
            var expected = new List<Satellite> { Satellite.CreateNewSatellite() };
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Activate(input)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Activate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(input), Times.Once);
        }

        [TestMethod]
        public void Activate_WithIdCollection_DelegatesToInner()
        {
            // Arrange
            var input = new List<Guid> { Guid.NewGuid() };
            var expected = new List<Satellite> { Satellite.CreateNewSatellite() };
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Activate(input)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Activate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(input), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Deprecate(id)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Deprecate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(id), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithSingleSatellite_DelegatesToInner()
        {
            // Arrange
            var satellite = Satellite.CreateNewSatellite();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Deprecate(satellite)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Deprecate(satellite);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(satellite), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithSatelliteCollection_DelegatesToInner()
        {
            // Arrange
            var satellites = new List<Satellite> { Satellite.CreateNewSatellite() };
            IReadOnlyCollection<Satellite> expected = new List<Satellite> { Satellite.CreateNewSatellite() };
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Deprecate(satellites)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Deprecate(satellites);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(satellites), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithIdCollection_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            IReadOnlyCollection<Satellite> expected = new List<Satellite> { Satellite.CreateNewSatellite() };
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Deprecate(ids)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Deprecate(ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(ids), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Reactivate(id)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Reactivate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(id), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithSatellite_DelegatesToInner()
        {
            // Arrange
            var satellite = Satellite.CreateNewSatellite();
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Reactivate(satellite)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Reactivate(satellite);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(satellite), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithSatelliteCollection_DelegatesToInner()
        {
            // Arrange
            var input = new[] { Satellite.CreateNewSatellite() };
            var expected = new[] { Satellite.CreateNewSatellite() };
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Reactivate(input)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Reactivate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(input), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithIdCollection_DelegatesToInner()
        {
            // Arrange
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var expected = new[] { Satellite.CreateNewSatellite() };
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Reactivate(ids)).Returns(expected);
            var repository = new SatelliteRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Reactivate(ids);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(ids), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_WithValidRepository_ReturnsWrappingMiddleware()
        {
            // Arrange
            var expected = Satellite.CreateNewSatellite();
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(x => x.Initialize()).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();

            // Act
            var result = inner.Object.WithMiddleware(middleware.Object);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SatelliteRepositoryMiddleware));
            Assert.AreSame(expected, result.Initialize());
            inner.Verify(x => x.Initialize(), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_WithNullRepository_ThrowsArgumentNullException()
        {
            // Arrange
            var middleware = new Mock<IMiddlewareMarker<Satellite>>();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => SatelliteRepositoryExtensions.WithMiddleware(null, middleware.Object));
        }

    }
}
