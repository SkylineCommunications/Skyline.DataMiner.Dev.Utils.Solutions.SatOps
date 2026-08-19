namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.TransponderSlot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;
    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>TransponderSlotRepositoryMiddleware</c> delegation behavior.
    /// </summary>
    [TestClass]
    public class TransponderSlotRepositoryMiddlewareTests
    {
        [TestMethod]
        public void Constructor_NullInner_ThrowsArgumentNullException()
        {
            var middleware = new Mock<IMiddlewareMarker<TransponderSlot>>();

            Assert.ThrowsException<ArgumentNullException>(() => new TransponderSlotRepositoryMiddleware(null, middleware.Object));
        }

        [TestMethod]
        public void Constructor_NullMiddleware_DoesNotThrow()
        {
            var inner = new Mock<ITransponderSlotRepository>();

            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void Count_NoArguments_DelegatesToInner()
        {
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Count()).Returns(42L);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.Count();

            Assert.AreEqual(42L, result);
            inner.Verify(x => x.Count(), Times.Once);
        }

        [TestMethod]
        public void Count_FilterWithNonCountableMiddleware_DelegatesToInner()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Count(filter)).Returns(7L);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.Count(filter);

            Assert.AreEqual(7L, result);
            inner.Verify(x => x.Count(filter), Times.Once);
        }

        [TestMethod]
        public void Count_FilterWithNullMiddleware_DelegatesToInner()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Count(filter)).Returns(3L);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            var result = sut.Count(filter);

            Assert.AreEqual(3L, result);
        }

        [TestMethod]
        public void Count_FilterWithCountableMiddleware_InvokesMiddleware()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Count(filter)).Returns(11L);
            var countable = new Mock<ICountableMiddleware<TransponderSlot>>();
            countable.Setup(x => x.OnCount(filter, It.IsAny<Func<FilterElement<TransponderSlot>, long>>()))
                     .Returns((FilterElement<TransponderSlot> f, Func<FilterElement<TransponderSlot>, long> next) => next(f) + 1);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, countable.Object);

            var result = sut.Count(filter);

            Assert.AreEqual(12L, result);
            countable.Verify(x => x.OnCount(filter, It.IsAny<Func<FilterElement<TransponderSlot>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Count_QueryWithNonCountableMiddleware_DelegatesToInner()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Count(query)).Returns(5L);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.Count(query);

            Assert.AreEqual(5L, result);
            inner.Verify(x => x.Count(query), Times.Once);
        }

        [TestMethod]
        public void Count_QueryWithCountableMiddleware_InvokesMiddleware()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Count(query)).Returns(20L);
            var countable = new Mock<ICountableMiddleware<TransponderSlot>>();
            countable.Setup(x => x.OnCount(query, It.IsAny<Func<IQuery<TransponderSlot>, long>>()))
                     .Returns((IQuery<TransponderSlot> q, Func<IQuery<TransponderSlot>, long> next) => next(q) * 2);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, countable.Object);

            var result = sut.Count(query);

            Assert.AreEqual(40L, result);
            countable.Verify(x => x.OnCount(query, It.IsAny<Func<IQuery<TransponderSlot>, long>>()), Times.Once);
        }

        [TestMethod]
        public void Create_BulkWithNonCreatableMiddleware_DelegatesToInner()
        {
            var input = new[] { new TransponderSlot() };
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.Create((IEnumerable<TransponderSlot>)input);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(input), Times.Once);
        }

        [TestMethod]
        public void Create_BulkWithBulkCreatableMiddleware_InvokesMiddleware()
        {
            var input = new[] { new TransponderSlot() };
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var bulk = new Mock<IBulkCreatableMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnCreate(input, It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()))
                .Returns((IEnumerable<TransponderSlot> o, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next) => next(o));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            var result = sut.Create((IEnumerable<TransponderSlot>)input);

            Assert.AreSame(expected, result);
            bulk.Verify(x => x.OnCreate(input, It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()), Times.Once);
        }

        [TestMethod]
        public void Create_SingleWithNonCreatableMiddleware_DelegatesToInner()
        {
            var input = new TransponderSlot();
            var expected = new TransponderSlot();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            var result = sut.Create(input);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Create(input), Times.Once);
        }

        [TestMethod]
        public void Create_SingleWithCreatableMiddleware_InvokesMiddleware()
        {
            var input = new TransponderSlot();
            var expected = new TransponderSlot();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Create(input)).Returns(expected);
            var creatable = new Mock<ICreatableMiddleware<TransponderSlot>>();
            creatable.Setup(x => x.OnCreate(input, It.IsAny<Func<TransponderSlot, TransponderSlot>>()))
                     .Returns((TransponderSlot o, Func<TransponderSlot, TransponderSlot> next) => next(o));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, creatable.Object);

            var result = sut.Create(input);

            Assert.AreSame(expected, result);
            creatable.Verify(x => x.OnCreate(input, It.IsAny<Func<TransponderSlot, TransponderSlot>>()), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_NonBulkRepositoryMiddleware_DelegatesToInner()
        {
            var input = new[] { new TransponderSlot() };
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.CreateOrUpdate(input)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.CreateOrUpdate(input);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.CreateOrUpdate(input), Times.Once);
        }

        [TestMethod]
        public void CreateOrUpdate_BulkRepositoryMiddleware_InvokesMiddleware()
        {
            var input = new[] { new TransponderSlot() };
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.CreateOrUpdate(input)).Returns(expected);
            var bulk = new Mock<IBulkRepositoryMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnCreateOrUpdate(input, It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()))
                .Returns((IEnumerable<TransponderSlot> o, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next) => next(o));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            var result = sut.CreateOrUpdate(input);

            Assert.AreSame(expected, result);
            bulk.Verify(x => x.OnCreateOrUpdate(input, It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()), Times.Once);
        }

        [TestMethod]
        public void Delete_ById_DelegatesToInner()
        {
            var id = Guid.NewGuid();
            var inner = new Mock<ITransponderSlotRepository>();
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            sut.Delete(id);

            inner.Verify(x => x.Delete(id), Times.Once);
        }

        [TestMethod]
        public void Delete_ByIds_DelegatesToInner()
        {
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var inner = new Mock<ITransponderSlotRepository>();
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            sut.Delete(ids);

            inner.Verify(x => x.Delete(ids), Times.Once);
        }

        [TestMethod]
        public void Delete_BulkWithNonDeletableMiddleware_DelegatesToInner()
        {
            var input = new[] { new TransponderSlot() };
            var inner = new Mock<ITransponderSlotRepository>();
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            sut.Delete((IEnumerable<TransponderSlot>)input);

            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_BulkWithBulkDeletableMiddleware_InvokesMiddlewareAndSkipsDirectInnerCall()
        {
            var input = new[] { new TransponderSlot() };
            var inner = new Mock<ITransponderSlotRepository>();
            var bulk = new Mock<IBulkDeletableMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnDelete(input, It.IsAny<Action<IEnumerable<TransponderSlot>>>()))
                .Callback((IEnumerable<TransponderSlot> o, Action<IEnumerable<TransponderSlot>> next) => next(o));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            sut.Delete((IEnumerable<TransponderSlot>)input);

            bulk.Verify(x => x.OnDelete(input, It.IsAny<Action<IEnumerable<TransponderSlot>>>()), Times.Once);
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_BulkWithBulkDeletableMiddlewareNotCallingNext_DoesNotCallInner()
        {
            var input = new[] { new TransponderSlot() };
            var inner = new Mock<ITransponderSlotRepository>();
            var bulk = new Mock<IBulkDeletableMiddleware<TransponderSlot>>();
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            sut.Delete((IEnumerable<TransponderSlot>)input);

            inner.Verify(x => x.Delete(It.IsAny<IEnumerable<TransponderSlot>>()), Times.Never);
        }

        [TestMethod]
        public void Delete_SingleWithNullMiddleware_DelegatesToInner()
        {
            var input = new TransponderSlot();
            var inner = new Mock<ITransponderSlotRepository>();
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            sut.Delete(input);

            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Delete_SingleWithDeletableMiddleware_InvokesMiddleware()
        {
            var input = new TransponderSlot();
            var inner = new Mock<ITransponderSlotRepository>();
            var deletable = new Mock<IDeletableMiddleware<TransponderSlot>>();
            deletable.Setup(x => x.OnDelete(input, It.IsAny<Action<TransponderSlot>>()))
                     .Callback((TransponderSlot o, Action<TransponderSlot> next) => next(o));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, deletable.Object);

            sut.Delete(input);

            deletable.Verify(x => x.OnDelete(input, It.IsAny<Action<TransponderSlot>>()), Times.Once);
            inner.Verify(x => x.Delete(input), Times.Once);
        }

        [TestMethod]
        public void Read_NoArguments_DelegatesToInner()
        {
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Read()).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.Read();

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(), Times.Once);
        }

        [TestMethod]
        public void Read_FilterWithNonReadableMiddleware_DelegatesToInner()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.Read(filter);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(filter), Times.Once);
        }

        [TestMethod]
        public void Read_FilterWithReadableMiddleware_InvokesMiddleware()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Read(filter)).Returns(expected);
            var readable = new Mock<IReadableMiddleware<TransponderSlot>>();
            readable.Setup(x => x.OnRead(filter, It.IsAny<Func<FilterElement<TransponderSlot>, IEnumerable<TransponderSlot>>>()))
                    .Returns((FilterElement<TransponderSlot> f, Func<FilterElement<TransponderSlot>, IEnumerable<TransponderSlot>> next) => next(f));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, readable.Object);

            var result = sut.Read(filter);

            Assert.AreSame(expected, result);
            readable.Verify(x => x.OnRead(filter, It.IsAny<Func<FilterElement<TransponderSlot>, IEnumerable<TransponderSlot>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_QueryWithNullMiddleware_DelegatesToInner()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            var result = sut.Read(query);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(query), Times.Once);
        }

        [TestMethod]
        public void Read_QueryWithReadableMiddleware_InvokesMiddleware()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Read(query)).Returns(expected);
            var readable = new Mock<IReadableMiddleware<TransponderSlot>>();
            readable.Setup(x => x.OnRead(query, It.IsAny<Func<IQuery<TransponderSlot>, IEnumerable<TransponderSlot>>>()))
                    .Returns((IQuery<TransponderSlot> q, Func<IQuery<TransponderSlot>, IEnumerable<TransponderSlot>> next) => next(q));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, readable.Object);

            var result = sut.Read(query);

            Assert.AreSame(expected, result);
            readable.Verify(x => x.OnRead(query, It.IsAny<Func<IQuery<TransponderSlot>, IEnumerable<TransponderSlot>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_NoArguments_DelegatesToInner()
        {
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged()).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IPageableMiddleware<TransponderSlot>>().Object);

            var result = sut.ReadPaged();

            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_PageSize_DelegatesToInner()
        {
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(25)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            var result = sut.ReadPaged(25);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(25), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_FilterWithNonPageableMiddleware_DelegatesToInner()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.ReadPaged(filter);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_FilterWithPageableMiddleware_InvokesMiddleware()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(filter)).Returns(expected);
            var pageable = new Mock<IPageableMiddleware<TransponderSlot>>();
            pageable.Setup(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<TransponderSlot>, IEnumerable<IPagedResult<TransponderSlot>>>>()))
                    .Returns((FilterElement<TransponderSlot> f, Func<FilterElement<TransponderSlot>, IEnumerable<IPagedResult<TransponderSlot>>> next) => next(f));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, pageable.Object);

            var result = sut.ReadPaged(filter);

            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, It.IsAny<Func<FilterElement<TransponderSlot>, IEnumerable<IPagedResult<TransponderSlot>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_QueryWithNullMiddleware_DelegatesToInner()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            var result = sut.ReadPaged(query);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_QueryWithPageableMiddleware_InvokesMiddleware()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(query)).Returns(expected);
            var pageable = new Mock<IPageableMiddleware<TransponderSlot>>();
            pageable.Setup(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<TransponderSlot>, IEnumerable<IPagedResult<TransponderSlot>>>>()))
                    .Returns((IQuery<TransponderSlot> q, Func<IQuery<TransponderSlot>, IEnumerable<IPagedResult<TransponderSlot>>> next) => next(q));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, pageable.Object);

            var result = sut.ReadPaged(query);

            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, It.IsAny<Func<IQuery<TransponderSlot>, IEnumerable<IPagedResult<TransponderSlot>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_FilterAndPageSizeWithNonPageableMiddleware_DelegatesToInner()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(filter, 10)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.ReadPaged(filter, 10);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(filter, 10), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_FilterAndPageSizeWithPageableMiddleware_InvokesMiddleware()
        {
            var filter = new Mock<FilterElement<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(filter, 10)).Returns(expected);
            var pageable = new Mock<IPageableMiddleware<TransponderSlot>>();
            pageable.Setup(x => x.OnReadPaged(filter, 10, It.IsAny<Func<FilterElement<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>>>()))
                    .Returns((FilterElement<TransponderSlot> f, int p, Func<FilterElement<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>> next) => next(f, p));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, pageable.Object);

            var result = sut.ReadPaged(filter, 10);

            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(filter, 10, It.IsAny<Func<FilterElement<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>>>()), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_QueryAndPageSizeWithNonPageableMiddleware_DelegatesToInner()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(query, 15)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.ReadPaged(query, 15);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadPaged(query, 15), Times.Once);
        }

        [TestMethod]
        public void ReadPaged_QueryAndPageSizeWithNullMiddleware_DelegatesToInner()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(query, 5)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            var result = sut.ReadPaged(query, 5);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void ReadPaged_QueryAndPageSizeWithPageableMiddleware_InvokesMiddleware()
        {
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new List<IPagedResult<TransponderSlot>>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadPaged(query, 20)).Returns(expected);
            var pageable = new Mock<IPageableMiddleware<TransponderSlot>>();
            pageable.Setup(x => x.OnReadPaged(query, 20, It.IsAny<Func<IQuery<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>>>()))
                    .Returns((IQuery<TransponderSlot> q, int p, Func<IQuery<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>> next) => next(q, p));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, pageable.Object);

            var result = sut.ReadPaged(query, 20);

            Assert.AreSame(expected, result);
            pageable.Verify(x => x.OnReadPaged(query, 20, It.IsAny<Func<IQuery<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>>>()), Times.Once);
        }

        [TestMethod]
        public void Read_ById_DelegatesToInner()
        {
            var id = Guid.NewGuid();
            var expected = new TransponderSlot();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Read(id)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.Read(id);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(id), Times.Once);
        }

        [TestMethod]
        public void Read_ByIds_DelegatesToInner()
        {
            var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Read(ids)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            var result = sut.Read((IEnumerable<Guid>)ids);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Read(ids), Times.Once);
        }

        [TestMethod]
        public void Update_BulkWithNonUpdatableMiddleware_DelegatesToInner()
        {
            var input = new[] { new TransponderSlot() };
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.Update((IEnumerable<TransponderSlot>)input);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void Update_BulkWithBulkUpdatableMiddleware_InvokesMiddleware()
        {
            var input = new[] { new TransponderSlot() };
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var bulk = new Mock<IBulkUpdatableMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnUpdate(input, It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()))
                .Returns((IEnumerable<TransponderSlot> o, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next) => next(o));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            var result = sut.Update((IEnumerable<TransponderSlot>)input);

            Assert.AreSame(expected, result);
            bulk.Verify(x => x.OnUpdate(input, It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()), Times.Once);
        }

        [TestMethod]
        public void Update_SingleWithNullMiddleware_DelegatesToInner()
        {
            var input = new TransponderSlot();
            var expected = new TransponderSlot();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            var result = sut.Update(input);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.Update(input), Times.Once);
        }

        [TestMethod]
        public void Update_SingleWithUpdatableMiddleware_InvokesMiddleware()
        {
            var input = new TransponderSlot();
            var expected = new TransponderSlot();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Update(input)).Returns(expected);
            var updatable = new Mock<IUpdatableMiddleware<TransponderSlot>>();
            updatable.Setup(x => x.OnUpdate(input, It.IsAny<Func<TransponderSlot, TransponderSlot>>()))
                     .Returns((TransponderSlot o, Func<TransponderSlot, TransponderSlot> next) => next(o));
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, updatable.Object);

            var result = sut.Update(input);

            Assert.AreSame(expected, result);
            updatable.Verify(x => x.OnUpdate(input, It.IsAny<Func<TransponderSlot, TransponderSlot>>()), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_GenerationRequest_ExpandsIntoTheCalculatedSlots()
        {
            var planId = Guid.NewGuid();
            var built = new List<TransponderSlot> { CreateSlot("A36", planId), CreateSlot("B36", planId) };

            var inner = new Mock<ITransponderSlotRepository>();
            inner.As<ITransponderSlotBuilder>().Setup(x => x.BuildSlots(planId)).Returns(built);

            IEnumerable<TransponderSlot> validated = null;
            var bulk = new Mock<IBulkCreatableMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnCreate(It.IsAny<IEnumerable<TransponderSlot>>(), It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()))
                .Callback<IEnumerable<TransponderSlot>, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>((s, _) => validated = s)
                .Returns(built);

            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            var result = sut.Create(new[] { CreateGenerationRequest(planId) });

            Assert.AreSame(built, result);
            CollectionAssert.AreEqual(built, validated.ToList());
            inner.Verify(x => x.DeleteSlotsByTransponderPlan(planId), Times.Once);
        }

        [TestMethod]
        public void CreateBulk_GenerationRequest_BuildsBeforeDeletingAndValidating()
        {
            var planId = Guid.NewGuid();
            var built = new List<TransponderSlot> { CreateSlot("A36", planId) };
            var sequence = new List<string>();

            var inner = new Mock<ITransponderSlotRepository>();
            inner.As<ITransponderSlotBuilder>().Setup(x => x.BuildSlots(planId)).Returns(built).Callback(() => sequence.Add("build"));
            inner.Setup(x => x.DeleteSlotsByTransponderPlan(planId)).Callback(() => sequence.Add("delete"));

            var bulk = new Mock<IBulkCreatableMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnCreate(It.IsAny<IEnumerable<TransponderSlot>>(), It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()))
                .Returns(built)
                .Callback(() => sequence.Add("validate"));

            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            sut.Create(new[] { CreateGenerationRequest(planId) });

            CollectionAssert.AreEqual(new[] { "build", "delete", "validate" }, sequence);
        }

        [TestMethod]
        public void CreateBulk_RegularSlots_AreNotExpanded()
        {
            var planId = Guid.NewGuid();
            var slots = new[] { CreateSlot("A36", planId) };

            var inner = new Mock<ITransponderSlotRepository>();
            var builder = inner.As<ITransponderSlotBuilder>();

            var bulk = new Mock<IBulkCreatableMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnCreate(It.IsAny<IEnumerable<TransponderSlot>>(), It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()))
                .Returns(slots);

            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            sut.Create(slots);

            builder.Verify(x => x.BuildSlots(It.IsAny<Guid>()), Times.Never);
            inner.Verify(x => x.DeleteSlotsByTransponderPlan(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void CreateBulk_SameGenerationRequestPlanTwice_ExpandsOnlyOnce()
        {
            var planId = Guid.NewGuid();
            var built = new List<TransponderSlot> { CreateSlot("A36", planId) };

            var inner = new Mock<ITransponderSlotRepository>();
            var builder = inner.As<ITransponderSlotBuilder>();
            builder.Setup(x => x.BuildSlots(planId)).Returns(built);

            var bulk = new Mock<IBulkCreatableMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnCreate(It.IsAny<IEnumerable<TransponderSlot>>(), It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()))
                .Returns(built);

            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            sut.Create(new[] { CreateGenerationRequest(planId), CreateGenerationRequest(planId) });

            builder.Verify(x => x.BuildSlots(planId), Times.Once);
            inner.Verify(x => x.DeleteSlotsByTransponderPlan(planId), Times.Once);
        }

        [TestMethod]
        public void CreateSingle_GenerationRequest_ExpandsAndReturnsFirstCreatedSlot()
        {
            var planId = Guid.NewGuid();
            var first = CreateSlot("A36", planId);
            var built = new List<TransponderSlot> { first, CreateSlot("B36", planId) };

            var inner = new Mock<ITransponderSlotRepository>();
            inner.As<ITransponderSlotBuilder>().Setup(x => x.BuildSlots(planId)).Returns(built);

            var bulk = new Mock<IBulkCreatableMiddleware<TransponderSlot>>();
            bulk.Setup(x => x.OnCreate(It.IsAny<IEnumerable<TransponderSlot>>(), It.IsAny<Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>>()))
                .Returns(built);

            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, bulk.Object);

            var result = sut.Create(CreateGenerationRequest(planId));

            Assert.AreSame(first, result);
            inner.Verify(x => x.DeleteSlotsByTransponderPlan(planId), Times.Once);
        }

        private static TransponderSlot CreateGenerationRequest(Guid planId)
        {
            var slot = new TransponderSlot();
            slot.TransponderPlan = planId;

            return slot;
        }

        private static TransponderSlot CreateSlot(string name, Guid planId)
        {
            var slot = new TransponderSlot();
            slot.TransponderPlan = planId;
            slot.Name = name;
            slot.SlotStartFrequency = 0d;
            slot.SlotEndFrequency = 36d;

            return slot;
        }

        [TestMethod]
        public void ReadByTransponderPlan_DelegatesToInner()
        {
            var planId = Guid.NewGuid();
            var expected = new List<TransponderSlot>();
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadByTransponderPlan(planId)).Returns(expected);
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, new Mock<IMiddlewareMarker<TransponderSlot>>().Object);

            var result = sut.ReadByTransponderPlan(planId);

            Assert.AreSame(expected, result);
            inner.Verify(x => x.ReadByTransponderPlan(planId), Times.Once);
        }

        [TestMethod]
        public void DeleteSlotsByTransponderPlan_DelegatesToInner()
        {
            var planId = Guid.NewGuid();
            var inner = new Mock<ITransponderSlotRepository>();
            var sut = new TransponderSlotRepositoryMiddleware(inner.Object, null);

            sut.DeleteSlotsByTransponderPlan(planId);

            inner.Verify(x => x.DeleteSlotsByTransponderPlan(planId), Times.Once);
        }

        [TestMethod]
        public void WithMiddleware_ValidArguments_ReturnsWrappingMiddleware()
        {
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.Count()).Returns(42L);
            var marker = new Mock<IMiddlewareMarker<TransponderSlot>>().Object;

            var result = inner.Object.WithMiddleware(marker);

            Assert.IsNotNull(result);
            Assert.AreNotSame(inner.Object, result);
            Assert.AreEqual(42L, result.Count());
        }

        [TestMethod]
        public void WithMiddleware_NullRepository_ThrowsArgumentNullException()
        {
            ITransponderSlotRepository repository = null;

            Assert.ThrowsException<ArgumentNullException>(() => repository.WithMiddleware(null));
        }
    }
}
