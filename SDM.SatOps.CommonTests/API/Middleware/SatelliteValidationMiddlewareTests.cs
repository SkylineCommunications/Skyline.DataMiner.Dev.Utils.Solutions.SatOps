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
    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>SatelliteValidationMiddleware</c> validation behavior.
    /// </summary>
    [TestClass]
    public class SatelliteValidationMiddlewareTests
    {
        [TestMethod]
        public void OnCreateSingle_NullSatellite_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((Satellite)null, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(CreateSatellite("A", "AA"), (Func<Satellite, Satellite>)null));
        }

        [TestMethod]
        public void OnCreateSingle_MissingName_ThrowsArgumentException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(CreateSatellite(" ", "AA"), s => s));
        }

        [TestMethod]
        public void OnCreateSingle_MissingAbbreviation_ThrowsArgumentException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(CreateSatellite("A", null), s => s));
        }

        [TestMethod]
        public void OnCreateSingle_ValidSatellite_CallsNext()
        {
            var sut = new SatelliteValidationMiddleware();
            var satellite = CreateSatellite("A", "AA");
            var called = false;

            var result = sut.OnCreate(satellite, s =>
            {
                called = true;
                return s;
            });

            Assert.IsTrue(called);
            Assert.AreSame(satellite, result);
        }

        [TestMethod]
        public void OnCreateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<Satellite>)null, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(new[] { CreateSatellite("A", "AA") }, null));
        }

        [TestMethod]
        public void OnCreateBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(new Satellite[] { null }, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_ValidSatellites_CallsNext()
        {
            var sut = new SatelliteValidationMiddleware();
            var satellites = new[] { CreateSatellite("A", "AA"), CreateSatellite("B", "BB") };

            var result = sut.OnCreate(satellites, s => s.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateSingle_NullSatellite_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((Satellite)null, s => s));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(CreateSatellite("A", "AA"), (Func<Satellite, Satellite>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_InvalidSatellite_ThrowsArgumentException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(CreateSatellite(null, "AA"), s => s));
        }

        [TestMethod]
        public void OnUpdateSingle_ValidSatellite_CallsNext()
        {
            var sut = new SatelliteValidationMiddleware();
            var satellite = CreateSatellite("A", "AA");

            var result = sut.OnUpdate(satellite, s => s);

            Assert.AreSame(satellite, result);
        }

        [TestMethod]
        public void OnUpdateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<Satellite>)null, s => s.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(new[] { CreateSatellite("A", "AA") }, null));
        }

        [TestMethod]
        public void OnUpdateBulk_InvalidSatellite_ThrowsArgumentException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(new[] { CreateSatellite("A", " ") }, s => s.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_ValidSatellites_CallsNext()
        {
            var sut = new SatelliteValidationMiddleware();
            var satellites = new[] { CreateSatellite("A", "AA") };

            var result = sut.OnUpdate(satellites, s => s.ToList());

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullCollection_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(null, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(new[] { CreateSatellite("A", "AA") }, null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_InvalidSatellite_ThrowsArgumentException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(new[] { CreateSatellite(String.Empty, "AA") }, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_ValidSatellites_CallsNext()
        {
            var sut = new SatelliteValidationMiddleware();
            var satellites = new[] { CreateSatellite("A", "AA"), CreateSatellite("B", "BB") };
            IEnumerable<Satellite> received = null;

            var result = sut.OnCreateOrUpdate(satellites, s =>
            {
                received = s;
                return s.ToList();
            });

            Assert.AreSame(satellites, received);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCountFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = new SatelliteValidationMiddleware();
            FilterElement<Satellite> received = new TRUEFilterElement<Satellite>();
            FilterElement<Satellite> captured = null;

            var result = sut.OnCount(received, f =>
            {
                captured = f;
                return 42L;
            });

            Assert.AreEqual(42L, result);
            Assert.AreSame(received, captured);
        }

        [TestMethod]
        public void OnCountQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = new SatelliteValidationMiddleware();
            var query = new Mock<IQuery<Satellite>>().Object;
            IQuery<Satellite> captured = null;

            var result = sut.OnCount(query, q =>
            {
                captured = q;
                return 7L;
            });

            Assert.AreEqual(7L, result);
            Assert.AreSame(query, captured);
        }

        [TestMethod]
        public void OnDeleteBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((IEnumerable<Satellite>)null, s => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(new[] { CreateSatellite("A", "AA") }, (Action<IEnumerable<Satellite>>)null));
        }

        [TestMethod]
        public void OnDeleteBulk_InvalidSatellite_ThrowsArgumentException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentException>(() => sut.OnDelete(new[] { CreateSatellite("A", " ") }, s => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_ValidSatellites_CallsNext()
        {
            var sut = new SatelliteValidationMiddleware();
            var satellites = new[] { CreateSatellite("A", "AA") };
            IEnumerable<Satellite> received = null;

            sut.OnDelete(satellites, s => received = s);

            Assert.AreSame(satellites, received);
        }

        [TestMethod]
        public void OnDeleteSingle_NullSatellite_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((Satellite)null, s => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SatelliteValidationMiddleware();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(CreateSatellite("A", "AA"), (Action<Satellite>)null));
        }

        [TestMethod]
        public void OnDeleteSingle_ValidSatellite_CallsNext()
        {
            var sut = new SatelliteValidationMiddleware();
            var satellite = CreateSatellite(null, null);
            Satellite received = null;

            sut.OnDelete(satellite, s => received = s);

            Assert.AreSame(satellite, received);
        }

        [TestMethod]
        public void OnReadFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = new SatelliteValidationMiddleware();
            FilterElement<Satellite> filter = new TRUEFilterElement<Satellite>();
            var expected = new[] { CreateSatellite("A", "AA") };
            FilterElement<Satellite> captured = null;

            var result = sut.OnRead(filter, f =>
            {
                captured = f;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(filter, captured);
        }

        [TestMethod]
        public void OnReadQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = new SatelliteValidationMiddleware();
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new[] { CreateSatellite("A", "AA") };
            IQuery<Satellite> captured = null;

            var result = sut.OnRead(query, q =>
            {
                captured = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, captured);
        }

        [TestMethod]
        public void OnReadPagedFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = new SatelliteValidationMiddleware();
            FilterElement<Satellite> filter = new TRUEFilterElement<Satellite>();
            var expected = new IPagedResult<Satellite>[0];
            FilterElement<Satellite> captured = null;

            var result = sut.OnReadPaged(filter, f =>
            {
                captured = f;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(filter, captured);
        }

        [TestMethod]
        public void OnReadPagedQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = new SatelliteValidationMiddleware();
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new IPagedResult<Satellite>[0];
            IQuery<Satellite> captured = null;

            var result = sut.OnReadPaged(query, q =>
            {
                captured = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, captured);
        }

        [TestMethod]
        public void OnReadPagedFilterWithPageSize_ValidFilter_ReturnsNextResult()
        {
            var sut = new SatelliteValidationMiddleware();
            FilterElement<Satellite> filter = new TRUEFilterElement<Satellite>();
            var expected = new IPagedResult<Satellite>[0];
            FilterElement<Satellite> captured = null;
            var capturedPageSize = 0;

            var result = sut.OnReadPaged(filter, 25, (f, size) =>
            {
                captured = f;
                capturedPageSize = size;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(filter, captured);
            Assert.AreEqual(25, capturedPageSize);
        }

        [TestMethod]
        public void OnReadPagedQueryWithPageSize_ValidQuery_ReturnsNextResult()
        {
            var sut = new SatelliteValidationMiddleware();
            var query = new Mock<IQuery<Satellite>>().Object;
            var expected = new IPagedResult<Satellite>[0];
            IQuery<Satellite> captured = null;
            var capturedPageSize = 0;

            var result = sut.OnReadPaged(query, 0, (q, size) =>
            {
                captured = q;
                capturedPageSize = size;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, captured);
            Assert.AreEqual(0, capturedPageSize);
        }

        private static Satellite CreateSatellite(string name, string abbreviation)
        {
            var satellite = Satellite.CreateNewSatellite();
            satellite.Name = name;
            satellite.Abbreviation = abbreviation;
            return satellite;
        }
    }
}
