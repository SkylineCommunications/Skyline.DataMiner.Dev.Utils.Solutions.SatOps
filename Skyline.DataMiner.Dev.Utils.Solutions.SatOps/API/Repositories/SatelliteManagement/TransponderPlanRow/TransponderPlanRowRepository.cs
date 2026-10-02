using Skyline.DataMiner.SDM;

namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using Skyline.DataMiner.Utils.DOM.Extensions;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TransponderPlanRowRepository : Repository, ITransponderPlanRowRepository
    {
        public TransponderPlanRowRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private readonly TransponderPlanRowFilterTranslator filterTranslator = new TransponderPlanRowFilterTranslator();

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsTransponderPlanRowInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.TransponderPlanRows);
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsTransponderPlanRowInstance);
        }

        public IReadOnlyCollection<TransponderPlanRow> Create(IEnumerable<TransponderPlanRow> oToCreate)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            var rows = oToCreate.ToList();
            if (rows.Any(row => row == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToCreate));

            var ids = rows.Select(row => row.Id).ToList();
            if (ids.Count != ids.Distinct().Count())
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderPlanRow);

            for (var i = 0; i < ids.Count; i += 100)
            {
                var filter = DomInstanceFilterHelper.BuildIdOrFilter(ids.Skip(i).Take(100), SlcSatellite_ManagementIds.ModuleId);
                if (DomHelper.DomInstances.Read(filter).Any())
                    throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderPlanRow);
            }

            if (rows.Count == 0)
                return rows;

            var result = DomHelper.DomInstances.CreateOrUpdateInBatches(
                rows.Select(row => row.ToUpdatedInstance().ToInstance()));
            if (result.UnsuccessfulIds.Count != 0 || result.SuccessfulItems.Count != rows.Count)
                throw new InvalidOperationException(
                    $"Could not create all transponder plan rows. Succeeded: {result.SuccessfulItems.Count}; failed IDs: {string.Join(", ", result.UnsuccessfulIds)}. {FormatFailureDetails(result.TraceDataPerItem)}");

            var byId = result.SuccessfulItems.ToDictionary(instance => instance.ID.Id);
            return ids.Select(id => TransponderPlanRow.FromInstance(new TransponderPlanRowsInstance(byId[id]))).ToList();
        }

        public TransponderPlanRow Create(TransponderPlanRow oToCreate)
        {
            if (oToCreate.Id != Guid.Empty && Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderPlanRow);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<TransponderPlanRow> CreateOrUpdate(IEnumerable<TransponderPlanRow> oToCreateOrUpdate)
        {
            var results = new List<TransponderPlanRow>();
            foreach (var transponderPlanRow in oToCreateOrUpdate)
            {
                var existing = transponderPlanRow.Id != Guid.Empty ? Read(transponderPlanRow.Id) : null;
                var result = existing == null ? CreateInternal(transponderPlanRow) : UpdateInternal(transponderPlanRow);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));

            var transponderPlanRow = Read(apiObjectId);

            transponderPlanRow?.ToOriginalInstance().Delete(DomHelper);
        }

        public void Delete(IEnumerable<Guid> apiObjectIds)
        {
            if (apiObjectIds == null)
                throw new ArgumentNullException(nameof(apiObjectIds));

            var ids = apiObjectIds.ToList();
            if (ids.Any(id => id == Guid.Empty))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(apiObjectIds));

            var rows = new List<TransponderPlanRow>();
            for (var i = 0; i < ids.Count; i += 100)
                rows.AddRange(Read(ids.Skip(i).Take(100)));

            DeleteInstances(rows.Select(row => row.ToOriginalInstance().ToInstance()));
        }

        public void Delete(IEnumerable<TransponderPlanRow> oToDelete)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            var rows = oToDelete.ToList();
            if (rows.Any(row => row == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToDelete));

            DeleteInstances(rows.Select(row => row.ToOriginalInstance().ToInstance()));
        }

        public void Delete(TransponderPlanRow oToDelete)
        {
            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        public IEnumerable<TransponderPlanRow> Read()
        {
            return DomHelper.DomInstances.Read(DomInstanceExposers.DomDefinitionId.Equal(SlcSatellite_ManagementIds.Definitions.TransponderPlanRows.Id))
                .Where(IsTransponderPlanRowInstance)
                .Select(di => TransponderPlanRow.FromInstance(new TransponderPlanRowsInstance(di)));
        }

        public IEnumerable<TransponderPlanRow> ReadByTransponderPlan(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            return Read(TransponderPlanRowExposers.TransponderPlan.Equal(transponderPlanId));
        }

        public TransponderPlanRow Read(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsTransponderPlanRowInstance(domInstance))
                return null;

            return TransponderPlanRow.FromInstance(new TransponderPlanRowsInstance(domInstance));
        }

        public IEnumerable<TransponderPlanRow> Read(IEnumerable<Guid> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var idSet = new HashSet<Guid>();
            foreach (var id in ids)
            {
                if (id == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(ids));

                idSet.Add(id);
            }

            var idFilter = DomInstanceFilterHelper.BuildIdOrFilter(idSet, SlcSatellite_ManagementIds.ModuleId);
            if (idFilter == null)
                return Enumerable.Empty<TransponderPlanRow>();

            return DomHelper.DomInstances.Read(idFilter)
                .Where(di => IsTransponderPlanRowInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => TransponderPlanRow.FromInstance(new TransponderPlanRowsInstance(di)));
        }

        public TransponderPlanRow Update(TransponderPlanRow oToUpdate)
        {
            if (oToUpdate.Id == Guid.Empty || Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderPlanRow);

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<TransponderPlanRow> Update(IEnumerable<TransponderPlanRow> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        public long Count(FilterElement<TransponderPlanRow> filter)
        {
            if (filter.isEmpty())
            {
                return 0;
            }

            var domFilter = filterTranslator.Translate(filter);
            return SatOpsApi.SlcSatelliteManagementHelper.CountSatelliteManagementInstances(domFilter);
        }

        public long Count(IQuery<TransponderPlanRow> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Count(query.Filter);
        }

        public IEnumerable<TransponderPlanRow> Read(FilterElement<TransponderPlanRow> filter)
        {
            if (filter.isEmpty())
            {
                return Enumerable.Empty<TransponderPlanRow>();
            }

            var domFilter = filterTranslator.Translate(filter);
            return TransponderPlanRow.InstantiateTransponderPlanRows(SatOpsApi.SlcSatelliteManagementHelper.GetTransponderPlanRows(domFilter));
        }

        public IEnumerable<TransponderPlanRow> Read(IQuery<TransponderPlanRow> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Read(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged()
        {
            return ReadPaged(new TRUEFilterElement<TransponderPlanRow>());
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(int pageSize)
        {
            return ReadPaged(new TRUEFilterElement<TransponderPlanRow>(), pageSize);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(FilterElement<TransponderPlanRow> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(IQuery<TransponderPlanRow> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(FilterElement<TransponderPlanRow> filter, int pageSize)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return ReadPagedIterator(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(IQuery<TransponderPlanRow> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter, pageSize);
        }

        private IEnumerable<IPagedResult<TransponderPlanRow>> ReadPagedIterator(FilterElement<TransponderPlanRow> filter, int pageSize)
        {
            var pageNumber = 0;
            var domFilter = filterTranslator.Translate(filter);
            var items = SatOpsApi.SlcSatelliteManagementHelper.GetTransponderPlanRowsPaged(domFilter, pageSize);

            var enumerator = items.GetEnumerator();
            var hasNext = enumerator.MoveNext();

            while (hasNext)
            {
                var page = enumerator.Current;
                hasNext = enumerator.MoveNext();
                yield return new PagedResult<TransponderPlanRow>(TransponderPlanRow.InstantiateTransponderPlanRows(page), pageNumber++, pageSize, hasNext);
            }
        }

        private TransponderPlanRow CreateInternal(TransponderPlanRow transponderPlanRow)
        {
            var updatedInstance = transponderPlanRow.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return TransponderPlanRow.FromInstance(new TransponderPlanRowsInstance(createdDomInstance));
        }

        private TransponderPlanRow UpdateInternal(TransponderPlanRow transponderPlanRow)
        {
            var updatedInstance = transponderPlanRow.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return TransponderPlanRow.FromInstance(new TransponderPlanRowsInstance(updatedDomInstance));
        }

        private void DeleteInstances(IEnumerable<DomInstance> instances)
        {
            var items = instances.GroupBy(instance => instance.ID.Id).Select(group => group.First()).ToList();
            if (items.Count == 0)
                return;

            var result = DomHelper.DomInstances.DeleteInBatches(items);
            if (result.UnsuccessfulIds.Count != 0 || result.SuccessfulIds.Count != items.Count)
                throw new InvalidOperationException(
                    $"Could not delete all transponder plan rows. Succeeded: {result.SuccessfulIds.Count}; failed IDs: {string.Join(", ", result.UnsuccessfulIds)}. {FormatFailureDetails(result.TraceDataPerItem)}");
        }

        private static string FormatFailureDetails<TKey>(IDictionary<TKey, Skyline.DataMiner.Net.TraceData> traceData)
        {
            return traceData == null
                ? string.Empty
                : string.Join("; ", traceData.Select(entry => $"{entry.Key}: {entry.Value}"));
        }
    }
}
