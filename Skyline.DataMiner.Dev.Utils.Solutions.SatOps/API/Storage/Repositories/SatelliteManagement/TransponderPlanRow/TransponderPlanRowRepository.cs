namespace Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories.SatelliteManagement.TransponderPlanRow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;

    internal class TransponderPlanRowRepository : Repository, ITransponderPlanRowRepository
    {
        public TransponderPlanRowRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsTransponderPlanRowInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.TransponderPlanRows);
        }

        public TransponderPlanRow Initialize()
        {
            return TransponderPlanRow.CreateNewTransponderPlanRow();
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsTransponderPlanRowInstance);
        }

        public IReadOnlyCollection<TransponderPlanRow> Create(IEnumerable<TransponderPlanRow> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public TransponderPlanRow Create(TransponderPlanRow oToCreate)
        {
            if (Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderPlanRow);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<TransponderPlanRow> CreateOrUpdate(IEnumerable<TransponderPlanRow> oToCreateOrUpdate)
        {
            var results = new List<TransponderPlanRow>();
            foreach (var transponderPlanRow in oToCreateOrUpdate)
            {
                var existing = Read(transponderPlanRow.Id);
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
            if (transponderPlanRow != null)
                transponderPlanRow.ToOriginalInstance().Delete(DomHelper);
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

        public void Delete(IEnumerable<TransponderPlanRow> oToDelete)
        {
            foreach (var transponderPlanRow in oToDelete)
            {
                Delete(transponderPlanRow);
            }
        }

        public void Delete(TransponderPlanRow oToDelete)
        {
            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        public IEnumerable<TransponderPlanRow> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsTransponderPlanRowInstance)
                .Select(di => TransponderPlanRow.FromInstance(new TransponderPlanRowsInstance(di)));
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

            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(di => IsTransponderPlanRowInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => TransponderPlanRow.FromInstance(new TransponderPlanRowsInstance(di)));
        }

        public TransponderPlanRow Update(TransponderPlanRow oToUpdate)
        {
            if (Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderPlanRow);

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<TransponderPlanRow> Update(IEnumerable<TransponderPlanRow> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        #region Not Implemented Methods
        public long Count(FilterElement<TransponderPlanRow> filter)
        {
            throw new NotImplementedException();
        }

        public long Count(IQuery<TransponderPlanRow> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TransponderPlanRow> Read(FilterElement<TransponderPlanRow> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TransponderPlanRow> Read(IQuery<TransponderPlanRow> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(FilterElement<TransponderPlanRow> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(IQuery<TransponderPlanRow> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(FilterElement<TransponderPlanRow> filter, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(IQuery<TransponderPlanRow> query, int pageSize)
        {
            throw new NotImplementedException();
        }
        #endregion

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
    }
}
