namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>TransponderPlanRowValidationMiddleware</c>.
    /// </summary>
    [TestClass]
    public class TransponderPlanRowValidationMiddlewareTests
    {
        private static readonly Guid PlanId = Guid.NewGuid();

        [TestMethod]
        public void Constructor_NullPlanResolver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderPlanRowValidationMiddleware(null));
        }

        [TestMethod]
        public void Constructor_NullRowsResolver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderPlanRowValidationMiddleware(_ => TransponderPlan.CreateNewTransponderPlan(), null));
        }

        [TestMethod]
        public void OnCreateSingle_NullRow_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((TransponderPlanRow)null, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(CreateValidRow(), (Func<TransponderPlanRow, TransponderPlanRow>)null));
        }

        [TestMethod]
        public void OnCreateSingle_ValidRow_CallsNext()
        {
            var sut = CreateSut();
            var row = CreateValidRow();
            var called = false;

            var result = sut.OnCreate(row, r =>
            {
                called = true;
                return r;
            });

            Assert.IsTrue(called);
            Assert.AreSame(row, result);
        }

        [TestMethod]
        public void OnCreateSingle_MissingBandwidth_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var row = CreateValidRow();
            row.Bandwidth = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(row, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_UnknownPlan_ThrowsArgumentException()
        {
            var sut = new TransponderPlanRowValidationMiddleware(_ => null);
            var row = CreateValidRow();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(row, r => r));
        }

        [TestMethod]
        public void OnCreateBatch_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<TransponderPlanRow>)null, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateBatch_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(
                new[] { CreateValidRow() }.AsEnumerable(),
                (Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>>)null));
        }

        [TestMethod]
        public void OnCreateBatch_ValidRows_CallsNextWithMaterializedRows()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow(1), CreateValidRow(2) };
            IEnumerable<TransponderPlanRow> passed = null;

            var result = sut.OnCreate(rows.AsEnumerable(), r =>
            {
                passed = r;
                return r.ToList();
            });

            Assert.IsNotNull(passed);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateBatch_NullItem_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(new TransponderPlanRow[] { null }.AsEnumerable(), r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateBatch_DuplicateBandwidth_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow(5), CreateValidRow(5) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(rows.AsEnumerable(), r => r.ToList()));
        }

        [TestMethod]
        public void OnUpdateSingle_NullRow_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((TransponderPlanRow)null, r => r));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(CreateValidRow(), (Func<TransponderPlanRow, TransponderPlanRow>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_ValidRow_CallsNext()
        {
            var sut = CreateSut();
            var row = CreateValidRow();

            var result = sut.OnUpdate(row, r => r);

            Assert.AreSame(row, result);
        }

        [TestMethod]
        public void OnUpdateSingle_StepSizeSmallerThanBandwidth_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var row = CreateValidRow();
            row.StepSize = 0.5d;

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(row, r => r));
        }

        [TestMethod]
        public void OnUpdateBatch_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<TransponderPlanRow>)null, r => r.ToList()));
        }

        [TestMethod]
        public void OnUpdateBatch_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(
                new[] { CreateValidRow() }.AsEnumerable(),
                (Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>>)null));
        }

        [TestMethod]
        public void OnUpdateBatch_ValidRows_CallsNext()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow(1), CreateValidRow(2) };

            var result = sut.OnUpdate(rows.AsEnumerable(), r => r.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateBatch_DuplicateBandwidth_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow(3), CreateValidRow(3) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(rows.AsEnumerable(), r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(null, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(new[] { CreateValidRow() }.AsEnumerable(), null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_ValidRows_CallsNext()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow(1), CreateValidRow(2) };

            var result = sut.OnCreateOrUpdate(rows.AsEnumerable(), r => r.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_DuplicateBandwidth_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow(4), CreateValidRow(4) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(rows.AsEnumerable(), r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_EmptyCollection_CallsNext()
        {
            var sut = CreateSut();

            var result = sut.OnCreateOrUpdate(Enumerable.Empty<TransponderPlanRow>(), r => r.ToList());

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void OnCountFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = CreateSut();

            var result = sut.OnCount((FilterElement<TransponderPlanRow>)null, f => 7L);

            Assert.AreEqual(7L, result);
        }

        [TestMethod]
        public void OnCountQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = CreateSut();

            var result = sut.OnCount((IQuery<TransponderPlanRow>)null, q => 11L);

            Assert.AreEqual(11L, result);
        }

        [TestMethod]
        public void OnRead_ValidFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow() };

            var result = sut.OnRead((FilterElement<TransponderPlanRow>)null, f => rows);

            Assert.AreSame(rows, result);
        }

        [TestMethod]
        public void OnDeleteBatch_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((IEnumerable<TransponderPlanRow>)null, r => { }));
        }

        [TestMethod]
        public void OnDeleteBatch_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(
                new[] { CreateValidRow() }.AsEnumerable(),
                (Action<IEnumerable<TransponderPlanRow>>)null));
        }

        [TestMethod]
        public void OnDeleteBatch_ValidRows_CallsNext()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow(1), CreateValidRow(2) };
            IEnumerable<TransponderPlanRow> passed = null;

            sut.OnDelete(rows.AsEnumerable(), r => passed = r);

            Assert.AreEqual(2, passed.Count());
        }

        [TestMethod]
        public void OnDeleteBatch_InvalidRow_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var row = CreateValidRow();
            row.Bandwidth = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnDelete(new[] { row }.AsEnumerable(), r => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullRow_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((TransponderPlanRow)null, r => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(CreateValidRow(), (Action<TransponderPlanRow>)null));
        }

        [TestMethod]
        public void OnDeleteSingle_ValidRow_CallsNext()
        {
            var sut = CreateSut();
            var row = CreateValidRow();
            TransponderPlanRow passed = null;

            sut.OnDelete(row, r => passed = r);

            Assert.AreSame(row, passed);
        }

        [TestMethod]
        public void OnRead_Query_ReturnsNextResult()
        {
            var sut = CreateSut();
            var rows = new[] { CreateValidRow() };
            var mockQuery = new Mock<IQuery<TransponderPlanRow>>();
            IQuery<TransponderPlanRow> passed = null;

            var result = sut.OnRead(mockQuery.Object, q =>
            {
                passed = q;
                return rows;
            });

            Assert.AreSame(rows, result);
            Assert.AreSame(mockQuery.Object, passed);
        }

        [TestMethod]
        public void OnReadPaged_Filter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var pages = new IPagedResult<TransponderPlanRow>[0];
            var filter = new TRUEFilterElement<TransponderPlanRow>();
            FilterElement<TransponderPlanRow> passed = null;

            var result = sut.OnReadPaged(filter, f =>
            {
                passed = f;
                return pages;
            });

            Assert.AreSame(pages, result);
            Assert.AreSame(filter, passed);
        }

        [TestMethod]
        public void OnReadPaged_Query_ReturnsNextResult()
        {
            var sut = CreateSut();
            var pages = new IPagedResult<TransponderPlanRow>[0];
            var mockQuery = new Mock<IQuery<TransponderPlanRow>>();
            IQuery<TransponderPlanRow> passed = null;

            var result = sut.OnReadPaged(mockQuery.Object, q =>
            {
                passed = q;
                return pages;
            });

            Assert.AreSame(pages, result);
            Assert.AreSame(mockQuery.Object, passed);
        }

        [TestMethod]
        public void OnReadPaged_FilterWithPageSize_ReturnsNextResult()
        {
            var sut = CreateSut();
            var pages = new IPagedResult<TransponderPlanRow>[0];
            var filter = new TRUEFilterElement<TransponderPlanRow>();
            FilterElement<TransponderPlanRow> passedFilter = null;
            var passedPageSize = 0;

            var result = sut.OnReadPaged(filter, 25, (f, size) =>
            {
                passedFilter = f;
                passedPageSize = size;
                return pages;
            });

            Assert.AreSame(pages, result);
            Assert.AreSame(filter, passedFilter);
            Assert.AreEqual(25, passedPageSize);
        }

        [TestMethod]
        public void OnReadPaged_QueryWithPageSize_ReturnsNextResult()
        {
            var sut = CreateSut();
            var pages = new IPagedResult<TransponderPlanRow>[0];
            var mockQuery = new Mock<IQuery<TransponderPlanRow>>();
            IQuery<TransponderPlanRow> passedQuery = null;
            var passedPageSize = 0;

            var result = sut.OnReadPaged(mockQuery.Object, 50, (q, size) =>
            {
                passedQuery = q;
                passedPageSize = size;
                return pages;
            });

            Assert.AreSame(pages, result);
            Assert.AreSame(mockQuery.Object, passedQuery);
            Assert.AreEqual(50, passedPageSize);
        }

        private static TransponderPlanRowValidationMiddleware CreateSut()
        {
            return new TransponderPlanRowValidationMiddleware(_ => TransponderPlan.CreateNewTransponderPlan());
        }

        private static TransponderPlanRow CreateValidRow(double bandwidth = 1d)
        {
            var row = TransponderPlanRow.CreateNewTransponderPlanRow();
            row.TransponderPlan = PlanId;
            row.Bandwidth = bandwidth;
            row.StepSize = bandwidth + 10d;
            row.Offset = 0d;
            return row;
        }
    }
}
