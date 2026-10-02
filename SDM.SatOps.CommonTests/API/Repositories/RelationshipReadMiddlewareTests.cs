namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class RelationshipReadMiddlewareTests
    {
        [TestMethod]
        public void PlanReadByTransponders_DelegatesToInner()
        {
            var ids = new[] { Guid.NewGuid() };
            var plans = new[] { new TransponderPlan() };
            var inner = new Mock<ITransponderPlanRepository>();
            inner.Setup(x => x.ReadByTransponders(ids)).Returns(plans);

            var result = new TransponderPlanRepositoryMiddleware(inner.Object, null).ReadByTransponders(ids);

            CollectionAssert.AreEqual(plans, result.ToArray());
            inner.Verify(x => x.ReadByTransponders(ids), Times.Once);
        }

        [TestMethod]
        public void SlotReadByTransponderPlans_DelegatesToInner()
        {
            var ids = new[] { Guid.NewGuid() };
            var slots = new[] { new TransponderSlot() };
            var inner = new Mock<ITransponderSlotRepository>();
            inner.Setup(x => x.ReadByTransponderPlans(ids)).Returns(slots);

            var result = new TransponderSlotRepositoryMiddleware(inner.Object, null).ReadByTransponderPlans(ids);

            CollectionAssert.AreEqual(slots, result.ToArray());
            inner.Verify(x => x.ReadByTransponderPlans(ids), Times.Once);
        }

        [TestMethod]
        public void RowReadByTransponderPlan_DelegatesToInner()
        {
            var id = Guid.NewGuid();
            var rows = new[] { new TransponderPlanRow() };
            var inner = new Mock<ITransponderPlanRowRepository>();
            inner.Setup(x => x.ReadByTransponderPlan(id)).Returns(rows);

            var result = new TransponderPlanRowRepositoryMiddleware(inner.Object, null).ReadByTransponderPlan(id);

            CollectionAssert.AreEqual(rows, result.ToArray());
            inner.Verify(x => x.ReadByTransponderPlan(id), Times.Once);
        }

        [TestMethod]
        public void BeamReadBySatellite_DelegatesToInner()
        {
            var id = Guid.NewGuid();
            var beams = new[] { new Beam() };
            var inner = new Mock<IBeamRepository>();
            inner.Setup(x => x.ReadBySatellite(id)).Returns(beams);

            var result = new BeamRepositoryMiddleware(inner.Object, null).ReadBySatellite(id);

            CollectionAssert.AreEqual(beams, result.ToArray());
            inner.Verify(x => x.ReadBySatellite(id), Times.Once);
        }
    }
}
