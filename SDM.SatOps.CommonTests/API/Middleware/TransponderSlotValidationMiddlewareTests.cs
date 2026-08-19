namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using SLDataGateway.API.Types.Querying;

    /// <summary>
    /// Tests for the <c>TransponderSlotValidationMiddleware</c>.
    /// </summary>
    [TestClass]
    public class TransponderSlotValidationMiddlewareTests
    {
        private static readonly Guid PlanId = Guid.NewGuid();

        [TestMethod]
        public void Constructor_NullResolver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TransponderSlotValidationMiddleware(null));
        }

        [TestMethod]
        public void OnCreateSingle_NullSlot_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((TransponderSlot)null, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(CreateValidSlot(), (Func<TransponderSlot, TransponderSlot>)null));
        }

        [TestMethod]
        public void OnCreateSingle_ValidSlot_CallsNext()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
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
        public void OnCreateSingle_MissingTransponderPlan_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.TransponderPlan = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_EmptyTransponderPlan_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.TransponderPlan = Guid.Empty;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_MissingName_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.Name = "  ";

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_MissingStartFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.SlotStartFrequency = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_MissingEndFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.SlotEndFrequency = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_MissingBandwidth_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.Bandwidth = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_MissingUplinkFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.UplinkFreq = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_MissingDownlinkFrequency_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.DownlinkFreq = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateSingle_UnknownTransponderPlan_ThrowsArgumentException()
        {
            var sut = new TransponderSlotValidationMiddleware(id => null);
            var slot = CreateValidSlot();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(slot, s => s));
        }

        [TestMethod]
        public void OnCreateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<TransponderSlot>)null, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(
                new List<TransponderSlot> { CreateValidSlot() },
                (Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>)null));
        }

        [TestMethod]
        public void OnCreateBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(new TransponderSlot[] { null }, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var slots = new List<TransponderSlot> { CreateValidSlot(), CreateValidSlot() };
            IEnumerable<TransponderSlot> received = null;

            var result = sut.OnCreate(slots, s =>
            {
                received = s;
                return s.ToList();
            });

            Assert.AreSame(slots, received);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateSingle_NullSlot_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((TransponderSlot)null, s => s));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(CreateValidSlot(), (Func<TransponderSlot, TransponderSlot>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_InvalidSlot_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            slot.Name = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(slot, s => s));
        }

        [TestMethod]
        public void OnUpdateSingle_ValidSlot_CallsNext()
        {
            var sut = CreateSut();
            var slot = CreateValidSlot();
            var called = false;

            var result = sut.OnUpdate(slot, s =>
            {
                called = true;
                return s;
            });

            Assert.IsTrue(called);
            Assert.AreSame(slot, result);
        }

        [TestMethod]
        public void OnUpdateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<TransponderSlot>)null, s => s.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(
                new List<TransponderSlot> { CreateValidSlot() },
                (Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>>)null));
        }

        [TestMethod]
        public void OnUpdateBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(new TransponderSlot[] { null }, s => s.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var slots = new List<TransponderSlot> { CreateValidSlot() };
            IEnumerable<TransponderSlot> received = null;

            var result = sut.OnUpdate(slots, s =>
            {
                received = s;
                return s.ToList();
            });

            Assert.AreSame(slots, received);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(null, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(
                new List<TransponderSlot> { CreateValidSlot() },
                null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(new TransponderSlot[] { null }, s => s.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var slots = new List<TransponderSlot> { CreateValidSlot(), CreateValidSlot() };
            IEnumerable<TransponderSlot> received = null;

            var result = sut.OnCreateOrUpdate(slots, s =>
            {
                received = s;
                return s.ToList();
            });

            Assert.AreSame(slots, received);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_UnknownTransponderPlan_ThrowsArgumentException()
        {
            var sut = new TransponderSlotValidationMiddleware(id => null);

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(
                new List<TransponderSlot> { CreateValidSlot() },
                s => s.ToList()));
        }

        [TestMethod]
        public void OnCountFilter_Always_ReturnsNextResult()
        {
            var sut = CreateSut();

            var result = sut.OnCount((FilterElement<TransponderSlot>)null, f => 42L);

            Assert.AreEqual(42L, result);
        }

        [TestMethod]
        public void OnCountQuery_Always_ReturnsNextResult()
        {
            var sut = CreateSut();

            var result = sut.OnCount((IQuery<TransponderSlot>)null, q => 7L);

            Assert.AreEqual(7L, result);
        }

        [TestMethod]
        public void OnDeleteBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((IEnumerable<TransponderSlot>)null, s => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(
                new List<TransponderSlot> { CreateValidSlot() },
                (Action<IEnumerable<TransponderSlot>>)null));
        }

        [TestMethod]
        public void OnDeleteBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnDelete(new TransponderSlot[] { null }, s => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var slots = new List<TransponderSlot> { CreateValidSlot(), CreateValidSlot() };
            IEnumerable<TransponderSlot> received = null;

            sut.OnDelete(slots, s => received = s);

            Assert.AreSame(slots, received);
        }

        [TestMethod]
        public void OnDeleteSingle_NullSlot_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((TransponderSlot)null, s => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(CreateValidSlot(), (Action<TransponderSlot>)null));
        }

        [TestMethod]
        public void OnDeleteSingle_ValidSlot_CallsNextWithoutValidation()
        {
            var sut = new TransponderSlotValidationMiddleware(id => null);
            var slot = TransponderSlot.CreateNewTransponderSlot();
            TransponderSlot received = null;

            sut.OnDelete(slot, s => received = s);

            Assert.AreSame(slot, received);
        }

        [TestMethod]
        public void OnReadFilter_Always_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<TransponderSlot> { CreateValidSlot() };

            var result = sut.OnRead((FilterElement<TransponderSlot>)null, f => expected);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadQuery_Always_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<TransponderSlot> { CreateValidSlot() };
            IQuery<TransponderSlot> received = null;
            var query = new Mock<IQuery<TransponderSlot>>().Object;

            var result = sut.OnRead(query, q =>
            {
                received = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnReadPagedFilter_Always_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<TransponderSlot>>();
            FilterElement<TransponderSlot> received = null;

            var result = sut.OnReadPaged((FilterElement<TransponderSlot>)null, f =>
            {
                received = f;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.IsNull(received);
        }

        [TestMethod]
        public void OnReadPagedQuery_Always_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<TransponderSlot>>();
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            IQuery<TransponderSlot> received = null;

            var result = sut.OnReadPaged(query, q =>
            {
                received = q;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
        }

        [TestMethod]
        public void OnReadPagedFilterWithPageSize_Always_PassesPageSizeToNext()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<TransponderSlot>>();
            var receivedPageSize = 0;

            var result = sut.OnReadPaged((FilterElement<TransponderSlot>)null, 42, (f, size) =>
            {
                receivedPageSize = size;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreEqual(42, receivedPageSize);
        }

        [TestMethod]
        public void OnReadPagedQueryWithPageSize_Always_PassesQueryAndPageSizeToNext()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<TransponderSlot>>();
            var query = new Mock<IQuery<TransponderSlot>>().Object;
            IQuery<TransponderSlot> received = null;
            var receivedPageSize = 0;

            var result = sut.OnReadPaged(query, 7, (q, size) =>
            {
                received = q;
                receivedPageSize = size;
                return expected;
            });

            Assert.AreSame(expected, result);
            Assert.AreSame(query, received);
            Assert.AreEqual(7, receivedPageSize);
        }

        private static TransponderSlotValidationMiddleware CreateSut()
        {
            return new TransponderSlotValidationMiddleware(id => TransponderPlan.CreateNewTransponderPlan());
        }

        private static TransponderSlot CreateValidSlot()
        {
            var slot = TransponderSlot.CreateNewTransponderSlot();
            slot.TransponderPlan = PlanId;
            slot.Name = "Slot";
            slot.SlotStartFrequency = 100;
            slot.SlotEndFrequency = 200;
            slot.Bandwidth = 100;
            slot.UplinkFreq = 14000;
            slot.DownlinkFreq = 12000;
            return slot;
        }
    }
}
