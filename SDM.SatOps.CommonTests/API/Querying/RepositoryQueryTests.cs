namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using SLDataGateway.API.Querying;
    using SLDataGateway.API.Types.Querying;

    [TestClass]
    public class RepositoryQueryTests
    {
        [DataTestMethod]
        [DataRow("satellites")]
        [DataRow("beams")]
        [DataRow("transponders")]
        [DataRow("plans")]
        [DataRow("rows")]
        [DataRow("slots")]
        public void QueryReads_ForwardFilterOrderOptionsAndLimitToStorage(string resource)
        {
            switch (resource)
            {
                case "satellites":
                    AssertQueryReads(api => api.Satellites, SatelliteExposers.Guid, SatelliteExposers.Name,
                        DomInstanceExposers.FieldValues.DomInstanceField(SlcSatellite_ManagementIds.Sections.General.SatelliteName));
                    break;
                case "beams":
                    AssertQueryReads(api => api.Beams, BeamExposers.Guid, BeamExposers.Name,
                        DomInstanceExposers.FieldValues.DomInstanceField(SlcSatellite_ManagementIds.Sections.Beam.BeamName));
                    break;
                case "transponders":
                    AssertQueryReads(api => api.Transponders, TransponderExposers.Guid, TransponderExposers.Name,
                        DomInstanceExposers.FieldValues.DomInstanceField(SlcSatellite_ManagementIds.Sections.Transponder.TransponderName));
                    break;
                case "plans":
                    AssertQueryReads(api => api.TransponderPlans, TransponderPlanExposers.Guid, TransponderPlanExposers.Name,
                        DomInstanceExposers.FieldValues.DomInstanceField(SlcSatellite_ManagementIds.Sections.TransponderPlan.PlanName));
                    break;
                case "rows":
                    AssertQueryReads(api => api.TransponderPlanRows, TransponderPlanRowExposers.Guid, TransponderPlanRowExposers.Bandwidth,
                        DomInstanceExposers.FieldValues.DomInstanceField(SlcSatellite_ManagementIds.Sections.TransponderPlanRow.Bandwidth));
                    break;
                case "slots":
                    AssertQueryReads<TransponderSlot>(api => api.TransponderSlots, TransponderSlotExposers.Guid, TransponderSlotExposers.Name,
                        DomInstanceExposers.FieldValues.DomInstanceField(SlcSatellite_ManagementIds.Sections.TransponderSlot.SlotName));
                    break;
                default:
                    Assert.Fail("Unexpected resource.");
                    break;
            }
        }

        [TestMethod]
        public void TranslateQuery_UnsupportedOrderField_ThrowsInsteadOfDroppingOrder()
        {
            var query = new TRUEFilterElement<Satellite>().ToQuery().WithOrder(
                OrderBy.Default.SingleConcat(OrderByElement.Default.WithFieldExposer(BeamExposers.FootprintFile)));

            Assert.ThrowsException<NotSupportedException>(() => new SatelliteFilterTranslator().TranslateQuery(query));
        }

        [TestMethod]
        public void TranslateQuery_Null_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SatelliteFilterTranslator().TranslateQuery(null));
        }

        [DataTestMethod]
        [DataRow(0, 2)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(3, 2)]
        public void LimitedQuery_PagesOnlyTheOrderedStorageResults(int limit, int pageSize)
        {
            var expected = new[] { "C", "B", "A" }.Take(limit).ToArray();
            var instances = expected.Select(name => new SatellitesInstance
            {
                General = { SatelliteName = name, SatelliteAbbreviation = name },
            }.ToInstance()).ToList();
            var connection = new Mock<IConnection>(MockBehavior.Strict);
            connection.SetupGet(value => value.IsShuttingDown).Returns(false);
            connection.Setup(value => value.HandleMessages(It.IsAny<DMSMessage[]>()))
                .Returns<DMSMessage[]>(messages =>
                {
                    if (messages.Single() is ManagerStoreReadRequest<DomInstance> read)
                    {
                        Assert.AreEqual(limit, read.Query.Limit.Limit);
                        var element = read.Query.Order.Elements.Single();
                        Assert.AreEqual(SortOrder.Descending, element.SortOrder);
                        Assert.AreEqual(DomInstanceExposers.FieldValues.DomInstanceField(
                            SlcSatellite_ManagementIds.Sections.General.SatelliteName).fieldName, element.Exposer.fieldName);
                        return new DMSMessage[] { new ManagerStoreCrudResponse<DomInstance> { Objects = instances } };
                    }

                    Assert.IsInstanceOfType(messages.Single(), typeof(ManagerStoreReadRequest<SectionDefinition>));
                    return new DMSMessage[] { new ManagerStoreCrudResponse<SectionDefinition>() };
                });
            var query = new TRUEFilterElement<Satellite>().ToQuery()
                .WithOrder(OrderBy.Default.SingleConcat(OrderByElement.Default
                    .WithFieldExposer(SatelliteExposers.Name).WithSortOrder(SortOrder.Descending)))
                .WithLimit(LimitBy.Default.WithLimit(limit));
            var repository = new SatOpsApi(connection.Object).Satellites;

            var pages = repository.ReadPaged(query, pageSize).ToArray();

            CollectionAssert.AreEqual(expected, repository.Read(query).Select(value => value.Name).ToArray());
            CollectionAssert.AreEqual(expected, pages.SelectMany(page => page).Select(value => value.Name).ToArray());
            Assert.AreEqual((limit + pageSize - 1) / pageSize, pages.Length);
            for (var index = 0; index < pages.Length; index++)
            {
                Assert.AreEqual(index < pages.Length - 1, pages[index].HasNextPage);
                Assert.AreEqual(Math.Min(pageSize, limit - index * pageSize), pages[index].Count());
            }
        }

        private static void AssertQueryReads<T>(
            Func<SatOpsApi, IPageableRepository<T>> repositoryFactory, FieldExposer idField, FieldExposer orderField, FieldExposer storageField)
            where T : class
        {
            foreach (var limited in new[] { false, true })
            {
                foreach (var mode in new[] { 0, 1, 2 })
                {
                    IQuery<DomInstance> captured = null;
                    var storageBoundary = new InvalidOperationException("Offline storage boundary.");
                    var connection = new Mock<IConnection>(MockBehavior.Strict);
                    connection.SetupGet(value => value.IsShuttingDown).Returns(false);
                    connection.Setup(value => value.HandleMessages(It.IsAny<DMSMessage[]>()))
                        .Returns<DMSMessage[]>(messages =>
                        {
                            var message = messages.Single();
                            if (message is ManagerStoreStartPagingRequest<DomInstance> paging)
                            {
                                captured = paging.Filter;
                                if (!limited && mode != 0)
                                {
                                    Assert.AreEqual(mode == 1 ? 100L : 1L, paging.PreferredPageSize);
                                }
                            }
                            else if (message is ManagerStoreReadRequest<DomInstance> read)
                            {
                                captured = read.Query;
                            }
                            else
                            {
                                Assert.IsInstanceOfType(message, typeof(ManagerStoreReadRequest<SectionDefinition>));
                                return new DMSMessage[] { new ManagerStoreCrudResponse<SectionDefinition>() };
                            }

                            throw storageBoundary;
                        });

                    var order = OrderBy.Default
                        .SingleConcat(OrderByElement.Default.WithFieldExposer(orderField).WithSortOrder(SortOrder.Descending).WithNaturalSort())
                        .SingleConcat(OrderByElement.Default.WithFieldExposer(idField).WithSortOrder(SortOrder.Ascending));
                    var query = FilterElementFactory.Create<T>(idField, Comparer.NotEquals, Guid.Empty).ToQuery().WithOrder(order);
                    if (limited)
                    {
                        query = query.WithLimit(LimitBy.Default.WithLimit(2));
                    }

                    var repository = repositoryFactory(new SatOpsApi(connection.Object));
                    var exception = Assert.ThrowsException<InvalidOperationException>(() =>
                    {
                        if (mode == 0)
                        {
                            repository.Read(query).ToList();
                        }
                        else if (mode == 1)
                        {
                            repository.ReadPaged(query).ToList();
                        }
                        else
                        {
                            repository.ReadPaged(query, 1).ToList();
                        }
                    });

                    Assert.AreSame(storageBoundary, exception);
                    Assert.IsNotNull(captured);
                    var elements = captured.Order.Elements.ToArray();
                    Assert.AreEqual(2, elements.Length);
                    Assert.AreEqual(storageField.fieldName, elements[0].Exposer.fieldName);
                    Assert.AreEqual(SortOrder.Descending, elements[0].SortOrder);
                    Assert.IsTrue(elements[0].Options.NaturalSort);
                    Assert.AreEqual(DomInstanceExposers.Id.fieldName, elements[1].Exposer.fieldName);
                    Assert.AreEqual(SortOrder.Ascending, elements[1].SortOrder);
                    Assert.AreEqual(query.Limit, captured.Limit);
                    var filter = (ANDFilterElement<DomInstance>)captured.Filter;
                    Assert.AreEqual(2, filter.subFilters.Length);
                    var idFilter = filter.subFilters.OfType<ManagedFilterIdentifier>()
                        .Single(value => value.getFieldName().fieldName == DomInstanceExposers.Id.fieldName);
                    Assert.AreEqual(Comparer.NotEquals, idFilter.getComparer());
                    Assert.AreEqual(Guid.Empty, idFilter.getValue());
                }
            }
        }
    }
}
