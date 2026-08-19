namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;

    using Skyline.DataMiner.SDM.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>BeamValidationMiddleware</c> create/update validation behavior.
    /// </summary>
    [TestClass]
    public class BeamValidationMiddlewareTests
    {
        [TestMethod]
        public void OnCreateSingle_NullBeam_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((Beam)null, b => b));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(CreateBeam(Guid.NewGuid()), (Func<Beam, Beam>)null));
        }

        [TestMethod]
        public void OnCreateSingle_MissingSatellite_ThrowsArgumentException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(CreateBeam(null), b => b));
        }

        [TestMethod]
        public void OnCreateSingle_EmptySatellite_ThrowsArgumentException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(CreateBeam(Guid.Empty), b => b));
        }

        [TestMethod]
        public void OnCreateSingle_UnknownSatellite_ThrowsArgumentException()
        {
            var sut = CreateSut(false);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(CreateBeam(Guid.NewGuid()), b => b));
        }

        [TestMethod]
        public void OnCreateSingle_ValidBeam_CallsNext()
        {
            var sut = CreateSut(true);
            var beam = CreateBeam(Guid.NewGuid());
            var called = false;

            var result = sut.OnCreate(beam, b =>
            {
                called = true;
                return b;
            });

            Assert.IsTrue(called);
            Assert.AreSame(beam, result);
        }

        [TestMethod]
        public void OnCreateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<Beam>)null, b => b.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(new List<Beam>(), (Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>>)null));
        }

        [TestMethod]
        public void OnCreateBulk_CollectionWithNullItem_ThrowsArgumentException()
        {
            var sut = CreateSut(true);
            var input = new List<Beam> { null };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate((IEnumerable<Beam>)input, b => b.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_ValidBeams_CallsNext()
        {
            var sut = CreateSut(true);
            var input = new List<Beam> { CreateBeam(Guid.NewGuid()), CreateBeam(Guid.NewGuid()) };

            var result = sut.OnCreate((IEnumerable<Beam>)input, b => b.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateSingle_NullBeam_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((Beam)null, b => b));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(CreateBeam(Guid.NewGuid()), (Func<Beam, Beam>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_UnknownSatellite_ThrowsArgumentException()
        {
            var sut = CreateSut(false);

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(CreateBeam(Guid.NewGuid()), b => b));
        }

        [TestMethod]
        public void OnUpdateSingle_ValidBeam_CallsNext()
        {
            var sut = CreateSut(true);
            var beam = CreateBeam(Guid.NewGuid());

            var result = sut.OnUpdate(beam, b => b);

            Assert.AreSame(beam, result);
        }

        [TestMethod]
        public void OnUpdateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<Beam>)null, b => b.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(new List<Beam>(), (Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>>)null));
        }

        [TestMethod]
        public void OnUpdateBulk_BeamWithoutSatellite_ThrowsArgumentException()
        {
            var sut = CreateSut(true);
            var input = new List<Beam> { CreateBeam(null) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate((IEnumerable<Beam>)input, b => b.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_ValidBeams_CallsNext()
        {
            var sut = CreateSut(true);
            var input = new List<Beam> { CreateBeam(Guid.NewGuid()) };

            var result = sut.OnUpdate((IEnumerable<Beam>)input, b => b.ToList());

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(null, b => b.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(new List<Beam>(), null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_UnknownSatellite_ThrowsArgumentException()
        {
            var sut = CreateSut(false);
            var input = new List<Beam> { CreateBeam(Guid.NewGuid()) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(input, b => b.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_ValidBeams_CallsNext()
        {
            var sut = CreateSut(true);
            var input = new List<Beam> { CreateBeam(Guid.NewGuid()), CreateBeam(Guid.NewGuid()) };

            var result = sut.OnCreateOrUpdate(input, b => b.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCount_WithFilter_ReturnsNextResult()
        {
            var sut = CreateSut(true);
            FilterElement<Beam> received = null;
            var filter = new TRUEFilterElement<Beam>();

            var result = sut.OnCount(filter, f =>
            {
                received = f;
                return 42L;
            });

            Assert.AreEqual(42L, result);
            Assert.AreSame(filter, received);
        }

        [TestMethod]
        public void OnCount_WithQuery_ReturnsNextResult()
        {
            var sut = CreateSut(true);
            var query = new Mock<IQuery<Beam>>().Object;
            IQuery<Beam> received = null;

            var result = sut.OnCount(query, q =>
            {
                received = q;
                return 7L;
            });

            Assert.AreEqual(7L, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnRead_WithFilter_ReturnsNextResult()
        {
            var sut = CreateSut(true);
            var filter = new TRUEFilterElement<Beam>();
            var expected = new List<Beam> { CreateBeam(Guid.NewGuid()) };

            var result = sut.OnRead(filter, f => expected);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnRead_WithQuery_ReturnsNextResult()
        {
            var sut = CreateSut(true);
            var query = new Mock<IQuery<Beam>>().Object;
            IQuery<Beam> received = null;
            var expected = new List<Beam> { CreateBeam(Guid.NewGuid()) };

            var result = sut.OnRead(query, q =>
            {
                received = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnReadPaged_WithFilter_ReturnsNextResult()
        {
            var sut = CreateSut(true);
            var filter = new TRUEFilterElement<Beam>();
            FilterElement<Beam> received = null;
            var expected = new List<IPagedResult<Beam>>();

            var result = sut.OnReadPaged(filter, f =>
            {
                received = f;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(filter, received);
        }

        [TestMethod]
        public void OnReadPaged_WithQuery_ReturnsNextResult()
        {
            var sut = CreateSut(true);
            var query = new Mock<IQuery<Beam>>().Object;
            IQuery<Beam> received = null;
            var expected = new List<IPagedResult<Beam>>();

            var result = sut.OnReadPaged(query, q =>
            {
                received = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnReadPaged_WithFilterAndPageSize_ReturnsNextResult()
        {
            var sut = CreateSut(true);
            var filter = new TRUEFilterElement<Beam>();
            FilterElement<Beam> received = null;
            var receivedPageSize = 0;
            var expected = new List<IPagedResult<Beam>>();

            var result = sut.OnReadPaged(filter, 25, (f, size) =>
            {
                received = f;
                receivedPageSize = size;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(filter, received);
            Assert.AreEqual(25, receivedPageSize);
        }

        [TestMethod]
        public void OnReadPaged_WithQueryAndPageSize_ReturnsNextResult()
        {
            var sut = CreateSut(true);
            var query = new Mock<IQuery<Beam>>().Object;
            IQuery<Beam> received = null;
            var receivedPageSize = 0;
            var expected = new List<IPagedResult<Beam>>();

            var result = sut.OnReadPaged(query, 10, (q, size) =>
            {
                received = q;
                receivedPageSize = size;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
            Assert.AreEqual(10, receivedPageSize);
        }

        [TestMethod]
        public void OnDeleteBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((IEnumerable<Beam>)null, b => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(new List<Beam>(), (Action<IEnumerable<Beam>>)null));
        }

        [TestMethod]
        public void OnDeleteBulk_BeamWithoutSatellite_ThrowsArgumentException()
        {
            var sut = CreateSut(true);
            var input = new List<Beam> { CreateBeam(null) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnDelete((IEnumerable<Beam>)input, b => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_UnknownSatellite_ThrowsArgumentException()
        {
            var sut = CreateSut(false);
            var input = new List<Beam> { CreateBeam(Guid.NewGuid()) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnDelete((IEnumerable<Beam>)input, b => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_ValidBeams_CallsNext()
        {
            var sut = CreateSut(true);
            var input = new List<Beam> { CreateBeam(Guid.NewGuid()), CreateBeam(Guid.NewGuid()) };
            IEnumerable<Beam> received = null;

            sut.OnDelete((IEnumerable<Beam>)input, b => received = b);

            Assert.AreSame(input, received);
        }

        [TestMethod]
        public void OnDeleteSingle_NullBeam_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((Beam)null, b => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(true);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(CreateBeam(Guid.NewGuid()), (Action<Beam>)null));
        }

        [TestMethod]
        public void OnDeleteSingle_ValidBeam_CallsNextWithoutValidation()
        {
            var sut = CreateSut(false);
            var beam = CreateBeam(null);
            Beam received = null;

            sut.OnDelete(beam, b => received = b);

            Assert.AreSame(beam, received);
        }

        private static BeamValidationMiddleware CreateSut(bool satelliteExists)
        {
            return new BeamValidationMiddleware(id => satelliteExists ? Satellite.CreateNewSatellite() : null);
        }

        private static Beam CreateBeam(Guid? satelliteId)
        {
            var beam = Beam.CreateNewBeam();
            beam.BeamSatellite = satelliteId;
            return beam;
        }
    }
}
