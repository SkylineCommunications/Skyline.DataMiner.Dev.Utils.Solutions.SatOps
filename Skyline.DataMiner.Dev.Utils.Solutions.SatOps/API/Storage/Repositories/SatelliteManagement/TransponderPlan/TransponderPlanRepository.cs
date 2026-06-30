namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;

    internal class TransponderPlanRepository : Repository, ITransponderPlanRepository
    {
        public TransponderPlanRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsTransponderPlanInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.TransponderPlans);
        }

        public TransponderPlan Initialize()
        {
            return TransponderPlan.CreateNewTransponderPlan();
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsTransponderPlanInstance);
        }

        public IReadOnlyCollection<TransponderPlan> Create(IEnumerable<TransponderPlan> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public TransponderPlan Create(TransponderPlan oToCreate)
        {
            if (Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderPlan);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<TransponderPlan> CreateOrUpdate(IEnumerable<TransponderPlan> oToCreateOrUpdate)
        {
            var results = new List<TransponderPlan>();
            foreach (var transponderPlan in oToCreateOrUpdate)
            {
                var existing = Read(transponderPlan.Id);
                var result = existing == null ? CreateInternal(transponderPlan) : UpdateInternal(transponderPlan);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));

            var transponderPlan = Read(apiObjectId);
            if (transponderPlan != null)
                transponderPlan.ToOriginalInstance().Delete(DomHelper);
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

        public void Delete(IEnumerable<TransponderPlan> oToDelete)
        {
            foreach (var transponderPlan in oToDelete)
            {
                Delete(transponderPlan);
            }
        }

        public void Delete(TransponderPlan oToDelete)
        {
            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        public IEnumerable<TransponderPlan> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsTransponderPlanInstance)
                .Select(di => TransponderPlan.FromInstance(new TransponderPlansInstance(di)));
        }

        public TransponderPlan Read(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsTransponderPlanInstance(domInstance))
                return null;

            return TransponderPlan.FromInstance(new TransponderPlansInstance(domInstance));
        }

        public IEnumerable<TransponderPlan> Read(IEnumerable<Guid> ids)
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
                .Where(di => IsTransponderPlanInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => TransponderPlan.FromInstance(new TransponderPlansInstance(di)));
        }

        public TransponderPlan Update(TransponderPlan oToUpdate)
        {
            if (Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderPlan);

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<TransponderPlan> Update(IEnumerable<TransponderPlan> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        public TransponderPlan Activate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.TransponderPlansBehavior.Transitions.Draft_To_Active);
        }

        public TransponderPlan Activate(TransponderPlan transponderPlan)
        {
            if (transponderPlan == null)
                throw new ArgumentNullException(nameof(transponderPlan));

            return Activate(transponderPlan.Id);
        }

        public IReadOnlyCollection<TransponderPlan> Activate(IEnumerable<TransponderPlan> transponderPlans)
        {
            if (transponderPlans == null)
                throw new ArgumentNullException(nameof(transponderPlans));

            foreach (var transponderPlan in transponderPlans)
            {
                if (transponderPlan == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponderPlans));
            }

            return transponderPlans.Select(Activate).ToList();
        }

        public IReadOnlyCollection<TransponderPlan> Activate(IEnumerable<Guid> transponderPlanIds)
        {
            if (transponderPlanIds == null)
                throw new ArgumentNullException(nameof(transponderPlanIds));

            foreach (var transponderPlanId in transponderPlanIds)
            {
                if (transponderPlanId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(transponderPlanIds));
            }

            return transponderPlanIds.Select(Activate).ToList();
        }

        public TransponderPlan Deprecate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.TransponderPlansBehavior.Transitions.Active_To_Deprecated);
        }

        public TransponderPlan Deprecate(TransponderPlan transponderPlan)
        {
            if (transponderPlan == null)
                throw new ArgumentNullException(nameof(transponderPlan));

            return Deprecate(transponderPlan.Id);
        }

        public IReadOnlyCollection<TransponderPlan> Deprecate(IEnumerable<TransponderPlan> transponderPlans)
        {
            if (transponderPlans == null)
                throw new ArgumentNullException(nameof(transponderPlans));

            foreach (var transponderPlan in transponderPlans)
            {
                if (transponderPlan == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponderPlans));
            }

            return transponderPlans.Select(Deprecate).ToList();
        }

        public IReadOnlyCollection<TransponderPlan> Deprecate(IEnumerable<Guid> transponderPlanIds)
        {
            if (transponderPlanIds == null)
                throw new ArgumentNullException(nameof(transponderPlanIds));

            foreach (var transponderPlanId in transponderPlanIds)
            {
                if (transponderPlanId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(transponderPlanIds));
            }

            return transponderPlanIds.Select(Deprecate).ToList();
        }

        public TransponderPlan Reactivate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.TransponderPlansBehavior.Transitions.Deprecated_To_Active);
        }

        public TransponderPlan Reactivate(TransponderPlan transponderPlan)
        {
            if (transponderPlan == null)
                throw new ArgumentNullException(nameof(transponderPlan));

            return Reactivate(transponderPlan.Id);
        }

        public IReadOnlyCollection<TransponderPlan> Reactivate(IEnumerable<TransponderPlan> transponderPlans)
        {
            if (transponderPlans == null)
                throw new ArgumentNullException(nameof(transponderPlans));

            foreach (var transponderPlan in transponderPlans)
            {
                if (transponderPlan == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponderPlans));
            }

            return transponderPlans.Select(Reactivate).ToList();
        }

        public IReadOnlyCollection<TransponderPlan> Reactivate(IEnumerable<Guid> transponderPlanIds)
        {
            if (transponderPlanIds == null)
                throw new ArgumentNullException(nameof(transponderPlanIds));

            foreach (var transponderPlanId in transponderPlanIds)
            {
                if (transponderPlanId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(transponderPlanIds));
            }

            return transponderPlanIds.Select(Reactivate).ToList();
        }

        #region Not Implemented Methods
        public long Count(FilterElement<TransponderPlan> filter)
        {
            throw new NotImplementedException();
        }

        public long Count(IQuery<TransponderPlan> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TransponderPlan> Read(FilterElement<TransponderPlan> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TransponderPlan> Read(IQuery<TransponderPlan> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(FilterElement<TransponderPlan> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(IQuery<TransponderPlan> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(FilterElement<TransponderPlan> filter, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(IQuery<TransponderPlan> query, int pageSize)
        {
            throw new NotImplementedException();
        }
        #endregion

        private TransponderPlan DoTransition(Guid id, string transitionId)
        {
            var transponderPlan = Read(id);
            if (transponderPlan == null)
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderPlanWithIdWasNotFound, id), nameof(id));

            var domInstanceId = transponderPlan.ToOriginalInstance().ID;
            var transitionedDomInstance = DomHelper.DomInstances.DoStatusTransition(domInstanceId, transitionId);
            return TransponderPlan.FromInstance(new TransponderPlansInstance(transitionedDomInstance));
        }

        private TransponderPlan CreateInternal(TransponderPlan transponderPlan)
        {
            var updatedInstance = transponderPlan.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return TransponderPlan.FromInstance(new TransponderPlansInstance(createdDomInstance));
        }

        private TransponderPlan UpdateInternal(TransponderPlan transponderPlan)
        {
            var updatedInstance = transponderPlan.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return TransponderPlan.FromInstance(new TransponderPlansInstance(updatedDomInstance));
        }
    }
}
