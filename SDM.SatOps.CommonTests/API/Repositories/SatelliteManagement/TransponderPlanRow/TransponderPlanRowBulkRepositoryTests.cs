namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.TransponderPlanRow
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class TransponderPlanRowBulkRepositoryTests
    {
        [TestMethod]
        public void Create_EmptyBatch_ReturnsEmptyWithoutContactingDom()
        {
            var connection = new Mock<IConnection>();
            var repository = new TransponderPlanRowRepository(new SatOpsApi(connection.Object));

            var result = repository.Create(Array.Empty<TransponderPlanRow>());

            Assert.AreEqual(0, result.Count);
            connection.VerifyGet(x => x.IsShuttingDown, Times.Once);
            connection.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Create_DuplicateIdInBatch_FailsBeforePersisting()
        {
            var connection = new Mock<IConnection>();
            var repository = new TransponderPlanRowRepository(new SatOpsApi(connection.Object));
            var row = new TransponderPlanRow();

            Assert.ThrowsException<InvalidOperationException>(() => repository.Create(new[] { row, row }));
            connection.VerifyGet(x => x.IsShuttingDown, Times.Once);
            connection.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Create_NullBatch_Throws()
        {
            var repository = CreateRepository();

            Assert.ThrowsException<ArgumentNullException>(() => repository.Create((IEnumerable<TransponderPlanRow>)null));
        }

        [TestMethod]
        public void Delete_EmptyBatches_DoNotContactDom()
        {
            var connection = new Mock<IConnection>();
            var repository = new TransponderPlanRowRepository(new SatOpsApi(connection.Object));

            repository.Delete(Array.Empty<Guid>());
            repository.Delete(Array.Empty<TransponderPlanRow>());

            connection.VerifyGet(x => x.IsShuttingDown, Times.Once);
            connection.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Delete_EmptyGuidInBatch_ThrowsBeforeDeleting()
        {
            var repository = CreateRepository();

            Assert.ThrowsException<ArgumentException>(() => repository.Delete(new[] { Guid.NewGuid(), Guid.Empty }));
        }

        [TestMethod]
        public void Delete_NullRowInBatch_ThrowsBeforeDeleting()
        {
            var repository = CreateRepository();

            Assert.ThrowsException<ArgumentException>(() => repository.Delete(new TransponderPlanRow[] { new TransponderPlanRow(), null }));
        }

        private static TransponderPlanRowRepository CreateRepository()
        {
            return new TransponderPlanRowRepository(new SatOpsApi(new Mock<IConnection>().Object));
        }
    }
}
