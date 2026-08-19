namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;

    internal sealed class TransponderPlanRowRepositoryMiddleware : ITransponderPlanRowRepository
    {
        private readonly ITransponderPlanRowRepository inner;
        private readonly IMiddlewareMarker<TransponderPlanRow> middleware;

        public TransponderPlanRowRepositoryMiddleware(ITransponderPlanRowRepository inner, IMiddlewareMarker<TransponderPlanRow> middleware)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
        }

        public TransponderPlanRow Initialize()
        {
            return inner.Initialize();
        }

        public long Count()
        {
            return inner.Count();
        }

        public long Count(FilterElement<TransponderPlanRow> filter)
        {
            if (middleware is ICountableMiddleware<TransponderPlanRow> countableMiddleware)
            {
                return countableMiddleware.OnCount(filter, inner.Count);
            }

            return inner.Count(filter);
        }

        public long Count(IQuery<TransponderPlanRow> query)
        {
            if (middleware is ICountableMiddleware<TransponderPlanRow> countableMiddleware)
            {
                return countableMiddleware.OnCount(query, inner.Count);
            }

            return inner.Count(query);
        }

        public IReadOnlyCollection<TransponderPlanRow> Create(IEnumerable<TransponderPlanRow> oToCreate)
        {
            if (middleware is IBulkCreatableMiddleware<TransponderPlanRow> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public TransponderPlanRow Create(TransponderPlanRow oToCreate)
        {
            if (middleware is ICreatableMiddleware<TransponderPlanRow> creatableMiddleware)
            {
                return creatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public IReadOnlyCollection<TransponderPlanRow> CreateOrUpdate(IEnumerable<TransponderPlanRow> oToCreateOrUpdate)
        {
            if (middleware is IBulkRepositoryMiddleware<TransponderPlanRow> bulkRepositoryMiddleware)
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

        public void Delete(IEnumerable<TransponderPlanRow> oToDelete)
        {
            if (middleware is IBulkDeletableMiddleware<TransponderPlanRow> bulkDeletableMiddleware)
            {
                bulkDeletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public void Delete(TransponderPlanRow oToDelete)
        {
            if (middleware is IDeletableMiddleware<TransponderPlanRow> deletableMiddleware)
            {
                deletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public IEnumerable<TransponderPlanRow> Read()
        {
            return inner.Read();
        }

        public IEnumerable<TransponderPlanRow> Read(FilterElement<TransponderPlanRow> filter)
        {
            if (middleware is IReadableMiddleware<TransponderPlanRow> readableMiddleware)
            {
                return readableMiddleware.OnRead(filter, inner.Read);
            }

            return inner.Read(filter);
        }

        public IEnumerable<TransponderPlanRow> Read(IQuery<TransponderPlanRow> query)
        {
            if (middleware is IReadableMiddleware<TransponderPlanRow> readableMiddleware)
            {
                return readableMiddleware.OnRead(query, inner.Read);
            }

            return inner.Read(query);
        }

        public TransponderPlanRow Read(Guid id)
        {
            return inner.Read(id);
        }

        public IEnumerable<TransponderPlanRow> Read(IEnumerable<Guid> ids)
        {
            return inner.Read(ids);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged()
        {
            return inner.ReadPaged();
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(int pageSize)
        {
            return inner.ReadPaged(pageSize);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(FilterElement<TransponderPlanRow> filter)
        {
            if (middleware is IPageableMiddleware<TransponderPlanRow> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, inner.ReadPaged);
            }

            return inner.ReadPaged(filter);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(IQuery<TransponderPlanRow> query)
        {
            if (middleware is IPageableMiddleware<TransponderPlanRow> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, inner.ReadPaged);
            }

            return inner.ReadPaged(query);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(FilterElement<TransponderPlanRow> filter, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderPlanRow> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> ReadPaged(IQuery<TransponderPlanRow> query, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderPlanRow> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(query, pageSize);
        }

        public IReadOnlyCollection<TransponderPlanRow> Update(IEnumerable<TransponderPlanRow> oToUpdate)
        {
            if (middleware is IBulkUpdatableMiddleware<TransponderPlanRow> bulkUpdatableMiddleware)
            {
                return bulkUpdatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public TransponderPlanRow Update(TransponderPlanRow oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<TransponderPlanRow> updatableMiddleware)
            {
                return updatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }
    }

    internal static class TransponderPlanRowRepositoryExtensions
    {
        public static ITransponderPlanRowRepository WithMiddleware(this ITransponderPlanRowRepository repository, IMiddlewareMarker<TransponderPlanRow> middleware)
        {
            return new TransponderPlanRowRepositoryMiddleware(repository, middleware);
        }
    }
}
