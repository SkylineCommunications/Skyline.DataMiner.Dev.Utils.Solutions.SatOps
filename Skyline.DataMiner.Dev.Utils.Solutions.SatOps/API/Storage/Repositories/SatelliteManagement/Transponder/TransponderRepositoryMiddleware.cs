
namespace Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories.SatelliteManagement.Transponder
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;

    internal sealed class TransponderRepositoryMiddleware : ITransponderRepository
    {
        private readonly ITransponderRepository inner;
        private readonly IMiddlewareMarker<Transponder> middleware;

        public TransponderRepositoryMiddleware(ITransponderRepository inner, IMiddlewareMarker<Transponder> middleware)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
        }

        public Transponder Initialize()
        {
            return inner.Initialize();
        }

        public long Count()
        {
            return inner.Count();
        }

        public long Count(FilterElement<Transponder> filter)
        {
            if (middleware is ICountableMiddleware<Transponder> countableMiddleware)
            {
                return countableMiddleware.OnCount(filter, inner.Count);
            }

            return inner.Count(filter);
        }

        public long Count(IQuery<Transponder> query)
        {
            if (middleware is ICountableMiddleware<Transponder> countableMiddleware)
            {
                return countableMiddleware.OnCount(query, inner.Count);
            }

            return inner.Count(query);
        }

        public IReadOnlyCollection<Transponder> Create(IEnumerable<Transponder> oToCreate)
        {
            if (middleware is IBulkCreatableMiddleware<Transponder> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public Transponder Create(Transponder oToCreate)
        {
            if (middleware is ICreatableMiddleware<Transponder> creatableMiddleware)
            {
                return creatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public IReadOnlyCollection<Transponder> CreateOrUpdate(IEnumerable<Transponder> oToCreateOrUpdate)
        {
            if (middleware is IBulkRepositoryMiddleware<Transponder> bulkRepositoryMiddleware)
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

        public void Delete(IEnumerable<Transponder> oToDelete)
        {
            if (middleware is IBulkDeletableMiddleware<Transponder> bulkDeletableMiddleware)
            {
                bulkDeletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public void Delete(Transponder oToDelete)
        {
            if (middleware is IDeletableMiddleware<Transponder> deletableMiddleware)
            {
                deletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public IEnumerable<Transponder> Read()
        {
            return inner.Read();
        }

        public IEnumerable<Transponder> Read(FilterElement<Transponder> filter)
        {
            if (middleware is IReadableMiddleware<Transponder> readableMiddleware)
            {
                return readableMiddleware.OnRead(filter, inner.Read);
            }

            return inner.Read(filter);
        }

        public IEnumerable<Transponder> Read(IQuery<Transponder> query)
        {
            if (middleware is IReadableMiddleware<Transponder> readableMiddleware)
            {
                return readableMiddleware.OnRead(query, inner.Read);
            }

            return inner.Read(query);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged()
        {
            return inner.ReadPaged();
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(int pageSize)
        {
            return inner.ReadPaged(pageSize);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(FilterElement<Transponder> filter)
        {
            if (middleware is IPageableMiddleware<Transponder> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, inner.ReadPaged);
            }

            return inner.ReadPaged(filter);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(IQuery<Transponder> query)
        {
            if (middleware is IPageableMiddleware<Transponder> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, inner.ReadPaged);
            }

            return inner.ReadPaged(query);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(FilterElement<Transponder> filter, int pageSize)
        {
            if (middleware is IPageableMiddleware<Transponder> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Transponder>> ReadPaged(IQuery<Transponder> query, int pageSize)
        {
            if (middleware is IPageableMiddleware<Transponder> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(query, pageSize);
        }

        public Transponder Read(Guid id)
        {
            return inner.Read(id);
        }

        public IEnumerable<Transponder> Read(IEnumerable<Guid> ids)
        {
            return inner.Read(ids);
        }

        public IReadOnlyCollection<Transponder> Update(IEnumerable<Transponder> oToUpdate)
        {
            if (middleware is IBulkUpdatableMiddleware<Transponder> bulkUpdatableMiddleware)
            {
                return bulkUpdatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public Transponder Update(Transponder oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<Transponder> updatableMiddleware)
            {
                return updatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public Transponder Activate(Guid id)
        {
            return inner.Activate(id);
        }

        public Transponder Activate(Transponder transponder)
        {
            return inner.Activate(transponder);
        }

        public IReadOnlyCollection<Transponder> Activate(IEnumerable<Transponder> transponders)
        {
            return inner.Activate(transponders);
        }

        public IReadOnlyCollection<Transponder> Activate(IEnumerable<Guid> transponderIds)
        {
            return inner.Activate(transponderIds);
        }

        public Transponder Deprecate(Guid id)
        {
            return inner.Deprecate(id);
        }

        public Transponder Deprecate(Transponder transponder)
        {
            return inner.Deprecate(transponder);
        }

        public IReadOnlyCollection<Transponder> Deprecate(IEnumerable<Transponder> transponders)
        {
            return inner.Deprecate(transponders);
        }

        public IReadOnlyCollection<Transponder> Deprecate(IEnumerable<Guid> transponderIds)
        {
            return inner.Deprecate(transponderIds);
        }

        public Transponder Reactivate(Guid id)
        {
            return inner.Reactivate(id);
        }

        public Transponder Reactivate(Transponder transponder)
        {
            return inner.Reactivate(transponder);
        }

        public IReadOnlyCollection<Transponder> Reactivate(IEnumerable<Transponder> transponders)
        {
            return inner.Reactivate(transponders);
        }

        public IReadOnlyCollection<Transponder> Reactivate(IEnumerable<Guid> transponderIds)
        {
            return inner.Reactivate(transponderIds);
        }
    }

    internal static class TransponderRepositoryExtensions
    {
        public static ITransponderRepository WithMiddleware(this ITransponderRepository repository, IMiddlewareMarker<Transponder> middleware)
        {
            return new TransponderRepositoryMiddleware(repository, middleware);
        }
    }
}
