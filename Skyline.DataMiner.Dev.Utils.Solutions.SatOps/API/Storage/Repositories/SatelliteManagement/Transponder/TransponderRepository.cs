namespace Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories.SatelliteManagement.Transponder
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TransponderRepository : Repository, ITransponderRepository
    {
        public TransponderRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsTransponderInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.Transponders);
        }

        public Transponder Initialize()
        {
            return Transponder.CreateNewTransponder();
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsTransponderInstance);
        }

        public IReadOnlyCollection<Transponder> Create(IEnumerable<Transponder> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public Transponder Create(Transponder oToCreate)
        {
            if (Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponder);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<Transponder> CreateOrUpdate(IEnumerable<Transponder> oToCreateOrUpdate)
        {
            var results = new List<Transponder>();
            foreach (var transponder in oToCreateOrUpdate)
            {
                var existing = Read(transponder.Id);
                var result = existing == null ? CreateInternal(transponder) : UpdateInternal(transponder);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));

            var transponder = Read(apiObjectId);
            if (transponder != null)
                transponder.ToOriginalInstance().Delete(DomHelper);
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

        public void Delete(IEnumerable<Transponder> oToDelete)
        {
            foreach (var transponder in oToDelete)
            {
                Delete(transponder);
            }
        }

        public void Delete(Transponder oToDelete)
        {
            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        public IEnumerable<Transponder> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsTransponderInstance)
                .Select(di => Transponder.FromInstance(new TranspondersInstance(di)));
        }

        public Transponder Read(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsTransponderInstance(domInstance))
                return null;

            return Transponder.FromInstance(new TranspondersInstance(domInstance));
        }

        public IEnumerable<Transponder> Read(IEnumerable<Guid> ids)
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
                .Where(di => IsTransponderInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => Transponder.FromInstance(new TranspondersInstance(di)));
        }

        public Transponder Update(Transponder oToUpdate)
        {
            if (Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponder);

            return UpdateInternal(oToUpdate);
        }

        public IReadOnlyCollection<Transponder> Update(IEnumerable<Transponder> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        public Transponder Activate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.TranspondersBehavior.Transitions.Draft_To_Active);
        }

        public Transponder Activate(Transponder transponder)
        {
            if (transponder == null)
                throw new ArgumentNullException(nameof(transponder));

            return Activate(transponder.Id);
        }

        public IReadOnlyCollection<Transponder> Activate(IEnumerable<Transponder> transponders)
        {
            if (transponders == null)
                throw new ArgumentNullException(nameof(transponders));

            foreach (var transponder in transponders)
            {
                if (transponder == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponders));
            }

            return transponders.Select(Activate).ToList();
        }

        public IReadOnlyCollection<Transponder> Activate(IEnumerable<Guid> transponderIds)
        {
            if (transponderIds == null)
                throw new ArgumentNullException(nameof(transponderIds));

            foreach (var transponderId in transponderIds)
            {
                if (transponderId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(transponderIds));
            }

            return transponderIds.Select(Activate).ToList();
        }

        public Transponder Deprecate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.TranspondersBehavior.Transitions.Active_To_Deprecated);
        }

        public Transponder Deprecate(Transponder transponder)
        {
            if (transponder == null)
                throw new ArgumentNullException(nameof(transponder));

            return Deprecate(transponder.Id);
        }

        public IReadOnlyCollection<Transponder> Deprecate(IEnumerable<Transponder> transponders)
        {
            if (transponders == null)
                throw new ArgumentNullException(nameof(transponders));

            foreach (var transponder in transponders)
            {
                if (transponder == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponders));
            }

            return transponders.Select(Deprecate).ToList();
        }

        public IReadOnlyCollection<Transponder> Deprecate(IEnumerable<Guid> transponderIds)
        {
            if (transponderIds == null)
                throw new ArgumentNullException(nameof(transponderIds));

            foreach (var transponderId in transponderIds)
            {
                if (transponderId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(transponderIds));
            }

            return transponderIds.Select(Deprecate).ToList();
        }

        public Transponder Reactivate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.TranspondersBehavior.Transitions.Deprecated_To_Active);
        }

        public Transponder Reactivate(Transponder transponder)
        {
            if (transponder == null)
                throw new ArgumentNullException(nameof(transponder));

            return Reactivate(transponder.Id);
        }

        public IReadOnlyCollection<Transponder> Reactivate(IEnumerable<Transponder> transponders)
        {
            if (transponders == null)
                throw new ArgumentNullException(nameof(transponders));

            foreach (var transponder in transponders)
            {
                if (transponder == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponders));
            }

            return transponders.Select(Reactivate).ToList();
        }

        public IReadOnlyCollection<Transponder> Reactivate(IEnumerable<Guid> transponderIds)
        {
            if (transponderIds == null)
                throw new ArgumentNullException(nameof(transponderIds));

            foreach (var transponderId in transponderIds)
            {
                if (transponderId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(transponderIds));
            }

            return transponderIds.Select(Reactivate).ToList();
        }

        public long Count(FilterElement<Transponder> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return Read(filter).LongCount();
        }

        public long Count(IQuery<Transponder> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Read(query).LongCount();
        }

        public IEnumerable<Transponder> Read(FilterElement<Transponder> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return Read();
        }

        public IEnumerable<Transponder> Read(IQuery<Transponder> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Read(query.Filter);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged()
        {
            return ReadPaged(new TRUEFilterElement<Transponder>());
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(int pageSize)
        {
            return ReadPaged(new TRUEFilterElement<Transponder>(), pageSize);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(FilterElement<Transponder> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(IQuery<Transponder> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(FilterElement<Transponder> filter, int pageSize)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return CreatePagedResults(Read(filter), pageSize);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(IQuery<Transponder> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return CreatePagedResults(Read(query.Filter), pageSize);
        }

        private static IEnumerable<IPagedResult<Transponder>> CreatePagedResults(IEnumerable<Transponder> items, int pageSize)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var list = items.ToList();
            if (list.Count == 0)
                return Enumerable.Empty<IPagedResult<Transponder>>();

            var totalPages = (list.Count + pageSize - 1) / pageSize;
            var pages = new List<IPagedResult<Transponder>>(totalPages);
            for (var page = 0; page < totalPages; page++)
            {
                pages.Add(PagedResult<Transponder>.Create(list, pageSize, page));
            }

            return pages;
        }

        private Transponder DoTransition(Guid id, string transitionId)
        {
            var transponder = Read(id);
            if (transponder == null)
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderWithIdWasNotFound, id), nameof(id));

            var domInstanceId = transponder.ToOriginalInstance().ID;
            var transitionedDomInstance = DomHelper.DomInstances.DoStatusTransition(domInstanceId, transitionId);
            return Transponder.FromInstance(new TranspondersInstance(transitionedDomInstance));
        }

        private Transponder CreateInternal(Transponder transponder)
        {
            var updatedInstance = transponder.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return Transponder.FromInstance(new TranspondersInstance(createdDomInstance));
        }

        private Transponder UpdateInternal(Transponder transponder)
        {
            var updatedInstance = transponder.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return Transponder.FromInstance(new TranspondersInstance(updatedDomInstance));
        }
    }
}
