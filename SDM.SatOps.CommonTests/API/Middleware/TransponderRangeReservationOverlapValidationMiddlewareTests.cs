namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation;
    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>TransponderRangeReservationOverlapValidationMiddleware</c> validation behavior.
    /// </summary>
    [TestClass]
    public class TransponderRangeReservationOverlapValidationMiddlewareTests
    {
        private static readonly Guid TransponderId = Guid.NewGuid();
        private static readonly DateTime BaseTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [TestMethod]
        public void Constructor_NullResolver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderRangeReservationOverlapValidationMiddleware(null));
        }

        [TestMethod]
        public void Constructor_ValidResolver_CreatesInstance()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());

            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void OnCreateSingle_NullReservation_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((TransponderRangeReservation)null, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservation = CreateReservation("A", 0, 2, 100, 200);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(reservation, (Func<TransponderRangeReservation, TransponderRangeReservation>)null));
        }

        [TestMethod]
        public void OnCreateSingle_NoOverlap_CallsNext()
        {
            var existing = CreateReservation("Existing", 2, 4, 100, 200);
            var sut = CreateSut(id => new[] { existing });
            var reservation = CreateReservation("New", 0, 2, 100, 200);
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
        public void OnCreateSingle_TimeOverlapButNoFrequencyOverlap_CallsNext()
        {
            var existing = CreateReservation("Existing", 0, 4, 200, 300);
            var sut = CreateSut(id => new[] { existing });
            var reservation = CreateReservation("New", 0, 2, 100, 200);

            var result = sut.OnCreate(reservation, r => r);

            Assert.AreSame(reservation, result);
        }

        [TestMethod]
        public void OnCreateSingle_OverlappingReservation_ThrowsArgumentException()
        {
            var existing = CreateReservation("Existing", 1, 4, 150, 300);
            var sut = CreateSut(id => new[] { existing });
            var reservation = CreateReservation("New", 0, 2, 100, 200);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateSingle_MissingTransponder_SkipsValidation()
        {
            var resolverCalled = false;
            var sut = CreateSut(id =>
            {
                resolverCalled = true;
                return Enumerable.Empty<TransponderRangeReservation>();
            });
            var reservation = CreateReservation("New", 0, 2, 100, 200);
            reservation.Transponder = null;

            var result = sut.OnCreate(reservation, r => r);

            Assert.IsFalse(resolverCalled);
            Assert.AreSame(reservation, result);
        }

        [TestMethod]
        public void OnCreateSingle_EmptyTransponderId_SkipsValidation()
        {
            var resolverCalled = false;
            var sut = CreateSut(id =>
            {
                resolverCalled = true;
                return Enumerable.Empty<TransponderRangeReservation>();
            });
            var reservation = CreateReservation("New", 0, 2, 100, 200);
            reservation.Transponder = Guid.Empty;

            sut.OnCreate(reservation, r => r);

            Assert.IsFalse(resolverCalled);
        }

        [TestMethod]
        public void OnCreateSingle_MissingRanges_SkipsValidation()
        {
            var resolverCalled = false;
            var sut = CreateSut(id =>
            {
                resolverCalled = true;
                return Enumerable.Empty<TransponderRangeReservation>();
            });

            var noStartTime = CreateReservation("A", 0, 2, 100, 200);
            noStartTime.StartTime = null;
            sut.OnCreate(noStartTime, r => r);

            var noEndTime = CreateReservation("B", 0, 2, 100, 200);
            noEndTime.EndTime = null;
            sut.OnCreate(noEndTime, r => r);

            var noStartFrequency = CreateReservation("C", 0, 2, 100, 200);
            noStartFrequency.RelativeStartFrequency = null;
            sut.OnCreate(noStartFrequency, r => r);

            var noEndFrequency = CreateReservation("D", 0, 2, 100, 200);
            noEndFrequency.RelativeEndFrequency = null;
            sut.OnCreate(noEndFrequency, r => r);

            Assert.IsFalse(resolverCalled);
        }

        [TestMethod]
        public void OnCreateSingle_ResolverReturnsNull_CallsNext()
        {
            var sut = CreateSut(id => null);
            var reservation = CreateReservation("New", 0, 2, 100, 200);

            var result = sut.OnCreate(reservation, r => r);

            Assert.AreSame(reservation, result);
        }

        [TestMethod]
        public void OnCreateSingle_ExistingWithoutRangesOrNull_Ignored()
        {
            var incomplete = CreateReservation("Existing", 0, 4, 100, 300);
            incomplete.RelativeEndFrequency = null;
            var sut = CreateSut(id => new[] { null, incomplete });
            var reservation = CreateReservation("New", 0, 2, 100, 200);

            var result = sut.OnCreate(reservation, r => r);

            Assert.AreSame(reservation, result);
        }

        [TestMethod]
        public void OnUpdateSingle_NullReservation_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((TransponderRangeReservation)null, r => r));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservation = CreateReservation("A", 0, 2, 100, 200);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(reservation, (Func<TransponderRangeReservation, TransponderRangeReservation>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_ExcludesItself_CallsNext()
        {
            var reservation = CreateReservation("Reservation", 0, 2, 100, 200, Guid.NewGuid());
            var sut = CreateSut(id => new[] { reservation });

            var result = sut.OnUpdate(reservation, r => r);

            Assert.AreSame(reservation, result);
        }

        [TestMethod]
        public void OnUpdateSingle_OverlappingOtherReservation_ThrowsArgumentException()
        {
            var existing = CreateReservation("Existing", 1, 4, 150, 300, Guid.NewGuid());
            var sut = CreateSut(id => new[] { existing });
            var reservation = CreateReservation("Reservation", 0, 2, 100, 200, Guid.NewGuid());

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(reservation, r => r));
        }

        [TestMethod]
        public void OnCreateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<TransponderRangeReservation>)null, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[] { CreateReservation("A", 0, 2, 100, 200) };

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(
                (IEnumerable<TransponderRangeReservation>)reservations,
                (Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>>)null));
        }

        [TestMethod]
        public void OnCreateBulk_TouchingBoundaries_CallsNext()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[]
            {
                CreateReservation("A", 0, 2, 100, 200),
                CreateReservation("B", 2, 4, 100, 200),
            };

            var result = sut.OnCreate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateBulk_OverlappingReservations_ThrowsArgumentException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[]
            {
                CreateReservation("A", 0, 3, 100, 200),
                CreateReservation("B", 1, 4, 150, 250),
            };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_DifferentTransponders_CallsNext()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var other = CreateReservation("B", 1, 4, 150, 250);
            other.Transponder = Guid.NewGuid();
            var reservations = new[]
            {
                CreateReservation("A", 0, 3, 100, 200),
                other,
            };

            var result = sut.OnCreate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<TransponderRangeReservation>)null, r => r.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[] { CreateReservation("A", 0, 2, 100, 200) };

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(
                (IEnumerable<TransponderRangeReservation>)reservations,
                (Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>>)null));
        }

        [TestMethod]
        public void OnUpdateBulk_NoOverlap_CallsNext()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[]
            {
                CreateReservation("A", 0, 2, 100, 200),
                CreateReservation("B", 3, 5, 100, 200),
            };

            var result = sut.OnUpdate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateBulk_OverlappingReservations_ThrowsArgumentException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[]
            {
                CreateReservation("A", 0, 3, 100, 200),
                CreateReservation("B", 1, 4, 150, 250),
            };

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate((IEnumerable<TransponderRangeReservation>)reservations, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(null, r => r.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[] { CreateReservation("A", 0, 2, 100, 200) };

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(reservations, null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NoOverlap_CallsNext()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[]
            {
                CreateReservation("A", 0, 2, 100, 200),
                CreateReservation("B", 2, 4, 100, 200),
            };
            IEnumerable<TransponderRangeReservation> received = null;

            var result = sut.OnCreateOrUpdate(reservations, r =>
            {
                received = r;
                return r.ToList();
            });

            Assert.AreSame(reservations, received);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_OverlappingReservations_ThrowsArgumentException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[]
            {
                CreateReservation("A", 0, 3, 100, 200),
                CreateReservation("B", 1, 4, 150, 250),
            };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(reservations, r => r.ToList()));
        }

        [TestMethod]
        public void OnCountFilter_Always_ReturnsNextResult()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            FilterElement<TransponderRangeReservation> filter = null;
            var called = false;

            var result = sut.OnCount(filter, f =>
            {
                called = true;
                Assert.IsNull(f);
                return 42L;
            });

            Assert.IsTrue(called);
            Assert.AreEqual(42L, result);
        }

        [TestMethod]
        public void OnCountQuery_Always_ReturnsNextResult()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var query = new Mock<IQuery<TransponderRangeReservation>>().Object;
            IQuery<TransponderRangeReservation> received = null;

            var result = sut.OnCount(query, q =>
            {
                received = q;
                return 7L;
            });

            Assert.AreSame(query, received);
            Assert.AreEqual(7L, result);
        }

        [TestMethod]
        public void OnDeleteBulk_Always_CallsNextWithSameCollection()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservations = new[] { CreateReservation("A", 0, 2, 100, 200) };
            IEnumerable<TransponderRangeReservation> received = null;

            sut.OnDelete(reservations, r => received = r);

            Assert.AreSame(reservations, received);
        }

        [TestMethod]
        public void OnDeleteSingle_Always_CallsNextWithSameReservation()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var reservation = CreateReservation("A", 0, 2, 100, 200);
            TransponderRangeReservation received = null;

            sut.OnDelete(reservation, r => received = r);

            Assert.AreSame(reservation, received);
        }

        [TestMethod]
        public void OnReadFilter_Always_ReturnsNextResult()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var filter = new Mock<FilterElement<TransponderRangeReservation>>().Object;
            var expected = new[] { CreateReservation("A", 0, 2, 100, 200) };
            FilterElement<TransponderRangeReservation> received = null;

            var result = sut.OnRead(filter, f =>
            {
                received = f;
                return expected;
            });

            Assert.AreSame(filter, received);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadQuery_Always_ReturnsNextResult()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var query = new Mock<IQuery<TransponderRangeReservation>>().Object;
            var expected = new[] { CreateReservation("A", 0, 2, 100, 200) };
            IQuery<TransponderRangeReservation> received = null;

            var result = sut.OnRead(query, q =>
            {
                received = q;
                return expected;
            });

            Assert.AreSame(query, received);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedFilter_Always_ReturnsNextResult()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var filter = new Mock<FilterElement<TransponderRangeReservation>>().Object;
            var expected = new[] { new Mock<IPagedResult<TransponderRangeReservation>>().Object };
            FilterElement<TransponderRangeReservation> received = null;

            var result = sut.OnReadPaged(filter, f =>
            {
                received = f;
                return expected;
            });

            Assert.AreSame(filter, received);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedQuery_Always_ReturnsNextResult()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var query = new Mock<IQuery<TransponderRangeReservation>>().Object;
            var expected = new[] { new Mock<IPagedResult<TransponderRangeReservation>>().Object };
            IQuery<TransponderRangeReservation> received = null;

            var result = sut.OnReadPaged(query, q =>
            {
                received = q;
                return expected;
            });

            Assert.AreSame(query, received);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedFilterWithPageSize_Always_PassesFilterAndPageSizeToNext()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var filter = new Mock<FilterElement<TransponderRangeReservation>>().Object;
            var expected = new[] { new Mock<IPagedResult<TransponderRangeReservation>>().Object };
            FilterElement<TransponderRangeReservation> received = null;
            var receivedPageSize = 0;

            var result = sut.OnReadPaged(filter, 25, (f, size) =>
            {
                received = f;
                receivedPageSize = size;
                return expected;
            });

            Assert.AreSame(filter, received);
            Assert.AreEqual(25, receivedPageSize);
            Assert.AreSame(expected, result);
        }


        [TestMethod]
        public void OnReadPagedQueryWithPageSize_Always_PassesQueryAndPageSizeToNext()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var query = new Mock<IQuery<TransponderRangeReservation>>().Object;
            var expected = new[] { new Mock<IPagedResult<TransponderRangeReservation>>().Object };
            IQuery<TransponderRangeReservation> received = null;
            var receivedPageSize = 0;

            var result = sut.OnReadPaged(query, 50, (q, size) =>
            {
                received = q;
                receivedPageSize = size;
                return expected;
            });

            Assert.AreSame(query, received);
            Assert.AreEqual(50, receivedPageSize);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedQueryWithPageSize_NullQueryAndZeroPageSize_PassesValuesThrough()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var called = false;

            var result = sut.OnReadPaged((IQuery<TransponderRangeReservation>)null, 0, (q, size) =>
            {
                called = true;
                Assert.IsNull(q);
                Assert.AreEqual(0, size);
                return Enumerable.Empty<IPagedResult<TransponderRangeReservation>>();
            });

            Assert.IsTrue(called);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void OnReadPagedQueryWithPageSize_NullNext_ThrowsNullReferenceException()
        {
            var sut = CreateSut(id => Enumerable.Empty<TransponderRangeReservation>());
            var query = new Mock<IQuery<TransponderRangeReservation>>().Object;

            Assert.ThrowsException<NullReferenceException>(() => sut.OnReadPaged(
                query,
                10,
                (Func<IQuery<TransponderRangeReservation>, int, IEnumerable<IPagedResult<TransponderRangeReservation>>>)null));
        }

        private static TransponderRangeReservationOverlapValidationMiddleware CreateSut(Func<Guid, IEnumerable<TransponderRangeReservation>> resolver)
        {
            var repository = new Mock<ITransponderRangeReservationRepository>();
            repository.Setup(r => r.ReadByTransponder(It.IsAny<Guid>())).Returns(resolver);

            return new TransponderRangeReservationOverlapValidationMiddleware(repository.Object);
        }

        private static TransponderRangeReservation CreateReservation(string name, int startHour, int endHour, double startFrequency, double endFrequency, Guid? id = null)
        {
            var reservation = id.HasValue
                ? TransponderRangeReservation.CreateWithId(id.Value)
                : new TransponderRangeReservation();

            reservation.Name = name;
            reservation.Transponder = TransponderId;
            reservation.StartTime = BaseTime.AddHours(startHour);
            reservation.EndTime = BaseTime.AddHours(endHour);
            reservation.RelativeStartFrequency = startFrequency;
            reservation.RelativeEndFrequency = endFrequency;

            return reservation;
        }
    }
}
