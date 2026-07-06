namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;

    internal sealed class TransponderSlotRepositoryMiddleware : ITransponderSlotRepository
    {
        private readonly ITransponderSlotRepository inner;
        private readonly IMiddlewareMarker<TransponderSlot> middleware;

        public TransponderSlotRepositoryMiddleware(ITransponderSlotRepository inner, IMiddlewareMarker<TransponderSlot> middleware)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
        }

        public TransponderSlot Initialize()
        {
            return inner.Initialize();
        }

        public long Count()
        {
            return inner.Count();
        }

        public long Count(FilterElement<TransponderSlot> filter)
        {
            if (middleware is ICountableMiddleware<TransponderSlot> countableMiddleware)
            {
                return countableMiddleware.OnCount(filter, inner.Count);
            }

            return inner.Count(filter);
        }

        public long Count(IQuery<TransponderSlot> query)
        {
            if (middleware is ICountableMiddleware<TransponderSlot> countableMiddleware)
            {
                return countableMiddleware.OnCount(query, inner.Count);
            }

            return inner.Count(query);
        }

        public IReadOnlyCollection<TransponderSlot> Create(IEnumerable<TransponderSlot> oToCreate)
        {
            if (middleware is IBulkCreatableMiddleware<TransponderSlot> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public TransponderSlot Create(TransponderSlot oToCreate)
        {
            if (middleware is ICreatableMiddleware<TransponderSlot> creatableMiddleware)
            {
                return creatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> CreateOrUpdate(IEnumerable<TransponderSlot> oToCreateOrUpdate)
        {
            if (middleware is IBulkRepositoryMiddleware<TransponderSlot> bulkRepositoryMiddleware)
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

        public void Delete(IEnumerable<TransponderSlot> oToDelete)
        {
            if (middleware is IBulkDeletableMiddleware<TransponderSlot> bulkDeletableMiddleware)
            {
                bulkDeletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public void Delete(TransponderSlot oToDelete)
        {
            if (middleware is IDeletableMiddleware<TransponderSlot> deletableMiddleware)
            {
                deletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public IEnumerable<TransponderSlot> Read()
        {
            return inner.Read();
        }

        public IEnumerable<TransponderSlot> Read(FilterElement<TransponderSlot> filter)
        {
            if (middleware is IReadableMiddleware<TransponderSlot> readableMiddleware)
            {
                return readableMiddleware.OnRead(filter, inner.Read);
            }

            return inner.Read(filter);
        }

        public IEnumerable<TransponderSlot> Read(IQuery<TransponderSlot> query)
        {
            if (middleware is IReadableMiddleware<TransponderSlot> readableMiddleware)
            {
                return readableMiddleware.OnRead(query, inner.Read);
            }

            return inner.Read(query);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged()
        {
            return inner.ReadPaged();
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(int pageSize)
        {
            return inner.ReadPaged(pageSize);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter)
        {
            if (middleware is IPageableMiddleware<TransponderSlot> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, inner.ReadPaged);
            }

            return inner.ReadPaged(filter);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query)
        {
            if (middleware is IPageableMiddleware<TransponderSlot> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, inner.ReadPaged);
            }

            return inner.ReadPaged(query);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(FilterElement<TransponderSlot> filter, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderSlot> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> ReadPaged(IQuery<TransponderSlot> query, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderSlot> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(query, pageSize);
        }

        public TransponderSlot Read(Guid id)
        {
            return inner.Read(id);
        }

        public IEnumerable<TransponderSlot> Read(IEnumerable<Guid> ids)
        {
            return inner.Read(ids);
        }

        public IReadOnlyCollection<TransponderSlot> Update(IEnumerable<TransponderSlot> oToUpdate)
        {
            if (middleware is IBulkUpdatableMiddleware<TransponderSlot> bulkUpdatableMiddleware)
            {
                return bulkUpdatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public TransponderSlot Update(TransponderSlot oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<TransponderSlot> updatableMiddleware)
            {
                return updatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }
    }

    internal static class TransponderSlotRepositoryExtensions
    {
        public static ITransponderSlotRepository WithMiddleware(this ITransponderSlotRepository repository, IMiddlewareMarker<TransponderSlot> middleware)
        {
            return new TransponderSlotRepositoryMiddleware(repository, middleware);
        }
    }
}
