namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>SlotOverlapValidationMiddleware</c> validation behavior.
    /// </summary>
    [TestClass]
    public class SlotOverlapValidationMiddlewareTests
    {
        private static readonly Guid PlanId = Guid.NewGuid();

        [TestMethod]
        public void Constructor_NullResolver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SlotOverlapValidationMiddleware(null));
        }

        [TestMethod]
        public void Constructor_ValidResolver_CreatesInstance()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void OnCreateSingle_NullSlot_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((TransponderSlot)null, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slot = CreateSlot("A", 100, 200, PlanId);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(slot, (Func<TransponderSlot, TransponderSlot>)null));
        }

        [TestMethod]
        public void OnCreateSingle_NoOverlap_CallsNext()
        {
            var existing = CreateSlot("Existing", 200, 300, PlanId);
            var sut = new SlotOverlapValidationMiddleware(id => new[] { existing });
            var slot = CreateSlot("New", 100, 200, PlanId);
            var called = false;

            var result = sut.OnCreate(slot, s =>
            {
                called = true;
                return s;
            });

            Assert.IsTrue(called);
            Assert.AreSame(slot, result);
        }

        [TestMethod]
        public void OnCreateSingle_OverlappingSlot_ThrowsArgumentException()
        {
            var existing = CreateSlot("Existing", 150, 300, PlanId);
            var sut = new SlotOverlapValidationMiddleware(id => new[] { existing });
            var slot = CreateSlot("New", 100, 200, PlanId);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_NoPlan_SkipsValidation()
        {
            var resolverCalled = false;
            var sut = new SlotOverlapValidationMiddleware(id =>
            {
                resolverCalled = true;
                return Enumerable.Empty<TransponderSlot>();
            });
            var slot = CreateSlot("New", 100, 200, null);

            var result = sut.OnCreate(slot, s => s);

            Assert.IsFalse(resolverCalled);
            Assert.AreSame(slot, result);
        }

        [TestMethod]
        public void OnCreateSingle_EmptyPlanId_SkipsValidation()
        {
            var resolverCalled = false;
            var sut = new SlotOverlapValidationMiddleware(id =>
            {
                resolverCalled = true;
                return Enumerable.Empty<TransponderSlot>();
            });
            var slot = CreateSlot("New", 100, 200, Guid.Empty);

            sut.OnCreate(slot, s => s);

            Assert.IsFalse(resolverCalled);
        }

        [TestMethod]
        public void OnCreateSingle_OpenEndedFrequencies_SkipsValidation()
        {
            var resolverCalled = false;
            var sut = new SlotOverlapValidationMiddleware(id =>
            {
                resolverCalled = true;
                return Enumerable.Empty<TransponderSlot>();
            });
            var slot = CreateSlot("New", null, 200, PlanId);

            sut.OnCreate(slot, s => s);

            Assert.IsFalse(resolverCalled);

            var slot2 = CreateSlot("New2", 100, null, PlanId);
            sut.OnCreate(slot2, s => s);

            Assert.IsFalse(resolverCalled);
        }

        [TestMethod]
        public void OnCreateSingle_ResolverReturnsNull_CallsNext()
        {
            var sut = new SlotOverlapValidationMiddleware(id => null);
            var slot = CreateSlot("New", 100, 200, PlanId);

            var result = sut.OnCreate(slot, s => s);

            Assert.AreSame(slot, result);
        }

        [TestMethod]
        public void OnCreateSingle_ExistingWithoutFrequencies_Ignored()
        {
            var existing = CreateSlot("Existing", null, null, PlanId);
            var sut = new SlotOverlapValidationMiddleware(id => new[] { null, existing });
            var slot = CreateSlot("New", 100, 200, PlanId);

            var result = sut.OnCreate(slot, s => s);

            Assert.AreSame(slot, result);
        }

        [TestMethod]
        public void OnUpdateSingle_NullSlot_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((TransponderSlot)null, s => s));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slot = CreateSlot("A", 100, 200, PlanId);

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(slot, (Func<TransponderSlot, TransponderSlot>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_ExcludesItself_CallsNext()
        {
            var slot = CreateSlot("Slot", 100, 200, PlanId);
            var sut = new SlotOverlapValidationMiddleware(id => new[] { slot });

            var result = sut.OnUpdate(slot, s => s);

            Assert.AreSame(slot, result);
        }

        [TestMethod]
        public void OnUpdateSingle_OverlappingOtherSlot_ThrowsArgumentException()
        {
            var existing = CreateSlot("Existing", 150, 300, PlanId);
            var sut = new SlotOverlapValidationMiddleware(id => new[] { existing });
            var slot = CreateSlot("Slot", 100, 200, PlanId);

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<TransponderSlot>)null, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(
                new List<TransponderSlot>(),
                (Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>)null));
        }

        [TestMethod]
        public void OnCreateBulk_BoundaryTouchingSlots_CallsNext()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot>
            {
                CreateSlot("A", 100, 200, PlanId),
                CreateSlot("B", 200, 300, PlanId),
            };

            var result = sut.OnCreate(slots, s => s.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateBulk_OverlappingSlots_ThrowsArgumentException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot>
            {
                CreateSlot("A", 100, 250, PlanId),
                CreateSlot("B", 200, 300, PlanId),
            };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slots, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_DuplicateNames_ThrowsArgumentException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot>
            {
                CreateSlot("Same", 100, 200, PlanId),
                CreateSlot("same", 200, 300, PlanId),
            };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slots, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_DifferentPlans_NoOverlapDetected()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot>
            {
                CreateSlot("A", 100, 250, PlanId),
                CreateSlot("B", 200, 300, Guid.NewGuid()),
                CreateSlot(null, 400, 500, PlanId),
                null,
                CreateSlot("C", 100, 200, Guid.Empty),
                CreateSlot("D", null, null, PlanId),
            };

            var result = sut.OnCreate(slots, s => s.ToList());

            Assert.AreEqual(6, result.Count);
        }

        [TestMethod]
        public void OnUpdateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<TransponderSlot>)null, s => s.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(
                new List<TransponderSlot>(),
                (Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>)null));
        }

        [TestMethod]
        public void OnUpdateBulk_NoOverlap_CallsNext()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot>
            {
                CreateSlot("A", 100, 200, PlanId),
                CreateSlot("B", 300, 400, PlanId),
            };

            var result = sut.OnUpdate(slots, s => s.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateBulk_OverlappingSlots_ThrowsArgumentException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot>
            {
                CreateSlot("A", 100, 350, PlanId),
                CreateSlot("B", 300, 400, PlanId),
            };

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(slots, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullCollection_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(null, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullNext_ThrowsArgumentNullException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(
                new List<TransponderSlot>(),
                null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NoOverlap_CallsNext()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot>
            {
                CreateSlot("A", 100, 200, PlanId),
                CreateSlot("B", 200, 300, PlanId),
            };
            var called = false;

            var result = sut.OnCreateOrUpdate(slots, s =>
            {
                called = true;
                return s.ToList();
            });

            Assert.IsTrue(called);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_OverlappingSlots_ThrowsArgumentException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot>
            {
                CreateSlot("A", 100, 250, PlanId),
                CreateSlot("B", 200, 300, PlanId),
            };

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(slots, s => s.ToList()));
        }

        [TestMethod]
        public void OnCountWithFilter_Always_ReturnsNextResult()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            FilterElement<TransponderSlot> received = null;

            var result = sut.OnCount(
                (FilterElement<TransponderSlot>)null,
                f =>
                {
                    received = f;
                    return 42L;
                });

            Assert.AreEqual(42L, result);
            Assert.IsNull(received);
        }

        [TestMethod]
        public void OnCountWithQuery_Always_ReturnsNextResult()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            IQuery<TransponderSlot> received = null;

            var result = sut.OnCount(
                query,
                q =>
                {
                    received = q;
                    return 7L;
                });

            Assert.AreEqual(7L, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnDeleteBulk_Always_CallsNextWithSameCollection()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slots = new List<TransponderSlot> { CreateSlot("A", 100, 200, PlanId) };
            IEnumerable<TransponderSlot> received = null;

            sut.OnDelete(slots, s => received = s);

            Assert.AreSame(slots, received);
        }

        [TestMethod]
        public void OnDeleteSingle_Always_CallsNextWithSameSlot()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var slot = CreateSlot("A", 100, 200, PlanId);
            TransponderSlot received = null;

            sut.OnDelete(slot, s => received = s);

            Assert.AreSame(slot, received);
        }

        [TestMethod]
        public void OnDeleteSingle_NullSlot_CallsNextWithNull()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var called = false;

            sut.OnDelete((TransponderSlot)null, s =>
            {
                called = true;
                Assert.IsNull(s);
            });

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void OnReadWithFilter_Always_ReturnsNextResult()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var expected = new[] { CreateSlot("A", 100, 200, PlanId) };
            FilterElement<TransponderSlot> received = null;

            var result = sut.OnRead(
                (FilterElement<TransponderSlot>)null,
                f =>
                {
                    received = f;
                    return expected;
                });

            Assert.AreSame(expected, result);
            Assert.IsNull(received);
        }

        [TestMethod]
        public void OnReadWithQuery_Always_ReturnsNextResult()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new[] { CreateSlot("A", 100, 200, PlanId) };
            IQuery<TransponderSlot> received = null;

            var result = sut.OnRead(
                query,
                q =>
                {
                    received = q;
                    return expected;
                });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnReadPagedWithFilter_Always_ReturnsNextResult()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var expected = new[] { new Mock<IPagedResult<TransponderSlot>>().Object };
            var called = false;

            var result = sut.OnReadPaged(
                (FilterElement<TransponderSlot>)null,
                f =>
                {
                    called = true;
                    Assert.IsNull(f);
                    return expected;
                });

            Assert.IsTrue(called);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedWithQuery_Always_ReturnsNextResult()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new[] { new Mock<IPagedResult<TransponderSlot>>().Object };
            IQuery<TransponderSlot> received = null;

            var result = sut.OnReadPaged(
                query,
                q =>
                {
                    received = q;
                    return expected;
                });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnReadPagedWithFilterAndPageSize_Always_ForwardsPageSize()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var expected = new[] { new Mock<IPagedResult<TransponderSlot>>().Object };
            var receivedPageSize = 0;

            var result = sut.OnReadPaged(
                (FilterElement<TransponderSlot>)null,
                25,
                (f, p) =>
                {
                    receivedPageSize = p;
                    Assert.IsNull(f);
                    return expected;
                });

            Assert.AreEqual(25, receivedPageSize);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedWithQueryAndPageSize_Always_ForwardsQueryAndPageSize()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            var expected = new[] { new Mock<IPagedResult<TransponderSlot>>().Object };
            IQuery<TransponderSlot> receivedQuery = null;
            var receivedPageSize = 0;

            var result = sut.OnReadPaged(
                query,
                50,
                (q, p) =>
                {
                    receivedQuery = q;
                    receivedPageSize = p;
                    return expected;
                });

            Assert.AreSame(query, receivedQuery);
            Assert.AreEqual(50, receivedPageSize);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedWithQueryAndPageSize_NullQuery_ForwardsNull()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var called = false;

            var result = sut.OnReadPaged(
                (IQuery<TransponderSlot>)null,
                0,
                (q, p) =>
                {
                    called = true;
                    Assert.IsNull(q);
                    Assert.AreEqual(0, p);
                    return null;
                });

            Assert.IsTrue(called);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void OnReadPagedWithQueryAndPageSize_NullNext_ThrowsNullReferenceException()
        {
            var sut = new SlotOverlapValidationMiddleware(id => Enumerable.Empty<TransponderSlot>());
            var query = new Mock<IQuery<TransponderSlot>>().Object;

            Assert.ThrowsException<NullReferenceException>(() => sut.OnReadPaged(
                query,
                10,
                (Func<IQuery<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>>)null));
        }

        private static TransponderSlot CreateSlot(string name, double? start, double? end, Guid? planId)
        {
            var slot = TransponderSlot.CreateNewTransponderSlot();
            slot.Name = name;
            slot.SlotStartFrequency = start;
            slot.SlotEndFrequency = end;
            slot.TransponderPlan = planId;
            return slot;
        }
    }
}
