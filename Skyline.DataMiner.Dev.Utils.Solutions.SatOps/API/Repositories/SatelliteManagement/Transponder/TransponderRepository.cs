namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TransponderRepository : Repository, ITransponderRepository
    {
        public TransponderRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private readonly TransponderFilterTranslator filterTranslator = new TransponderFilterTranslator();

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
            if (oToCreate.Id != Guid.Empty && Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponder);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<Transponder> CreateOrUpdate(IEnumerable<Transponder> oToCreateOrUpdate)
        {
            var results = new List<Transponder>();
            foreach (var transponder in oToCreateOrUpdate)
            {
                var existing = transponder.Id != Guid.Empty ? Read(transponder.Id) : null;
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

        public IEnumerable<Transponder> ReadBySatellite(Guid satelliteId)
        {
            if (satelliteId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(satelliteId));

            return Read(TransponderExposers.TransponderSatellite.Equal(satelliteId));
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

        public IEnumerable<Transponder> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsTransponderInstance)
                .Select(di => Transponder.FromInstance(new TranspondersInstance(di)));
        }

        public Transponder Update(Transponder oToUpdate)
        {
            if (oToUpdate.Id == Guid.Empty || Read(oToUpdate.Id) == null)
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

            if (transponders.Any(transponder => transponder == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponders));

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

            if(transponders.Any(transponder => transponder == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponders));

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

            if(transponders.Any(transponder => transponder == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponders));

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
            if (filter.isEmpty())
            {
                return 0;
            }

            var domFilter = filterTranslator.Translate(filter);
            return SatOpsApi.SlcSatelliteManagementHelper.CountSatelliteManagementInstances(domFilter);
        }

        public long Count(IQuery<Transponder> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Count(query.Filter);
        }

        public IEnumerable<Transponder> Read(FilterElement<Transponder> filter)
        {
            if (filter.isEmpty())
            {
                return Enumerable.Empty<Transponder>();
            }

            var domFilter = filterTranslator.Translate(filter);
            return Transponder.InstantiateTransponders(SatOpsApi.SlcSatelliteManagementHelper.GetTransponders(domFilter));
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

            return ReadPagedIterator(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(IQuery<Transponder> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter, pageSize);
        }

        private IEnumerable<IPagedResult<Transponder>> ReadPagedIterator(FilterElement<Transponder> filter, int pageSize)
        {
            var pageNumber = 0;
            var domFilter = filterTranslator.Translate(filter);
            var items = SatOpsApi.SlcSatelliteManagementHelper.GetTranspondersPaged(domFilter, pageSize);

            var enumerator = items.GetEnumerator();
            var hasNext = enumerator.MoveNext();

            while (hasNext)
            {
                var page = enumerator.Current;
                hasNext = enumerator.MoveNext();
                yield return new PagedResult<Transponder>(Transponder.InstantiateTransponders(page), pageNumber++, pageSize, hasNext);
            }
        }

        private Transponder DoTransition(Guid id, string transitionId)
        {
            var transponder = Read(id) ?? throw new ArgumentException(string.Format(ExceptionMessages.TransponderWithIdWasNotFound, id), nameof(id));

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
