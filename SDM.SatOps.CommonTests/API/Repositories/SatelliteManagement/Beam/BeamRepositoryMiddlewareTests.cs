namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.Beam
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Beam;

    using SLDataGateway.API.Types.Querying;

    [TestClass]
    public class BeamRepositoryMiddlewareTests
    {
        [TestMethod]
        public void Constructor_WhenInnerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var middleware = new Mock<IMiddlewareMarker<Beam>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => new BeamRepositoryMiddleware(null, middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_WhenMiddlewareIsNull_DoesNotThrow()
        {
            // Arrange
            var inner = new Mock<IBeamRepository>();

            // Act
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Assert
            Assert.IsNotNull(repository);
        }

        [TestMethod]
        public void Initialize_Always_DelegatesToInner()
        {
            // Arrange
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Initialize()).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

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
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Count()).Returns(42L);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

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
            FilterElement<Beam> filter = null;
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            FilterElement<Beam> filter = null;
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var countable = middleware.As<ICountableMiddleware<Beam>>();
            countable
                .Setup(x => x.OnCount(filter, It.IsAny<Func<FilterElement<Beam>, long>>()))
                .Returns((FilterElement<Beam> f, Func<FilterElement<Beam>, long> next) => next(f) + 1);

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Count(filter);

            // Assert
            Assert.AreEqual(8L, result);
            countable.Verify(x => x.OnCount(filter, It.IsAny<Func<FilterElement<Beam>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_WithQueryAndNonCountableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Beam>>().Object;
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            var query = new Mock<IQuery<Beam>>().Object;
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Count(query)).Returns(3L);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var countable = middleware.As<ICountableMiddleware<Beam>>();
            countable
                .Setup(x => x.OnCount(query, It.IsAny<Func<IQuery<Beam>, long>>()))
                .Returns((IQuery<Beam> q, Func<IQuery<Beam>, long> next) => next(q) * 2);

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Count(query);

            // Assert
            Assert.AreEqual(6L, result);
            countable.Verify(x => x.OnCount(query, It.IsAny<Func<IQuery<Beam>, long>>()), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithNonBulkCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new List<Beam>();
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Create((IEnumerable<Beam>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(input), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_WithBulkCreatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new List<Beam>();
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var creatable = middleware.As<IBulkCreatableMiddleware<Beam>>();
            creatable
                .Setup(x => x.OnCreate(input, It.IsAny<Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>>>()))
                .Returns((IEnumerable<Beam> o, Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>> next) => next(o));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Create((IEnumerable<Beam>)input);

            // Assert
            Assert.AreSame(expected, result);
            creatable.Verify(x => x.OnCreate(input, It.IsAny<Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>>>()), Times.Once);
            inner.Verify(x => x.Create(input), Times.Once);
        }

        [TestMethod]
        public void Create_WithNonCreatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = Beam.CreateNewBeam();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            var input = Beam.CreateNewBeam();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var creatable = middleware.As<ICreatableMiddleware<Beam>>();
            creatable
                .Setup(x => x.OnCreate(input, It.IsAny<Func<Beam, Beam>>()))
                .Returns((Beam o, Func<Beam, Beam> next) => next(o));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Create(input);

            // Assert
            Assert.AreSame(expected, result);
            creatable.Verify(x => x.OnCreate(input, It.IsAny<Func<Beam, Beam>>()), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_WithNonBulkRepositoryMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new List<Beam>();
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.CreateOrUpdate(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            var input = new List<Beam>();
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.CreateOrUpdate(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var bulk = middleware.As<IBulkRepositoryMiddleware<Beam>>();
            bulk
                .Setup(x => x.OnCreateOrUpdate(input, It.IsAny<Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>>>()))
                .Returns((IEnumerable<Beam> o, Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>> next) => next(o));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.CreateOrUpdate(input);

            // Assert
            Assert.AreSame(expected, result);
            bulk.Verify(x => x.OnCreateOrUpdate(input, It.IsAny<Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>>>()), Times.Once);
        }

        [TestMethod]
        public void Delete_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var inner = new Mock<IBeamRepository>();
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            repository.Delete(id);

            // Assert
            inner.Verify(x => x.Delete(id), Times.Once);
        }

        [TestMethod]
        public void Delete_WithIds_DelegatesToInner()
        {
            // Arrange
            var ids = new List<Guid> { Guid.NewGuid() };
            var inner = new Mock<IBeamRepository>();
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            repository.Delete(ids);

            // Assert
            inner.Verify(x => x.Delete(ids), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithNonBulkDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            repository.Delete((IEnumerable<Beam>)input);

            // Assert
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void DeleteBulk_WithBulkDeletableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new List<Beam>();
            var inner = new Mock<IBeamRepository>();

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var deletable = middleware.As<IBulkDeletableMiddleware<Beam>>();
            deletable
                .Setup(x => x.OnDelete(input, It.IsAny<Action<IEnumerable<Beam>>>()))
                .Callback((IEnumerable<Beam> o, Action<IEnumerable<Beam>> next) => next(o));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            repository.Delete((IEnumerable<Beam>)input);

            // Assert
            deletable.Verify(x => x.OnDelete(input, It.IsAny<Action<IEnumerable<Beam>>>()), Times.Once);
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_WithNonDeletableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            repository.Delete(input);

            // Assert
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_WithDeletableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var deletable = middleware.As<IDeletableMiddleware<Beam>>();
            deletable
                .Setup(x => x.OnDelete(input, It.IsAny<Action<Beam>>()))
                .Callback((Beam o, Action<Beam> next) => next(o));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            repository.Delete(input);

            // Assert
            deletable.Verify(x => x.OnDelete(input, It.IsAny<Action<Beam>>()), Times.Once);
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Read_WithFilterAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            FilterElement<Beam> filter = null;
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            FilterElement<Beam> filter = null;
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var readable = middleware.As<IReadableMiddleware<Beam>>();
            readable
                .Setup(x => x.OnRead(filter, It.IsAny<Func<FilterElement<Beam>, IEnumerable<Beam>>>()))
                .Returns((FilterElement<Beam> f, Func<FilterElement<Beam>, IEnumerable<Beam>> next) => next(f));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Read(filter);

            // Assert
            Assert.AreSame(expected, result);
            readable.Verify(x => x.OnRead(filter, It.IsAny<Func<FilterElement<Beam>, IEnumerable<Beam>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_WithQueryAndNonReadableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Beam>>().Object;
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            var query = new Mock<IQuery<Beam>>().Object;
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var readable = middleware.As<IReadableMiddleware<Beam>>();
            readable
                .Setup(x => x.OnRead(query, It.IsAny<Func<IQuery<Beam>, IEnumerable<Beam>>>()))
                .Returns((IQuery<Beam> q, Func<IQuery<Beam>, IEnumerable<Beam>> next) => next(q));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Read(query);

            // Assert
            Assert.AreSame(expected, result);
            readable.Verify(x => x.OnRead(query, It.IsAny<Func<IQuery<Beam>, IEnumerable<Beam>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithoutArguments_DelegatesToInner()
        {
            // Arrange
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged()).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

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
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(100)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

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
            FilterElement<Beam> filter = null;
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            FilterElement<Beam> filter = null;
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var pageable = middleware.As<IPageableMiddleware<Beam>>();
            pageable
                .Setup(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<Beam>, IEnumerable<IPagedResult<Beam>>>>()))
                .Returns((FilterElement<Beam> f, Func<FilterElement<Beam>, IEnumerable<IPagedResult<Beam>>> next) => next(f));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(filter);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<Beam>, IEnumerable<IPagedResult<Beam>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Beam>>().Object;
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            var query = new Mock<IQuery<Beam>>().Object;
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var pageable = middleware.As<IPageableMiddleware<Beam>>();
            pageable
                .Setup(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<Beam>, IEnumerable<IPagedResult<Beam>>>>()))
                .Returns((IQuery<Beam> q, Func<IQuery<Beam>, IEnumerable<IPagedResult<Beam>>> next) => next(q));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(query);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<Beam>, IEnumerable<IPagedResult<Beam>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            FilterElement<Beam> filter = null;
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(filter, 25)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(filter, 25);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter, 25), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithFilterAndPageSizeAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            FilterElement<Beam> filter = null;
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(filter, 25)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var pageable = middleware.As<IPageableMiddleware<Beam>>();
            pageable
                .Setup(x => x.OnReadPaged(filter, 25, It.IsAny<Func<FilterElement<Beam>, int, IEnumerable<IPagedResult<Beam>>>>()))
                .Returns((FilterElement<Beam> f, int p, Func<FilterElement<Beam>, int, IEnumerable<IPagedResult<Beam>>> next) => next(f, p));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(filter, 25);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, 25, It.IsAny<Func<FilterElement<Beam>, int, IEnumerable<IPagedResult<Beam>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageSizeAndNonPageableMiddleware_DelegatesToInner()
        {
            // Arrange
            var query = new Mock<IQuery<Beam>>().Object;
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(query, 15)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(query, 15);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query, 15), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_WithQueryAndPageSizeAndPageableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var query = new Mock<IQuery<Beam>>().Object;
            var expected = new List<IPagedResult<Beam>>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadPaged(query, 15)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var pageable = middleware.As<IPageableMiddleware<Beam>>();
            pageable
                .Setup(x => x.OnReadPaged(query, 15, It.IsAny<Func<IQuery<Beam>, int, IEnumerable<IPagedResult<Beam>>>>()))
                .Returns((IQuery<Beam> q, int p, Func<IQuery<Beam>, int, IEnumerable<IPagedResult<Beam>>> next) => next(q, p));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.ReadPaged(query, 15);

            // Assert
            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, 15, It.IsAny<Func<IQuery<Beam>, int, IEnumerable<IPagedResult<Beam>>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_WithId_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Read(id)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

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
            var ids = new List<Guid> { Guid.NewGuid() };
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Read(ids)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

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
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Read()).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Read();

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithNonBulkUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = new List<Beam>();
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Update((IEnumerable<Beam>)input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void UpdateBulk_WithBulkUpdatableMiddleware_DelegatesToMiddleware()
        {
            // Arrange
            var input = new List<Beam>();
            var expected = new List<Beam>();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var updatable = middleware.As<IBulkUpdatableMiddleware<Beam>>();
            updatable
                .Setup(x => x.OnUpdate(input, It.IsAny<Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>>>()))
                .Returns((IEnumerable<Beam> o, Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>> next) => next(o));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Update((IEnumerable<Beam>)input);

            // Assert
            Assert.AreSame(expected, result);
            updatable.Verify(x => x.OnUpdate(input, It.IsAny<Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>>>()), Times.Once);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void Update_WithNonUpdatableMiddleware_DelegatesToInner()
        {
            // Arrange
            var input = Beam.CreateNewBeam();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

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
            var input = Beam.CreateNewBeam();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);

            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var updatable = middleware.As<IUpdatableMiddleware<Beam>>();
            updatable
                .Setup(x => x.OnUpdate(input, It.IsAny<Func<Beam, Beam>>()))
                .Returns((Beam o, Func<Beam, Beam> next) => next(o));

            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Update(input);

            // Assert
            Assert.AreSame(expected, result);
            updatable.Verify(x => x.OnUpdate(input, It.IsAny<Func<Beam, Beam>>()), Times.Once);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void Activate_WithGuid_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Activate(id)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Activate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(id), Times.Once);
        }

        [TestMethod]
        public void Activate_WithBeam_DelegatesToInner()
        {
            // Arrange
            var input = Beam.CreateNewBeam();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Activate(input)).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();
            var repository = new BeamRepositoryMiddleware(inner.Object, middleware.Object);

            // Act
            var result = repository.Activate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(input), Times.Once);
        }

        [TestMethod]
        public void Activate_WithBeamCollection_DelegatesToInner()
        {
            // Arrange
            var input = new List<Beam> { Beam.CreateNewBeam() };
            IReadOnlyCollection<Beam> expected = new List<Beam> { Beam.CreateNewBeam() };
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Activate(input)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Activate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(input), Times.Once);
        }

        [TestMethod]
        public void Activate_WithGuidCollection_DelegatesToInner()
        {
            // Arrange
            var input = new List<Guid> { Guid.NewGuid() };
            IReadOnlyCollection<Beam> expected = new List<Beam> { Beam.CreateNewBeam() };
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Activate(input)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Activate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Activate(input), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithGuid_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Deprecate(id)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Deprecate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(id), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithBeam_DelegatesToInner()
        {
            // Arrange
            var input = Beam.CreateNewBeam();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Deprecate(input)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Deprecate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(input), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithBeamCollection_DelegatesToInner()
        {
            // Arrange
            var input = new List<Beam> { Beam.CreateNewBeam() };
            IReadOnlyCollection<Beam> expected = new List<Beam> { Beam.CreateNewBeam() };
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Deprecate(input)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Deprecate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(input), Times.Once);
        }

        [TestMethod]
        public void Deprecate_WithGuidCollection_DelegatesToInner()
        {
            // Arrange
            var input = new List<Guid> { Guid.NewGuid() };
            IReadOnlyCollection<Beam> expected = new List<Beam> { Beam.CreateNewBeam() };
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Deprecate(input)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Deprecate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Deprecate(input), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithGuid_DelegatesToInner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Reactivate(id)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Reactivate(id);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(id), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithBeam_DelegatesToInner()
        {
            // Arrange
            var input = Beam.CreateNewBeam();
            var expected = Beam.CreateNewBeam();
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Reactivate(input)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Reactivate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(input), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithBeamCollection_DelegatesToInner()
        {
            // Arrange
            var input = new List<Beam> { Beam.CreateNewBeam() };
            IReadOnlyCollection<Beam> expected = new List<Beam> { Beam.CreateNewBeam() };
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Reactivate(input)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Reactivate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(input), Times.Once);
        }

        [TestMethod]
        public void Reactivate_WithGuidCollection_DelegatesToInner()
        {
            // Arrange
            var input = new List<Guid> { Guid.NewGuid() };
            IReadOnlyCollection<Beam> expected = new List<Beam> { Beam.CreateNewBeam() };
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.Reactivate(input)).Returns(expected);
            var repository = new BeamRepositoryMiddleware(inner.Object, null);

            // Act
            var result = repository.Reactivate(input);

            // Assert
            Assert.AreSame(expected, result);
            inner.Verify(x => x.Reactivate(input), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_Always_ReturnsWrappingRepositoryDelegatingToInner()
        {
            // Arrange
            var inner = new Mock<IBeamRepository>();
            var expected = Beam.CreateNewBeam();
            inner.Setup(x => x.Initialize()).Returns(expected);
            var middleware = new Mock<IMiddlewareMarker<Beam>>();

            // Act
            var result = inner.Object.WithMiddleware(middleware.Object);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BeamRepositoryMiddleware));
            Assert.AreNotSame(inner.Object, result);
            Assert.AreSame(expected, result.Initialize());
        }

        [TestMethod]
        public void WithMiddleware_WhenRepositoryIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IBeamRepository repository = null;
            var middleware = new Mock<IMiddlewareMarker<Beam>>();

            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentNullException>(() => repository.WithMiddleware(middleware.Object));
            Assert.AreEqual("inner", exception.ParamName);
        }
    }
}
