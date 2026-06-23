namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement;
    using SLDataGateway.API.Types.Querying;
    using System.Collections.Generic;
    using System.Linq;
    using SDM = Skyline.DataMiner.SDM;
    public abstract class Repository<T> : SDM.IBulkRepository<T>, SDM.IQueryableRepository<T>
        where T : ApiObject<T>
    {
        private readonly FilterElement<DomInstance> _domDefinitionFilter;


        public long Count(FilterElement<T> filter)
        {
            throw new System.NotImplementedException();
        }

        public long Count(IQuery<T> query)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<T> Create(IEnumerable<T> oToCreate)
        {
            throw new System.NotImplementedException();
        }

        public T Create(T oToCreate)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<T> CreateOrUpdate(IEnumerable<T> oToCreateOrUpdate)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(IEnumerable<T> oToDelete)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(T oToDelete)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<T> Query()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> Read(FilterElement<T> filter)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> Read(IQuery<T> query)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPagedResult<T>> ReadPaged(FilterElement<T> filter)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPagedResult<T>> ReadPaged(IQuery<T> query)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPagedResult<T>> ReadPaged(FilterElement<T> filter, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IPagedResult<T>> ReadPaged(IQuery<T> query, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<T> Update(IEnumerable<T> oToUpdate)
        {
            throw new System.NotImplementedException();
        }

        public T Update(T oToUpdate)
        {
            throw new System.NotImplementedException();
        }
    }
}
