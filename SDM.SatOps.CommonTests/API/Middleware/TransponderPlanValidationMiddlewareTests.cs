namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using SLDataGateway.API.Types.Querying;

    using DomModel = Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;

    /// <summary>
    /// Tests for the <c>TransponderPlanValidationMiddleware</c>.
    /// </summary>
    [TestClass]
    public class TransponderPlanValidationMiddlewareTests
    {
        [TestMethod]
        public void OnCreateSingle_ValidPlan_CallsNext()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            var called = false;

            var result = sut.OnCreate(plan, p =>
            {
                called = true;
                return p;
            });

            Assert.IsTrue(called);
            Assert.AreSame(plan, result);
        }

        [TestMethod]
        public void OnCreateSingle_NullPlan_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((TransponderPlan)null, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(CreateValidPlan(), (Func<TransponderPlan, TransponderPlan>)null));
        }

        [TestMethod]
        public void OnCreateSingle_MissingName_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            plan.Name = "  ";

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_MissingSlotSize_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            plan.DefaultSlotSize = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_ZeroSlotSize_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            plan.DefaultSlotSize = 0;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_EmptyTransponder_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            plan.Transponder = Guid.Empty;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_UnknownTransponder_ThrowsArgumentException()
        {
            var sut = new TransponderPlanValidationMiddleware(CreateTransponderRepository(null), CreatePlanRepository(Enumerable.Empty<TransponderPlan>()));
            var plan = CreateValidPlan();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_MissingStartTime_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            plan.StartTime = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_MissingEndTime_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            plan.EndTime = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_StartTimeNotBeforeEndTime_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            plan.EndTime = plan.StartTime;

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_PermanentPlanWithoutExistingPermanent_CallsNext()
        {
            var plan = CreateValidPlan();
            plan.IsPermanent = true;
            plan.StartTime = null;
            plan.EndTime = null;

            var sut = CreateSut(Enumerable.Empty<TransponderPlan>());

            var result = sut.OnCreate(plan, p => p);

            Assert.AreSame(plan, result);
        }

        [TestMethod]
        public void OnCreateSingle_PermanentPlan_SetsFullTimeWindow()
        {
            var plan = CreateValidPlan();
            plan.IsPermanent = true;
            plan.StartTime = null;
            plan.EndTime = null;

            var sut = CreateSut(Enumerable.Empty<TransponderPlan>());

            TransponderPlan persisted = null;
            sut.OnCreate(plan, p =>
            {
                persisted = p;
                return p;
            });

            Assert.AreEqual(DateTime.MinValue, persisted.StartTime);
            Assert.AreEqual(DateTime.MaxValue, persisted.EndTime);
        }

        [TestMethod]
        public void OnCreateSingle_PermanentPlanWithCustomTimeWindow_OverwritesTimeWindow()
        {
            var plan = CreateValidPlan();
            plan.IsPermanent = true;

            var sut = CreateSut(Enumerable.Empty<TransponderPlan>());

            sut.OnCreate(plan, p => p);

            Assert.AreEqual(DateTime.MinValue, plan.StartTime);
            Assert.AreEqual(DateTime.MaxValue, plan.EndTime);
        }

        [TestMethod]
        public void OnCreateSingle_NonPermanentPlan_KeepsTimeWindow()
        {
            var plan = CreateValidPlan();
            plan.IsPermanent = false;

            var sut = CreateSut(Enumerable.Empty<TransponderPlan>());

            sut.OnCreate(plan, p => p);

            Assert.AreEqual(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), plan.StartTime);
            Assert.AreEqual(new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc), plan.EndTime);
        }

        [TestMethod]
        public void OnCreateBulk_PermanentPlan_SetsFullTimeWindow()
        {
            var permanentPlan = CreateValidPlan();
            permanentPlan.IsPermanent = true;
            permanentPlan.StartTime = null;
            permanentPlan.EndTime = null;

            var nonPermanentPlan = CreateValidPlan();

            var sut = CreateSut(Enumerable.Empty<TransponderPlan>());

            sut.OnCreate(new[] { permanentPlan, nonPermanentPlan }, p => p.ToList());

            Assert.AreEqual(DateTime.MinValue, permanentPlan.StartTime);
            Assert.AreEqual(DateTime.MaxValue, permanentPlan.EndTime);
            Assert.AreEqual(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), nonPermanentPlan.StartTime);
            Assert.AreEqual(new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc), nonPermanentPlan.EndTime);
        }

        [TestMethod]
        public void OnCreateBulk_LazySequenceWithPermanentPlan_ForwardsNormalizedPlans()
        {
            var plan = CreateValidPlan();
            plan.IsPermanent = true;
            plan.StartTime = null;
            plan.EndTime = null;

            var sut = CreateSut(Enumerable.Empty<TransponderPlan>());

            var result = sut.OnCreate(new[] { plan }.Select(p => p), p => p.ToList());

            var persisted = result.Single();
            Assert.AreEqual(DateTime.MinValue, persisted.StartTime);
            Assert.AreEqual(DateTime.MaxValue, persisted.EndTime);
        }

        [TestMethod]
        public void OnUpdateSingle_PermanentPlan_SetsFullTimeWindow()
        {
            var plan = CreateValidPlan();
            plan.IsPermanent = true;

            var sut = CreateSut(Enumerable.Empty<TransponderPlan>());

            sut.OnUpdate(plan, p => p);

            Assert.AreEqual(DateTime.MinValue, plan.StartTime);
            Assert.AreEqual(DateTime.MaxValue, plan.EndTime);
        }

        [TestMethod]
        public void OnCreateOrUpdate_PermanentPlan_SetsFullTimeWindow()
        {
            var plan = CreateValidPlan();
            plan.IsPermanent = true;
            plan.StartTime = null;
            plan.EndTime = null;

            var sut = CreateSut(Enumerable.Empty<TransponderPlan>());

            sut.OnCreateOrUpdate(new[] { plan }, p => p.ToList());

            Assert.AreEqual(DateTime.MinValue, plan.StartTime);
            Assert.AreEqual(DateTime.MaxValue, plan.EndTime);
        }

        [TestMethod]
        public void OnCreateSingle_PermanentPlanWithExistingPermanent_ThrowsArgumentException()
        {
            var plan = CreateValidPlan();
            plan.IsPermanent = true;

            var existing = CreateActivePlan();
            existing.IsPermanent = true;

            var sut = CreateSut(new[] { existing });

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_OverlappingExistingPlan_ThrowsArgumentException()
        {
            var plan = CreateValidPlan();
            plan.StartTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            plan.EndTime = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc);

            var existing = CreateActivePlan();
            existing.StartTime = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc);
            existing.EndTime = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);

            var sut = CreateSut(new[] { existing });

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(plan, p => p));
        }

        [TestMethod]
        public void OnCreateSingle_BoundaryTouchingExistingPlan_CallsNext()
        {
            var plan = CreateValidPlan();
            plan.StartTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            plan.EndTime = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc);

            var existing = CreateActivePlan();
            existing.StartTime = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc);
            existing.EndTime = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);

            var sut = CreateSut(new[] { existing });

            var result = sut.OnCreate(plan, p => p);

            Assert.AreSame(plan, result);
        }

        [TestMethod]
        public void OnCreateSingle_ExistingPlanWithOpenEndedRange_IsIgnored()
        {
            var plan = CreateValidPlan();
            plan.StartTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            plan.EndTime = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc);

            var existing = CreateActivePlan();
            existing.StartTime = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);
            existing.EndTime = null;

            var sut = CreateSut(new[] { existing });

            var result = sut.OnCreate(plan, p => p);

            Assert.AreSame(plan, result);
        }

        [TestMethod]
        public void OnCreateSingle_NullResolvedPlans_CallsNext()
        {
            var plan = CreateValidPlan();
            var sut = new TransponderPlanValidationMiddleware(CreateTransponderRepository(new Transponder()), CreatePlanRepository(null));

            var result = sut.OnCreate(plan, p => p);

            Assert.AreSame(plan, result);
        }

        [TestMethod]
        public void OnCreateBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var plans = new[] { CreateValidPlan() };
            var called = false;

            var result = sut.OnCreate(plans, p =>
            {
                called = true;
                return p.ToList();
            });

            Assert.IsTrue(called);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void OnCreateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate((IEnumerable<TransponderPlan>)null, p => p.ToList()));
        }

        [TestMethod]
        public void OnCreateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreate(new[] { CreateValidPlan() }, (Func<IEnumerable<TransponderPlan>, IReadOnlyCollection<TransponderPlan>>)null));
        }

        [TestMethod]
        public void OnCreateBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreate(new TransponderPlan[] { null }, p => p.ToList()));
        }

        [TestMethod]
        public void OnUpdateSingle_ValidPlan_CallsNext()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();

            var result = sut.OnUpdate(plan, p => p);

            Assert.AreSame(plan, result);
        }

        [TestMethod]
        public void OnUpdateSingle_NullPlan_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((TransponderPlan)null, p => p));
        }

        [TestMethod]
        public void OnUpdateSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(CreateValidPlan(), (Func<TransponderPlan, TransponderPlan>)null));
        }

        [TestMethod]
        public void OnUpdateSingle_InvalidPlan_ThrowsArgumentException()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            plan.Name = null;

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(plan, p => p));
        }

        [TestMethod]
        public void OnUpdateBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var plans = new[] { CreateValidPlan(), CreateValidPlan() };

            var result = sut.OnUpdate(plans, p => p.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void OnUpdateBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate((IEnumerable<TransponderPlan>)null, p => p.ToList()));
        }

        [TestMethod]
        public void OnUpdateBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnUpdate(new[] { CreateValidPlan() }, (Func<IEnumerable<TransponderPlan>, IReadOnlyCollection<TransponderPlan>>)null));
        }

        [TestMethod]
        public void OnUpdateBulk_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnUpdate(new TransponderPlan[] { null }, p => p.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var called = false;

            var result = sut.OnCreateOrUpdate(new[] { CreateValidPlan() }, p =>
            {
                called = true;
                return p.ToList();
            });

            Assert.IsTrue(called);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(null, p => p.ToList()));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnCreateOrUpdate(new[] { CreateValidPlan() }, null));
        }

        [TestMethod]
        public void OnCreateOrUpdate_NullItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnCreateOrUpdate(new TransponderPlan[] { null }, p => p.ToList()));
        }

        [TestMethod]
        public void OnCountFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            FilterElement<TransponderPlan> captured = null;

            var result = sut.OnCount((FilterElement<TransponderPlan>)null, f =>
            {
                captured = f;
                return 42L;
            });

            Assert.AreEqual(42L, result);
            Assert.IsNull(captured);
        }

        [TestMethod]
        public void OnCountQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = CreateSut();

            var result = sut.OnCount((IQuery<TransponderPlan>)null, q => 7L);

            Assert.AreEqual(7L, result);
        }

        [TestMethod]
        public void OnReadFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new[] { CreateValidPlan() };

            var result = sut.OnRead((FilterElement<TransponderPlan>)null, f => expected);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnDeleteSingle_ValidPlan_CallsNext()
        {
            var sut = CreateSut();
            var plan = CreateValidPlan();
            TransponderPlan captured = null;

            sut.OnDelete(plan, p => captured = p);

            Assert.AreSame(plan, captured);
        }

        [TestMethod]
        public void OnDeleteSingle_NullPlan_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((TransponderPlan)null, p => { }));
        }

        [TestMethod]
        public void OnDeleteSingle_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(CreateValidPlan(), (Action<TransponderPlan>)null));
        }

        [TestMethod]
        public void OnDeleteBulk_ValidCollection_CallsNext()
        {
            var sut = CreateSut();
            var plans = new[] { CreateValidPlan(), CreateValidPlan() };
            IEnumerable<TransponderPlan> captured = null;

            sut.OnDelete(plans, p => captured = p);

            Assert.AreSame(plans, captured);
        }

        [TestMethod]
        public void OnDeleteBulk_NullCollection_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete((IEnumerable<TransponderPlan>)null, p => { }));
        }

        [TestMethod]
        public void OnDeleteBulk_NullNext_ThrowsArgumentNullException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentNullException>(() => sut.OnDelete(new[] { CreateValidPlan() }, (Action<IEnumerable<TransponderPlan>>)null));
        }

        [TestMethod]
        public void OnDeleteBulk_InvalidItemInCollection_ThrowsArgumentException()
        {
            var sut = CreateSut();

            Assert.ThrowsException<ArgumentException>(() => sut.OnDelete(new TransponderPlan[] { null }, p => { }));
        }

        [TestMethod]
        public void OnReadQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new[] { CreateValidPlan() };

            var result = sut.OnRead((IQuery<TransponderPlan>)null, q => expected);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedFilter_ValidFilter_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<TransponderPlan>>();

            var result = sut.OnReadPaged((FilterElement<TransponderPlan>)null, f => expected);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedQuery_ValidQuery_ReturnsNextResult()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<TransponderPlan>>();

            var result = sut.OnReadPaged((IQuery<TransponderPlan>)null, q => expected);

            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedFilterWithPageSize_ValidFilter_PassesPageSizeToNext()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<TransponderPlan>>();
            var capturedPageSize = 0;

            var result = sut.OnReadPaged((FilterElement<TransponderPlan>)null, 25, (f, size) =>
            {
                capturedPageSize = size;
                return expected;
            });

            Assert.AreEqual(25, capturedPageSize);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void OnReadPagedQueryWithPageSize_ValidQuery_PassesPageSizeToNext()
        {
            var sut = CreateSut();
            var expected = new List<IPagedResult<TransponderPlan>>();
            var capturedPageSize = 0;

            var result = sut.OnReadPaged((IQuery<TransponderPlan>)null, 50, (q, size) =>
            {
                capturedPageSize = size;
                return expected;
            });

            Assert.AreEqual(50, capturedPageSize);
            Assert.AreSame(expected, result);
        }

        private static TransponderPlanValidationMiddleware CreateSut()
        {
            return new TransponderPlanValidationMiddleware(CreateTransponderRepository(new Transponder()), CreatePlanRepository(Enumerable.Empty<TransponderPlan>()));
        }

        [TestMethod]
        public void OnCreateBulk_PlansOnSameTransponder_ReadsTransponderAndExistingPlansOnlyOnce()
        {
            var transponderId = Guid.NewGuid();

            var transponderRepository = new Mock<ITransponderRepository>();
            transponderRepository.Setup(r => r.Read(It.IsAny<IEnumerable<Guid>>()))
                .Returns((IEnumerable<Guid> ids) => ids.Select(CreateTransponderWithId).ToList());

            var planRepository = new Mock<ITransponderPlanRepository>();
            planRepository.Setup(r => r.ReadByTransponder(It.IsAny<Guid>())).Returns(Enumerable.Empty<TransponderPlan>());

            var sut = new TransponderPlanValidationMiddleware(transponderRepository.Object, planRepository.Object);

            var plans = new List<TransponderPlan> { CreateValidPlan(), CreateValidPlan(), CreateValidPlan() };
            foreach (var plan in plans)
                plan.Transponder = transponderId;

            sut.OnCreate((IEnumerable<TransponderPlan>)plans, p => p.ToList());

            // The transponder existence check is batched and the persisted plans are read once per distinct transponder.
            transponderRepository.Verify(r => r.Read(It.IsAny<IEnumerable<Guid>>()), Times.Once);
            transponderRepository.Verify(r => r.Read(It.IsAny<Guid>()), Times.Never);
            planRepository.Verify(r => r.ReadByTransponder(transponderId), Times.Once);
        }

        private static TransponderPlanValidationMiddleware CreateSut(IEnumerable<TransponderPlan> existingPlans)
        {
            return new TransponderPlanValidationMiddleware(CreateTransponderRepository(new Transponder()), CreatePlanRepository(existingPlans));
        }

        private static ITransponderRepository CreateTransponderRepository(Transponder transponder)
        {
            var repository = new Mock<ITransponderRepository>();
            repository.Setup(r => r.Read(It.IsAny<Guid>())).Returns(transponder);

            // The bulk paths resolve every referenced transponder with a single batch read, so the requested
            // identifiers are echoed back as existing transponders.
            repository.Setup(r => r.Read(It.IsAny<IEnumerable<Guid>>()))
                .Returns((IEnumerable<Guid> ids) => transponder != null
                    ? ids.Select(CreateTransponderWithId).ToList()
                    : new List<Transponder>());

            return repository.Object;
        }

        private static Transponder CreateTransponderWithId(Guid id)
        {
            var domInstance = new DomInstance
            {
                ID = new DomInstanceId(id) { ModuleId = DomModel.SlcSatellite_ManagementIds.ModuleId },
                DomDefinitionId = DomModel.SlcSatellite_ManagementIds.Definitions.Transponders,
            };

            return Transponder.FromInstance(new DomModel.TranspondersInstance(domInstance));
        }

        private static ITransponderPlanRepository CreatePlanRepository(IEnumerable<TransponderPlan> existingPlans)
        {
            var repository = new Mock<ITransponderPlanRepository>();
            repository.Setup(r => r.ReadByTransponder(It.IsAny<Guid>())).Returns(existingPlans);
            return repository.Object;
        }

        private static TransponderPlan CreateValidPlan()
        {
            var plan = new TransponderPlan();
            plan.Name = "Plan";
            plan.DefaultSlotSize = 10;
            plan.Transponder = Guid.NewGuid();
            plan.StartTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            plan.EndTime = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            return plan;
        }

        private static TransponderPlan CreateActivePlan()
        {
            var domInstance = new DomInstance
            {
                DomDefinitionId = DomModel.SlcSatellite_ManagementIds.Definitions.TransponderPlans,
                StatusId = DomModel.SlcSatellite_ManagementIds.Behaviors.TransponderPlansBehavior.Statuses.Active,
            };

            var plan = TransponderPlan.FromInstance(new DomModel.TransponderPlansInstance(domInstance));
            plan.Name = "Existing";
            plan.DefaultSlotSize = 10;
            plan.Transponder = Guid.NewGuid();
            plan.StartTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            plan.EndTime = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            return plan;
        }
    }
}
