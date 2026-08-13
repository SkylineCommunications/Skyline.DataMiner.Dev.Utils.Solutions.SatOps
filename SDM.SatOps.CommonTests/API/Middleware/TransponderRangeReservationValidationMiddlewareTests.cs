namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;

    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>TransponderRangeReservationValidationMiddleware</c> validation behavior.
    /// </summary>
    [TestClass]
    public class TransponderRangeReservationValidationMiddlewareTests
    {
        private static readonly Guid TransponderId = Guid.NewGuid();
        private static readonly DateTime BaseTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [TestMethod]
        public void Constructor_NullResolver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderRangeReservationValidationMiddleware(null));
        }

        [TestMethod]
        public void OnCreateSingle_NullReservation_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((TransponderRangeReservation)null, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(reservation, (Func<TransponderRangeReservation, TransponderRangeReservation>)null));
        }

        [TestMethod]
        public void OnCreateSingle_ValidReservation_CallsNext()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);
            var called = false;

            var result = sut.OnCreate(reservation, r =>
            {
                called = true;
                return r;
            });

            Assert.IsTrue(called);
            Assert.AreSame(reservation, result);
        }

        [TestMethod]
        public void OnCreateSingle_MissingTransponder_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);
            reservation.Transponder = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_EmptyTransponder_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);
            reservation.Transponder = Guid.Empty;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_MissingStartFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);
            reservation.RelativeStartFrequency = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_MissingEndFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);
            reservation.RelativeEndFrequency = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_MissingStartTime_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);
            reservation.StartTime = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_MissingEndTime_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);
            reservation.EndTime = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_EqualFrequencies_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 100);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_EqualTimes_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(2, 2, 100, 200);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_UnknownTransponder_ThrowsArgumentException()
        {
            var sut = new TransponderRangeReservationValidationMiddleware(id => null);
            var reservation = CreateReservation(0, 2, 100, 200);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<TransponderRangeReservation>)null, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200) };

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(
                (IEnumerable<TransponderRangeReservation>)reservations,
                (Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>>)null));
        }

        [TestMethod]
        public void OnCreateBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200), null };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_ValidReservations_CallsNext()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200), CreateReservation(2, 4, 100, 200) };

            var result = sut.OnCreate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateBulk_EmptyCollection_CallsNext()
        {
            var sut = CreateSut();

            var result = sut.OnCreate(Enumerable.Empty<TransponderRangeReservation>(), r => r.ToList());

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void OnUpdateSingle_NullReservation_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((TransponderRangeReservation)null, r => r));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(reservation, (Func<TransponderRangeReservation, TransponderRangeReservation>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_ValidReservation_CallsNext()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);

            var result = sut.OnUpdate(reservation, r => r);

            Assert.AreSame(reservation, result);
        }

        [TestMethod]
        public void OnUpdateSingle_InvalidReservation_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(4, 2, 100, 200);

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(reservation, r => r));
        }

        [TestMethod]
        public void OnUpdateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<TransponderRangeReservation>)null, r => r.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200) };

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(
                (IEnumerable<TransponderRangeReservation>)reservations,
                (Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>>)null));
        }

        [TestMethod]
        public void OnUpdateBulk_InvalidReservation_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 200, 100) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_ValidReservations_CallsNext()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200) };

            var result = sut.OnUpdate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList());

            Assert.AreEqual(1, result.Count);
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
            var reservations = new[] { CreateReservation(0, 2, 100, 200) };

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(reservations, null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_InvalidReservation_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200), CreateReservation(5, 1, 100, 200) };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(reservations, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_ValidReservations_CallsNext()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200), CreateReservation(2, 4, 100, 200) };
            var called = false;

            var result = sut.OnCreateOrUpdate(reservations, r =>
            {
                called = true;
                return r.ToList();
            });

            Assert.IsTrue(called);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCountFilter_ValidFilter_CallsNext()
        {
            var sut = CreateSut();
            FilterElement<TransponderRangeReservation> captured = new TRUEFilterElement<TransponderRangeReservation>();

            var result = sut.OnCount(captured, f =>
            {
                Assert.AreSame(captured, f);
                return 42L;
            });

            Assert.AreEqual(42L, result);
        }

        [TestMethod]
        public void OnCountQuery_ValidQuery_CallsNext()
        {
            var sut = CreateSut();

            var result = sut.OnCount((IQuery<TransponderRangeReservation>)null, q => 7L);

            Assert.AreEqual(7L, result);
        }

        [TestMethod]
        public void OnDeleteBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((IEnumerable<TransponderRangeReservation>)null, r => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200) };

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(
                (IEnumerable<TransponderRangeReservation>)reservations,
                (Action<IEnumerable<TransponderRangeReservation>>)null));
        }

        [TestMethod]
        public void OnDeleteBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var reservations = new[] { CreateReservation(0, 2, 100, 200) };
            IEnumerable<TransponderRangeReservation> captured = null;

            sut.OnDelete((IEnumerable<TransponderRangeReservation>)reservations, r => captured = r);

            Assert.AreSame(reservations, captured);
        }

        [TestMethod]
        public void OnDeleteSingle_NullReservation_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((TransponderRangeReservation)null, r => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(reservation, (Action<TransponderRangeReservation>)null));
        }

        [TestMethod]
        public void OnDeleteSingle_ValidReservation_CallsNext()
        {
            var sut = CreateSut();
            var reservation = CreateReservation(0, 2, 100, 200);
            TransponderRangeReservation captured = null;

            sut.OnDelete(reservation, r => captured = r);

            Assert.AreSame(reservation, captured);
        }

        [TestMethod]
        public void OnReadFilter_ValidFilter_CallsNext()
        {
            var sut = CreateSut();
            var expected = new[] { CreateReservation(0, 2, 100, 200) };
            FilterElement<TransponderRangeReservation> filter = new TRUEFilterElement<TransponderRangeReservation>();

            var result = sut.OnRead(filter, f =>
            {
                Assert.AreSame(filter, f);
                return expected;
            });

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadQuery_ValidQuery_CallsNext()
        {
            var sut = CreateSut();
            var expected = new[] { CreateReservation(0, 2, 100, 200) };

            var result = sut.OnRead((IQuery<TransponderRangeReservation>)null, q =>
            {
                Assert.IsNull(q);
                return expected;
            });

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedFilter_ValidFilter_CallsNext()
        {
            var sut = CreateSut();
            var expected = Enumerable.Empty<IPagedResult<TransponderRangeReservation>>();
            FilterElement<TransponderRangeReservation> filter = new TRUEFilterElement<TransponderRangeReservation>();

            var result = sut.OnReadPaged(filter, f =>
            {
                Assert.AreSame(filter, f);
                return expected;
            });

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedQuery_ValidQuery_CallsNext()
        {
            var sut = CreateSut();
            var expected = Enumerable.Empty<IPagedResult<TransponderRangeReservation>>();

            var result = sut.OnReadPaged((IQuery<TransponderRangeReservation>)null, q =>
            {
                Assert.IsNull(q);
                return expected;
            });

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedFilterWithPageSize_ValidArguments_CallsNextWithPageSize()
        {
            var sut = CreateSut();
            var expected = Enumerable.Empty<IPagedResult<TransponderRangeReservation>>();
            FilterElement<TransponderRangeReservation> filter = new TRUEFilterElement<TransponderRangeReservation>();
            var capturedPageSize = 0;

            var result = sut.OnReadPaged(filter, 25, (f, size) =>
            {
                Assert.AreSame(filter, f);
                capturedPageSize = size;
                return expected;
            });

            Assert.AreEqual(25, capturedPageSize);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedQueryWithPageSize_ValidArguments_CallsNextWithPageSize()
        {
            var sut = CreateSut();
            var expected = Enumerable.Empty<IPagedResult<TransponderRangeReservation>>();
            var capturedPageSize = 0;

            var result = sut.OnReadPaged((IQuery<TransponderRangeReservation>)null, 10, (q, size) =>
            {
                Assert.IsNull(q);
                capturedPageSize = size;
                return expected;
            });

            Assert.AreEqual(10, capturedPageSize);
            Assert.AreSame(expected, result);
        }

        private static TransponderRangeReservationValidationMiddleware CreateSut()
        {
            return new TransponderRangeReservationValidationMiddleware(id => Transponder.CreateNewTransponder());
        }

        private static TransponderRangeReservation CreateReservation(int startHour, int endHour, double startFrequency, double endFrequency)
        {
            var reservation = TransponderRangeReservation.CreateNew();

            reservation.Transponder = TransponderId;
            reservation.StartTime = BaseTime.AddHours(startHour);
            reservation.EndTime = BaseTime.AddHours(endHour);
            reservation.RelativeStartFrequency = startFrequency;
            reservation.RelativeEndFrequency = endFrequency;

            return reservation;
        }
    }
}
