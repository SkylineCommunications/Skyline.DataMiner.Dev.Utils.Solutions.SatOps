namespace Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories.SatelliteManagement.Satellite
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class SatelliteRepository : Repository, ISatelliteRepository
    {

        public SatelliteRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsSatelliteInstance(DomInstance instance)
        {
            return instance.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.Satellites);
        }

        public Satellite Initialize()
        {
            return Satellite.CreateNewSatellite();
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsSatelliteInstance);
        }

        public IReadOnlyCollection<Satellite> Create(IEnumerable<Satellite> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public Satellite Create(Satellite oToCreate)
        {
            if (Read(oToCreate.Id) != null)
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingSatellite);

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<Satellite> CreateOrUpdate(IEnumerable<Satellite> oToCreateOrUpdate)
        {
            var results = new List<Satellite>();
            foreach (var satellite in oToCreateOrUpdate)
            {
                var existing = Read(satellite.Id);
                var result = existing == null ? CreateInternal(satellite) : UpdateInternal(satellite);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));

            var satellite = Read(apiObjectId);
            if (satellite != null)
                satellite.ToOriginalInstance().Delete(DomHelper);
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

        public void Delete(IEnumerable<Satellite> oToDelete)
        {
            foreach (var satellite in oToDelete)
            {
                Delete(satellite);
            }
        }

        public void Delete(Satellite oToDelete)
        {
            oToDelete.ToOriginalInstance().Delete(DomHelper);
        }

        public IEnumerable<Satellite> Read()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(IsSatelliteInstance)
                .Select(di => Satellite.FromInstance(new SatellitesInstance(di)));
        }

        public Satellite Read(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsSatelliteInstance(domInstance))
                return null;

            return Satellite.FromInstance(new SatellitesInstance(domInstance));
        }

        public IEnumerable<Satellite> Read(IEnumerable<Guid> ids)
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
                .Where(di => IsSatelliteInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => Satellite.FromInstance(new SatellitesInstance(di)));
        }

        public Satellite Update(Satellite oToUpdate)
        {
            if (Read(oToUpdate.Id) == null)
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingSatellite);

            return UpdateInternal(oToUpdate);
        }

        public Satellite Activate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.SatellitesBehavior.Transitions.Draft_To_Active);
        }
        public Satellite Activate(Satellite satellite)
        {
            if (satellite == null)
                throw new ArgumentNullException(nameof(satellite));

            return Activate(satellite.Id);
        }

        public IReadOnlyCollection<Satellite> Activate(IEnumerable<Satellite> satellites)
        {
            if (satellites == null)
                throw new ArgumentNullException(nameof(satellites));

            foreach (var satellite in satellites)
            {
                if (satellite == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(satellites));
            }

            return satellites.Select(s => Activate(s)).ToList();
        }

        public IReadOnlyCollection<Satellite> Activate(IEnumerable<Guid> satelliteIds)
        {
            if (satelliteIds == null)
                throw new ArgumentNullException(nameof(satelliteIds));

            foreach (var satelliteId in satelliteIds)
            {
                if (satelliteId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(satelliteIds));
            }

            return satelliteIds.Select(id => Activate(id)).ToList();
        }

        public Satellite Deprecate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.SatellitesBehavior.Transitions.Active_To_Deprecated);
        }
        public Satellite Deprecate(Satellite satellite)
        {
            if (satellite == null)
                throw new ArgumentNullException(nameof(satellite));

            return Deprecate(satellite.Id);
        }

        public IReadOnlyCollection<Satellite> Deprecate(IEnumerable<Satellite> satellites)
        {
            if (satellites == null)
                throw new ArgumentNullException(nameof(satellites));

            foreach (var satellite in satellites)
            {
                if (satellite == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(satellites));
            }

            return satellites.Select(s => Deprecate(s)).ToList();
        }

        public IReadOnlyCollection<Satellite> Deprecate(IEnumerable<Guid> satelliteIds)
        {
            if (satelliteIds == null)
                throw new ArgumentNullException(nameof(satelliteIds));

            foreach (var satelliteId in satelliteIds)
            {
                if (satelliteId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(satelliteIds));
            }

            return satelliteIds.Select(id => Deprecate(id)).ToList();
        }

        public Satellite Reactivate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));

            return DoTransition(id, SlcSatellite_ManagementIds.Behaviors.SatellitesBehavior.Transitions.Deprecated_To_Active);
        }

        public Satellite Reactivate(Satellite satellite)
        {
            if (satellite == null)
                throw new ArgumentNullException(nameof(satellite));

            return Reactivate(satellite.Id);
        }

        public IReadOnlyCollection<Satellite> Reactivate(IEnumerable<Satellite> satellites)
        {
            if (satellites == null)
                throw new ArgumentNullException(nameof(satellites));

            foreach (var satellite in satellites)
            {
                if (satellite == null)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(satellites));
            }

            return satellites.Select(s => Reactivate(s)).ToList();
        }

        public IReadOnlyCollection<Satellite> Reactivate(IEnumerable<Guid> satelliteIds)
        {
            if (satelliteIds == null)
                throw new ArgumentNullException(nameof(satelliteIds));

            foreach (var satelliteId in satelliteIds)
            {
                if (satelliteId == Guid.Empty)
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(satelliteIds));
            }

            return satelliteIds.Select(id => Reactivate(id)).ToList();
        }

        private Satellite DoTransition(Guid id, string transitionId)
        {
            var satellite = Read(id);
            if (satellite == null)
                throw new ArgumentException(string.Format(ExceptionMessages.SatelliteWithIdWasNotFound, id), nameof(id));

            var domInstanceId = satellite.ToOriginalInstance().ID;
            var transitionedDomInstance = DomHelper.DomInstances.DoStatusTransition(domInstanceId, transitionId);
            return Satellite.FromInstance(new SatellitesInstance(transitionedDomInstance));
        }

        private Satellite CreateInternal(Satellite satellite)
        {
            var updatedInstance = satellite.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return Satellite.FromInstance(new SatellitesInstance(createdDomInstance));
        }

        private Satellite UpdateInternal(Satellite satellite)
        {
            var updatedInstance = satellite.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return Satellite.FromInstance(new SatellitesInstance(updatedDomInstance));
        }

        public IReadOnlyCollection<Satellite> Update(IEnumerable<Satellite> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        public long Count(FilterElement<Satellite> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return Read(filter).LongCount();
        }

        public long Count(IQuery<Satellite> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Read(query).LongCount();
        }

        public IEnumerable<Satellite> Read(FilterElement<Satellite> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return Read();
        }

        public IEnumerable<Satellite> Read(IQuery<Satellite> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return Read(query.Filter);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged()
        {
            return ReadPaged(new TRUEFilterElement<Satellite>());
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(int pageSize)
        {
            return ReadPaged(new TRUEFilterElement<Satellite>(), pageSize);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(FilterElement<Satellite> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(IQuery<Satellite> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return ReadPaged(query.Filter);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(FilterElement<Satellite> filter, int pageSize)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return CreatePagedResults(Read(filter), pageSize);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(IQuery<Satellite> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return CreatePagedResults(Read(query.Filter), pageSize);
        }

        private static IEnumerable<IPagedResult<Satellite>> CreatePagedResults(IEnumerable<Satellite> items, int pageSize)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            var list = items.ToList();
            if (list.Count == 0)
                return Enumerable.Empty<IPagedResult<Satellite>>();

            var totalPages = (list.Count + pageSize - 1) / pageSize;
            var pages = new List<IPagedResult<Satellite>>(totalPages);
            for (var page = 0; page < totalPages; page++)
            {
                pages.Add(PagedResult<Satellite>.Create(list, pageSize, page));
            }

            return pages;
        }
    }
}

