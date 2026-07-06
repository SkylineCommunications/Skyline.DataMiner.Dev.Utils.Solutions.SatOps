namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Satellite
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;

    internal sealed class SatelliteRepositoryMiddleware : ISatelliteRepository
    {
        private readonly ISatelliteRepository inner;
        private readonly IMiddlewareMarker<Satellite> middleware;

        public SatelliteRepositoryMiddleware(ISatelliteRepository inner, IMiddlewareMarker<Satellite> middleware)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
        }

        public Satellite Initialize()
        {
            return inner.Initialize();
        }

        public long Count()
        {
            return inner.Count();
        }

        public long Count(FilterElement<Satellite> filter)
        {
            if (middleware is ICountableMiddleware<Satellite> countableMiddleware)
            {
                return countableMiddleware.OnCount(filter, inner.Count);
            }

            return inner.Count(filter);
        }

        public long Count(IQuery<Satellite> query)
        {
            if (middleware is ICountableMiddleware<Satellite> countableMiddleware)
            {
                return countableMiddleware.OnCount(query, inner.Count);
            }

            return inner.Count(query);
        }

        public IReadOnlyCollection<Satellite> Create(IEnumerable<Satellite> oToCreate)
        {
            if (middleware is IBulkCreatableMiddleware<Satellite> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public Satellite Create(Satellite oToCreate)
        {
            if (middleware is ICreatableMiddleware<Satellite> creatableMiddleware)
            {
                return creatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public IReadOnlyCollection<Satellite> CreateOrUpdate(IEnumerable<Satellite> oToCreateOrUpdate)
        {
            if (middleware is IBulkRepositoryMiddleware<Satellite> bulkRepositoryMiddleware)
            {
                return bulkRepositoryMiddleware.OnCreateOrUpdate(oToCreateOrUpdate, inner.CreateOrUpdate);
            }

            return inner.CreateOrUpdate(oToCreateOrUpdate);
        }

        public void Delete(Guid apiObjectId)
        {
            inner.Delete(apiObjectId);
        }

        public void Delete(IEnumerable<Guid> apiObjectIds)
        {
            inner.Delete(apiObjectIds);
        }

        public void Delete(IEnumerable<Satellite> oToDelete)
        {
            if (middleware is IBulkDeletableMiddleware<Satellite> bulkDeletableMiddleware)
            {
                bulkDeletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public void Delete(Satellite oToDelete)
        {
            if (middleware is IDeletableMiddleware<Satellite> deletableMiddleware)
            {
                deletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public Satellite Read(Guid id)
        {
            return inner.Read(id);
        }

        public IEnumerable<Satellite> Read(IEnumerable<Guid> ids)
        {
            return inner.Read(ids);
        }

        public IEnumerable<Satellite> Read()
        {
            return inner.Read();
        }

        public IEnumerable<Satellite> Read(FilterElement<Satellite> filter)
        {
            if (middleware is IReadableMiddleware<Satellite> readableMiddleware)
            {
                return readableMiddleware.OnRead(filter, inner.Read);
            }

            return inner.Read(filter);
        }

        public IEnumerable<Satellite> Read(IQuery<Satellite> query)
        {
            if (middleware is IReadableMiddleware<Satellite> readableMiddleware)
            {
                return readableMiddleware.OnRead(query, inner.Read);
            }

            return inner.Read(query);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged()
        {
            return inner.ReadPaged();
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(int pageSize)
        {
            return inner.ReadPaged(pageSize);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(FilterElement<Satellite> filter)
        {
            if (middleware is IPageableMiddleware<Satellite> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, inner.ReadPaged);
            }

            return inner.ReadPaged(filter);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(IQuery<Satellite> query)
        {
            if (middleware is IPageableMiddleware<Satellite> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, inner.ReadPaged);
            }

            return inner.ReadPaged(query);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(FilterElement<Satellite> filter, int pageSize)
        {
            if (middleware is IPageableMiddleware<Satellite> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Satellite>> ReadPaged(IQuery<Satellite> query, int pageSize)
        {
            if (middleware is IPageableMiddleware<Satellite> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(query, pageSize);
        }

        public IReadOnlyCollection<Satellite> Update(IEnumerable<Satellite> oToUpdate)
        {
            if (middleware is IBulkUpdatableMiddleware<Satellite> bulkUpdatableMiddleware)
            {
                return bulkUpdatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public Satellite Update(Satellite oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<Satellite> updatableMiddleware)
            {
                return updatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public Satellite Activate(Guid id)
        {
            return inner.Activate(id);
        }

        public Satellite Activate(Satellite satellite)
        {
            return inner.Activate(satellite);
        }

        public IReadOnlyCollection<Satellite> Activate(IEnumerable<Satellite> satellites)
        {
            return inner.Activate(satellites);
        }

        public IReadOnlyCollection<Satellite> Activate(IEnumerable<Guid> satelliteIds)
        {
            return inner.Activate(satelliteIds);
        }

        public Satellite Deprecate(Guid id)
        {
            return inner.Deprecate(id);
        }

        public Satellite Deprecate(Satellite satellite)
        {
            return inner.Deprecate(satellite);
        }

        public IReadOnlyCollection<Satellite> Deprecate(IEnumerable<Satellite> satellites)
        {
            return inner.Deprecate(satellites);
        }

        public IReadOnlyCollection<Satellite> Deprecate(IEnumerable<Guid> satelliteIds)
        {
            return inner.Deprecate(satelliteIds);
        }

        public Satellite Reactivate(Guid id)
        {
            return inner.Reactivate(id);
        }

        public Satellite Reactivate(Satellite satellite)
        {
            return inner.Reactivate(satellite);
        }

        public IReadOnlyCollection<Satellite> Reactivate(IEnumerable<Satellite> satellites)
        {
            return inner.Reactivate(satellites);
        }

        public IReadOnlyCollection<Satellite> Reactivate(IEnumerable<Guid> satelliteIds)
        {
            return inner.Reactivate(satelliteIds);
        }
    }

    internal static class SatelliteRepositoryExtensions
    {
        public static ISatelliteRepository WithMiddleware(this ISatelliteRepository repository, IMiddlewareMarker<Satellite> middleware)
        {
            return new SatelliteRepositoryMiddleware(repository, middleware);
        }
    }
}
