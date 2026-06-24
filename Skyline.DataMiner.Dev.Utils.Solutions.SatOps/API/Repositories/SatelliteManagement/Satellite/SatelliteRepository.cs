using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Satellite
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;

    internal class SatelliteRepository : Repository, ISatelliteRepository
    {
        public SatelliteRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }

        private DomHelper DomHelper => SatOpsApi.SlcSatelliteManagementHelper.DomHelper;

        private static bool IsSatelliteInstance(DomInstance di)
        {
            return di.DomDefinitionId.Equals(SlcSatellite_ManagementIds.Definitions.Satellites);
        }

        public long Count()
        {
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .LongCount(IsSatelliteInstance);
        }

        public long Count(Net.Messages.SLDataGateway.FilterElement<Satellite> filter)
        {
            throw new NotImplementedException();
        }

        public long Count(SLDataGateway.API.Types.Querying.IQuery<Satellite> query)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Satellite> Create(IEnumerable<Satellite> oToCreate)
        {
            return oToCreate.Select(Create).ToList();
        }

        public Satellite Create(Satellite oToCreate)
        {
            var updatedInstance = oToCreate.ToUpdatedInstance();
            var createdDomInstance = DomHelper.DomInstances.Create(updatedInstance.ToInstance());
            return Satellite.FromInstance(new SatellitesInstance(createdDomInstance));
        }

        public IReadOnlyCollection<Satellite> CreateOrUpdate(IEnumerable<Satellite> oToCreateOrUpdate)
        {
            var results = new List<Satellite>();
            foreach (var satellite in oToCreateOrUpdate)
            {
                var updatedInstance = satellite.ToUpdatedInstance();
                updatedInstance.Save(DomHelper);
                results.Add(Satellite.FromInstance(updatedInstance.Clone()));
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            var satellite = Read(apiObjectId);
            if (satellite != null)
                satellite.ToOriginalInstance().Delete(DomHelper);
        }

        public void Delete(IEnumerable<Guid> apiObjectIds)
        {
            foreach (var id in apiObjectIds)
                Delete(id);
        }

        public void Delete(IEnumerable<Satellite> oToDelete)
        {
            foreach (var satellite in oToDelete)
                Delete(satellite);
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
            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(id) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            var domInstance = DomHelper.DomInstances.Read(filter).FirstOrDefault();
            if (domInstance == null || !IsSatelliteInstance(domInstance))
                return null;

            return Satellite.FromInstance(new SatellitesInstance(domInstance));
        }

        public IEnumerable<Satellite> Read(IEnumerable<Guid> ids)
        {
            var idSet = new HashSet<Guid>(ids);
            return DomHelper.DomInstances.Read(new TRUEFilterElement<DomInstance>())
                .Where(di => IsSatelliteInstance(di) && idSet.Contains(di.ID.Id))
                .Select(di => Satellite.FromInstance(new SatellitesInstance(di)));
        }

        public IEnumerable<Satellite> Read(Net.Messages.SLDataGateway.FilterElement<Satellite> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Satellite> Read(SLDataGateway.API.Types.Querying.IQuery<Satellite> query)
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

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(Net.Messages.SLDataGateway.FilterElement<Satellite> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(SLDataGateway.API.Types.Querying.IQuery<Satellite> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(Net.Messages.SLDataGateway.FilterElement<Satellite> filter, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(SLDataGateway.API.Types.Querying.IQuery<Satellite> query, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Satellite> Update(IEnumerable<Satellite> oToUpdate)
        {
            return oToUpdate.Select(Update).ToList();
        }

        public Satellite Update(Satellite oToUpdate)
        {
            var updatedInstance = oToUpdate.ToUpdatedInstance();
            var updatedDomInstance = DomHelper.DomInstances.Update(updatedInstance.ToInstance());
            return Satellite.FromInstance(new SatellitesInstance(updatedDomInstance));
        }
    }
}

