namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;

    using Skyline.DataMiner.SDM.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;

    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>TransponderNameUniquenessMiddleware</c>.
    /// </summary>
    [TestClass]
    public class TransponderNameUniquenessMiddlewareTests
    {
        [TestMethod]
        public void Constructor_NullResolver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderNameUniquenessMiddleware(null));
        }

        [TestMethod]
        public void OnCreateSingle_NullTransponder_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((Transponder)null, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(CreateTransponder("A"), (Func<Transponder, Transponder>)null));
        }

        [TestMethod]
        public void OnCreateSingle_UniqueName_CallsNext()
        {
            var sut = CreateSut(CreateTransponder("Other"));
            var transponder = CreateTransponder("A");

            var result = sut.OnCreate(transponder, t => t);

            Assert.AreSame(transponder, result);
        }

        [TestMethod]
        public void OnCreateSingle_ExistingName_ThrowsArgumentException()
        {
            var sut = CreateSut(CreateTransponder("Taken"));

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(CreateTransponder(" taken "), t => t));
        }

        [TestMethod]
        public void OnCreateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<Transponder>)null, t => t.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(new[] { CreateTransponder("A") }, null));
        }

        [TestMethod]
        public void OnCreateBulk_UniqueNames_CallsNext()
        {
            var sut = CreateSut();
            var transponders = new[] { CreateTransponder("A"), CreateTransponder("B") };

            var result = sut.OnCreate(transponders, t => t.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateBulk_DuplicateNamesInBatch_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponders = new[] { CreateTransponder("A"), CreateTransponder("a") };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponders, t => t.ToList()));
        }

        [TestMethod]
        public void OnUpdateSingle_NullTransponder_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((Transponder)null, t => t));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(CreateTransponder("A"), (Func<Transponder, Transponder>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_SameTransponderId_CallsNext()
        {
            var existing = CreateTransponder("A");
            var sut = CreateSut(existing);

            var result = sut.OnUpdate(existing, t => t);

            Assert.AreSame(existing, result);
        }

        [TestMethod]
        public void OnUpdateSingle_NameTakenByOther_ThrowsArgumentException()
        {
            var sut = CreateSut(CreateTransponder("Taken"));

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(CreateTransponder("TAKEN"), t => t));
        }

        [TestMethod]
        public void OnUpdateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<Transponder>)null, t => t.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(new[] { CreateTransponder("A") }, null));
        }

        [TestMethod]
        public void OnUpdateBulk_UniqueNames_CallsNext()
        {
            var sut = CreateSut();
            var transponders = new[] { CreateTransponder("A"), CreateTransponder("B") };

            var result = sut.OnUpdate(transponders, t => t.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateBulk_DuplicateNames_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponders = new[] { CreateTransponder("A"), CreateTransponder("A") };

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(transponders, t => t.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(null, t => t.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(new[] { CreateTransponder("A") }, null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_UniqueNames_CallsNext()
        {
            var sut = CreateSut();
            var transponders = new[] { CreateTransponder("A"), CreateTransponder("B") };
            var called = false;

            var result = sut.OnCreateOrUpdate(transponders, t =>
            {
                called = true;
                return t.ToList();
            });

            Assert.IsTrue(called);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_NameAlreadyExists_ThrowsArgumentException()
        {
            var sut = CreateSut(CreateTransponder("Taken"));

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(new[] { CreateTransponder("Taken") }, t => t.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NoNamedTransponders_CallsNext()
        {
            var sut = CreateSut(CreateTransponder("Taken"));
            var transponders = new[] { CreateTransponder(" "), null };

            var result = sut.OnCreateOrUpdate(transponders, t => t.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_ResolverReturnsNull_CallsNext()
        {
            var sut = new TransponderNameUniquenessMiddleware(() => null);

            var result = sut.OnCreateOrUpdate(new[] { CreateTransponder("A") }, t => t.ToList());

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void OnCount_WithFilter_PassesFilterToNext()
        {
            var sut = CreateSut();
            FilterElement<Transponder> captured = null;
            var filter = new TRUEFilterElement<Transponder>();

            var result = sut.OnCount(filter, f =>
            {
                captured = f;
                return 42L;
            });

            Assert.AreEqual(42L, result);
            Assert.AreSame(filter, captured);
        }

        [TestMethod]
        public void OnCount_WithQuery_PassesQueryToNext()
        {
            var sut = CreateSut();
            var queryMock = new Mock<IQuery<Transponder>>();
            IQuery<Transponder> captured = null;

            var result = sut.OnCount(queryMock.Object, q =>
            {
                captured = q;
                return 7L;
            });

            Assert.AreEqual(7L, result);
            Assert.AreSame(queryMock.Object, captured);
        }

        [TestMethod]
        public void OnDeleteBulk_CallsNextWithSameCollection()
        {
            var sut = CreateSut();
            var transponders = new[] { CreateTransponder("A") };
            IEnumerable<Transponder> captured = null;

            sut.OnDelete(transponders, t => captured = t);

            Assert.AreSame(transponders, captured);
        }

        [TestMethod]
        public void OnDeleteSingle_CallsNextWithSameTransponder()
        {
            var sut = CreateSut();
            var transponder = CreateTransponder("A");
            Transponder captured = null;

            sut.OnDelete(transponder, t => captured = t);

            Assert.AreSame(transponder, captured);
        }

        [TestMethod]
        public void OnRead_WithFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var filter = new TRUEFilterElement<Transponder>();
            var expected = new[] { CreateTransponder("A") };
            FilterElement<Transponder> captured = null;

            var result = sut.OnRead(filter, f =>
            {
                captured = f;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(filter, captured);
        }

        [TestMethod]
        public void OnRead_WithQuery_ReturnsNextResult()
        {
            var sut = CreateSut();
            var queryMock = new Mock<IQuery<Transponder>>();
            var expected = new[] { CreateTransponder("A") };
            IQuery<Transponder> captured = null;

            var result = sut.OnRead(queryMock.Object, q =>
            {
                captured = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(queryMock.Object, captured);
        }

        [TestMethod]
        public void OnReadPaged_WithFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var filter = new TRUEFilterElement<Transponder>();
            var expected = new[] { new Mock<IPagedResult<Transponder>>().Object };
            FilterElement<Transponder> captured = null;

            var result = sut.OnReadPaged(filter, f =>
            {
                captured = f;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(filter, captured);
        }

        [TestMethod]
        public void OnReadPaged_WithQuery_ReturnsNextResult()
        {
            var sut = CreateSut();
            var queryMock = new Mock<IQuery<Transponder>>();
            var expected = new[] { new Mock<IPagedResult<Transponder>>().Object };
            IQuery<Transponder> captured = null;

            var result = sut.OnReadPaged(queryMock.Object, q =>
            {
                captured = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(queryMock.Object, captured);
        }

        [TestMethod]
        public void OnReadPaged_WithFilterAndPageSize_ReturnsNextResult()
        {
            var sut = CreateSut();
            var filter = new TRUEFilterElement<Transponder>();
            var expected = new[] { new Mock<IPagedResult<Transponder>>().Object };
            FilterElement<Transponder> capturedFilter = null;
            var capturedPageSize = 0;

            var result = sut.OnReadPaged(filter, 25, (f, size) =>
            {
                capturedFilter = f;
                capturedPageSize = size;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(filter, capturedFilter);
            Assert.AreEqual(25, capturedPageSize);
        }

        [TestMethod]
        public void OnReadPaged_WithQueryAndPageSize_ReturnsNextResult()
        {
            var sut = CreateSut();
            var queryMock = new Mock<IQuery<Transponder>>();
            var expected = new[] { new Mock<IPagedResult<Transponder>>().Object };
            IQuery<Transponder> capturedQuery = null;
            var capturedPageSize = 0;

            var result = sut.OnReadPaged(queryMock.Object, 10, (q, size) =>
            {
                capturedQuery = q;
                capturedPageSize = size;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(queryMock.Object, capturedQuery);
            Assert.AreEqual(10, capturedPageSize);
        }

        private static TransponderNameUniquenessMiddleware CreateSut(params Transponder[] existing)
        {
            var list = existing ?? new Transponder[0];
            return new TransponderNameUniquenessMiddleware(() => list);
        }

        private static Transponder CreateTransponder(string name)
        {
            var transponder = Transponder.CreateNewTransponder();
            transponder.Name = name;
            return transponder;
        }
    }
}
