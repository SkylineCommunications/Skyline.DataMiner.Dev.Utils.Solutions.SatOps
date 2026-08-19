namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>TransponderValidationMiddleware</c>.
    /// </summary>
    [TestClass]
    public class TransponderValidationMiddlewareTests
    {
        [TestMethod]
        public void Constructor_NullResolver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderValidationMiddleware(null));
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

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(CreateValidTransponder(), (Func<Transponder, Transponder>)null));
        }

        [TestMethod]
        public void OnCreateSingle_ValidTransponder_CallsNext()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            var called = false;

            var result = sut.OnCreate(transponder, t =>
            {
                called = true;
                return t;
            });

            Assert.IsTrue(called);
            Assert.AreSame(transponder, result);
        }

        [TestMethod]
        public void OnCreateSingle_MissingName_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.Name = " ";

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_EmptySatellite_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.TransponderSatellite = Guid.Empty;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_MissingBandwidth_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.Bandwidth = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_MissingStartFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.StartFrequency = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_MissingStopFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.StopFrequency = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_MissingDownlinkStartFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.DownlinkStartFreq = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_MissingDownlinkEndFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.DownlinkEndFreq = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_MissingHardEndDate_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.HardEndDate = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_EmptyDomResource_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.DOMResource = Guid.Empty;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
        }

        [TestMethod]
        public void OnCreateSingle_UnknownSatellite_ThrowsArgumentException()
        {
            var sut = new TransponderValidationMiddleware(id => null);
            var transponder = CreateValidTransponder();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(transponder, t => t));
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

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(new[] { CreateValidTransponder() }, (Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>>)null));
        }

        [TestMethod]
        public void OnCreateBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(new Transponder[] { null }, t => t.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            var called = false;

            var result = sut.OnCreate(new[] { transponder }, t =>
            {
                called = true;
                return t.ToList();
            });

            Assert.IsTrue(called);
            Assert.AreEqual(1, result.Count);
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

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(CreateValidTransponder(), (Func<Transponder, Transponder>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_InvalidTransponder_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            transponder.Name = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(transponder, t => t));
        }

        [TestMethod]
        public void OnUpdateSingle_ValidTransponder_CallsNext()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            var called = false;

            var result = sut.OnUpdate(transponder, t =>
            {
                called = true;
                return t;
            });

            Assert.IsTrue(called);
            Assert.AreSame(transponder, result);
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

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(new[] { CreateValidTransponder() }, (Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>>)null));
        }

        [TestMethod]
        public void OnUpdateBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(new Transponder[] { null }, t => t.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var called = false;

            var result = sut.OnUpdate(new[] { CreateValidTransponder(), CreateValidTransponder() }, t =>
            {
                called = true;
                return t.ToList();
            });

            Assert.IsTrue(called);
            Assert.AreEqual(2, result.Count);
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

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(new[] { CreateValidTransponder() }, null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(new Transponder[] { null }, t => t.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var called = false;

            var result = sut.OnCreateOrUpdate(new[] { CreateValidTransponder() }, t =>
            {
                called = true;
                return t.ToList();
            });

            Assert.IsTrue(called);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void OnCountFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var called = false;

            var result = sut.OnCount((FilterElement<Transponder>)null, f =>
            {
                called = true;
                return 42L;
            });

            Assert.IsTrue(called);
            Assert.AreEqual(42L, result);
        }

        [TestMethod]
        public void OnCountQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = CreateSut();
            var called = false;

            var result = sut.OnCount((IQuery<Transponder>)null, q =>
            {
                called = true;
                return 7L;
            });

            Assert.IsTrue(called);
            Assert.AreEqual(7L, result);
        }

        [TestMethod]
        public void OnDeleteBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((IEnumerable<Transponder>)null, t => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(new[] { CreateValidTransponder() }, (Action<IEnumerable<Transponder>>)null));
        }

        [TestMethod]
        public void OnDeleteBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnDelete(new Transponder[] { null }, t => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            IEnumerable<Transponder> received = null;
            var input = new[] { CreateValidTransponder(), CreateValidTransponder() };

            sut.OnDelete(input, t => received = t);

            Assert.AreSame(input, received);
        }

        [TestMethod]
        public void OnDeleteSingle_NullTransponder_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((Transponder)null, t => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(CreateValidTransponder(), (Action<Transponder>)null));
        }

        [TestMethod]
        public void OnDeleteSingle_ValidTransponder_CallsNext()
        {
            var sut = CreateSut();
            var transponder = CreateValidTransponder();
            Transponder received = null;

            sut.OnDelete(transponder, t => received = t);

            Assert.AreSame(transponder, received);
        }

        [TestMethod]
        public void OnReadFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<Transponder> { CreateValidTransponder() };

            var result = sut.OnRead((FilterElement<Transponder>)null, f => expected);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<Transponder> { CreateValidTransponder() };
            IQuery<Transponder> received = null;
            var query = new Mock<IQuery<Transponder>>().Object;

            var result = sut.OnRead(query, q =>
            {
                received = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnReadPagedFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<Transponder>>();
            var called = false;

            var result = sut.OnReadPaged((FilterElement<Transponder>)null, f =>
            {
                called = true;
                return expected;
            });

            Assert.IsTrue(called);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<Transponder>>();
            var query = new Mock<IQuery<Transponder>>().Object;
            IQuery<Transponder> received = null;

            var result = sut.OnReadPaged(query, q =>
            {
                received = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnReadPagedFilterWithPageSize_ValidArguments_PassesPageSizeToNext()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<Transponder>>();
            var receivedPageSize = 0;

            var result = sut.OnReadPaged((FilterElement<Transponder>)null, 25, (f, size) =>
            {
                receivedPageSize = size;
                return expected;
            });

            Assert.AreEqual(25, receivedPageSize);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedQueryWithPageSize_ValidArguments_PassesQueryAndPageSizeToNext()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<Transponder>>();
            var query = new Mock<IQuery<Transponder>>().Object;
            IQuery<Transponder> received = null;
            var receivedPageSize = 0;

            var result = sut.OnReadPaged(query, 100, (q, size) =>
            {
                received = q;
                receivedPageSize = size;
                return expected;
            });

            Assert.AreSame(query, received);
            Assert.AreEqual(100, receivedPageSize);
            Assert.AreSame(expected, result);
        }

        private static TransponderValidationMiddleware CreateSut()
        {
            return new TransponderValidationMiddleware(id => new Satellite());
        }

        private static Transponder CreateValidTransponder()
        {
            var transponder = new Transponder();
            transponder.Name = "TP1";
            transponder.TransponderSatellite = Guid.NewGuid();
            transponder.Bandwidth = 10;
            transponder.StartFrequency = 100;
            transponder.StopFrequency = 110;
            transponder.DownlinkStartFreq = 200;
            transponder.DownlinkEndFreq = 210;
            transponder.HardEndDate = DateTime.UtcNow;
            transponder.DOMResource = Guid.NewGuid();
            return transponder;
        }
    }
}
