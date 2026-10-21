namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class RelationshipReadHelperTests
    {
        [TestMethod]
        public void ReadByIds_EmptyIds_ReturnsEmptyWithoutReading()
        {
            var reads = 0;
            var result = Read(Array.Empty<Guid>(), _ =>
            {
                reads++;
                return Enumerable.Empty<TransponderPlan>();
            });

            Assert.AreEqual(0, result.Count());
            Assert.AreEqual(0, reads);
        }

        [TestMethod]
        public void ReadByIds_DeduplicatesAndBoundsOrFilters()
        {
            var ids = Enumerable.Range(0, 121).Select(_ => Guid.NewGuid()).ToList();
            ids.Add(ids[0]);
            var sizes = new List<int>();

            var result = Read(ids, filter =>
            {
                sizes.Add(((ORFilterElement<TransponderPlan>)filter).subFilters.Length);
                return Enumerable.Empty<TransponderPlan>();
            });

            Assert.AreEqual(0, result.Count());
            CollectionAssert.AreEqual(new[] { 50, 50, 21 }, sizes);
        }

        [TestMethod]
        public void ReadByIds_SingleId_UsesSingleFilter()
        {
            FilterElement<TransponderPlan> receivedFilter = null;

            Read(new[] { Guid.NewGuid() }, filter =>
            {
                receivedFilter = filter;
                return Enumerable.Empty<TransponderPlan>();
            }).ToList();

            Assert.IsNotNull(receivedFilter);
            Assert.IsFalse(receivedFilter is ORFilterElement<TransponderPlan>);
        }

        [TestMethod]
        public void ReadByIds_NullOrEmptyGuid_RejectsBeforeRead()
        {
            Assert.AreEqual("transponderIds",
                Assert.ThrowsException<ArgumentNullException>(() => Read(null, _ => Enumerable.Empty<TransponderPlan>())).ParamName);
            Assert.AreEqual("transponderIds",
                Assert.ThrowsException<ArgumentException>(() => Read(new[] { Guid.NewGuid(), Guid.Empty }, _ => Enumerable.Empty<TransponderPlan>())).ParamName);
        }

        private static IEnumerable<TransponderPlan> Read(
            IEnumerable<Guid> ids,
            Func<FilterElement<TransponderPlan>, IEnumerable<TransponderPlan>> read)
        {
            return RelationshipReadHelper.ReadByIds(
                ids,
                "transponderIds",
                id => TransponderPlanExposers.Transponder.Equal(id),
                read);
        }
    }
}
