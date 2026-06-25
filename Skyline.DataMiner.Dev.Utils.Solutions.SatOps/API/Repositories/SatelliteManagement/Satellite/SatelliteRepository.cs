using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Satellite
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using SLDataGateway.API.Types.Querying;

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

        public Satellite CreateNew()
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
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            return oToCreate.Select(Create).ToList();
        }

        public Satellite Create(Satellite oToCreate)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (Read(oToCreate.Id) != null)
                throw new InvalidOperationException("Cannot create an existing satellite.");

            return CreateInternal(oToCreate);
        }

        public IReadOnlyCollection<Satellite> CreateOrUpdate(IEnumerable<Satellite> oToCreateOrUpdate)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            var results = new List<Satellite>();
            foreach (var satellite in oToCreateOrUpdate)
            {
                if (satellite == null)
                    throw new ArgumentException("Collection cannot contain null items.", nameof(oToCreateOrUpdate));

                var existing = Read(satellite.Id);
                var result = existing == null ? CreateInternal(satellite) : UpdateInternal(satellite);
                results.Add(result);
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
                throw new ArgumentException("The value cannot be an empty GUID.", nameof(apiObjectId));

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
                    throw new ArgumentException("Collection cannot contain empty GUID values.", nameof(apiObjectIds));

                Delete(id);
            }
        }

        public void Delete(IEnumerable<Satellite> oToDelete)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            foreach (var satellite in oToDelete)
            {
                if (satellite == null)
                    throw new ArgumentException("Collection cannot contain null items.", nameof(oToDelete));

                Delete(satellite);
            }
        }

        public void Delete(Satellite oToDelete)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

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
                throw new ArgumentException("The value cannot be an empty GUID.", nameof(id));

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
                    throw new ArgumentException("Collection cannot contain empty GUID values.", nameof(ids));

                idSet.Add(id);
            }

            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(di => IsSatelliteInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => Satellite.FromInstance(new SatellitesInstance(di)));
        }

        public Satellite Update(Satellite oToUpdate)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (Read(oToUpdate.Id) == null)
                throw new InvalidOperationException("Cannot update a satellite that does not exist.");

            return UpdateInternal(oToUpdate);
        }

        public Satellite Activate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The value cannot be an empty GUID.", nameof(id));

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
                    throw new ArgumentException("Collection cannot contain null items.", nameof(satellites));
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
                    throw new ArgumentException("Collection cannot contain empty GUID values.", nameof(satelliteIds));
            }

            return satelliteIds.Select(id => Activate(id)).ToList();
        }

        public Satellite Deprecate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The value cannot be an empty GUID.", nameof(id));

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
                    throw new ArgumentException("Collection cannot contain null items.", nameof(satellites));
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
                    throw new ArgumentException("Collection cannot contain empty GUID values.", nameof(satelliteIds));
            }

            return satelliteIds.Select(id => Deprecate(id)).ToList();
        }

        public Satellite Reactivate(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The value cannot be an empty GUID.", nameof(id));

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
                    throw new ArgumentException("Collection cannot contain null items.", nameof(satellites));
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
                    throw new ArgumentException("Collection cannot contain empty GUID values.", nameof(satelliteIds));
            }

            return satelliteIds.Select(id => Reactivate(id)).ToList();
        }

        private Satellite DoTransition(Guid id, string transitionId)
        {
            var satellite = Read(id);
            if (satellite == null)
                throw new ArgumentException($"Satellite with id '{id}' was not found.", nameof(id));

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

        #region Not Implemented Methods
        public IReadOnlyCollection<Satellite> Update(IEnumerable<Satellite> oToUpdate)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            foreach (var satellite in oToUpdate)
            {
                if (satellite == null)
                    throw new ArgumentException("Collection cannot contain null items.", nameof(oToUpdate));
            }

            return oToUpdate.Select(Update).ToList();
        }

        public long Count(FilterElement<Satellite> filter)
        {
            throw new NotImplementedException();
        }

        public long Count(IQuery<Satellite> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Satellite> Read(FilterElement<Satellite> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Satellite> Read(IQuery<Satellite> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(FilterElement<Satellite> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(IQuery<Satellite> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(FilterElement<Satellite> filter, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(IQuery<Satellite> query, int pageSize)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

