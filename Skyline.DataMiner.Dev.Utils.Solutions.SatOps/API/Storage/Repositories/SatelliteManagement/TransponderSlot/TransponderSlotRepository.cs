namespace Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories.SatelliteManagement.TransponderSlot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;

    internal class TransponderSlotRepository : Repository, ITransponderSlotRepository
    {
        public TransponderSlotRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsTransponderSlotInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.TransponderSlots);
        }

        public TransponderSlot Initialize()
        {
            return TransponderSlot.CreateNewTransponderSlot();
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsTransponderSlotInstance);
        }

        public IReadOnlyCollection<TransponderSlot> Create(IEnumerable<TransponderSlot> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public TransponderSlot Create(TransponderSlot oToCreate)
        {
            if (Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderSlot);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> CreateOrUpdate(IEnumerable<TransponderSlot> oToCreateOrUpdate)
        {
            var results = new List<TransponderSlot>();
            foreach (var transponderSlot in oToCreateOrUpdate)
            {
                var existing = Read(transponderSlot.Id);
                var result = existing == null ? CreateInternal(transponderSlot) : UpdateInternal(transponderSlot);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));

            var transponderSlot = Read(apiObjectId);
            if (transponderSlot != null)
                transponderSlot.ToOriginalInstance().Delete(DomHelper);
        }

        public void Delete(IEnumerable<Guid> apiObjectIds)
        {
            if (apiObjectIds == null)
                throw new ArgumentNullException(nameof(apiObjectIds));

            foreach (var id in apiObjectIds)
            {
                if (id == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(apiObjectIds));

                Delete(id);
            }
        }

        public void Delete(IEnumerable<TransponderSlot> oToDelete)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            foreach (var transponderSlot in oToDelete)
            {
                if (transponderSlot == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToDelete));

                Delete(transponderSlot);
            }
        }

        public void Delete(TransponderSlot oToDelete)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        public IEnumerable<TransponderSlot> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsTransponderSlotInstance)
                .Select(di => TransponderSlot.FromInstance(new TransponderSlotsInstance(di)));
        }

        public TransponderSlot Read(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsTransponderSlotInstance(domInstance))
                return null;

            return TransponderSlot.FromInstance(new TransponderSlotsInstance(domInstance));
        }

        public IEnumerable<TransponderSlot> Read(IEnumerable<Guid> ids)
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

            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(di => IsTransponderSlotInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => TransponderSlot.FromInstance(new TransponderSlotsInstance(di)));
        }

        public TransponderSlot Update(TransponderSlot oToUpdate)
        {
            if (Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderSlot);

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> Update(IEnumerable<TransponderSlot> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        public long Count(FilterElement<TransponderSlot> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return Read(filter).LongCount();
        }

        public long Count(IQuery<TransponderSlot> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Read(query).LongCount();
        }

        public IEnumerable<TransponderSlot> Read(FilterElement<TransponderSlot> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return Read();
        }

        public IEnumerable<TransponderSlot> Read(IQuery<TransponderSlot> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Read(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged()
        {
            return ReadPaged(new TRUEFilterElement<TransponderSlot>());
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(int pageSize)
        {
            return ReadPaged(new TRUEFilterElement<TransponderSlot>(), pageSize);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query)
        {
            return ReadPaged(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter, int pageSize)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return CreatePagedResults(Read(filter), pageSize);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return CreatePagedResults(Read(query.Filter), pageSize);
        }

        private static IEnumerable<IPagedResult<TransponderSlot>> CreatePagedResults(IEnumerable<TransponderSlot> items, int pageSize)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var list = items.ToList();
            if (list.Count == 0)
                return Enumerable.Empty<IPagedResult<TransponderSlot>>();

            var totalPages = (list.Count + pageSize - 1) / pageSize;
            var pages = new List<IPagedResult<TransponderSlot>>(totalPages);
            for (var page = 0; page < totalPages; page++)
            {
                pages.Add(PagedResult<TransponderSlot>.Create(list, pageSize, page));
            }

            return pages;
        }

        private TransponderSlot CreateInternal(TransponderSlot transponderSlot)
        {
            var updatedInstance = transponderSlot.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return TransponderSlot.FromInstance(new TransponderSlotsInstance(createdDomInstance));
        }

        private TransponderSlot UpdateInternal(TransponderSlot transponderSlot)
        {
            var updatedInstance = transponderSlot.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return TransponderSlot.FromInstance(new TransponderSlotsInstance(updatedDomInstance));
        }
    }
}
