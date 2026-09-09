namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;

    internal sealed class TransponderPlanRepositoryMiddleware : ITransponderPlanRepository
    {
        private readonly ITransponderPlanRepository inner;
        private readonly IMiddlewareMarker<TransponderPlan> middleware;

        public TransponderPlanRepositoryMiddleware(ITransponderPlanRepository inner, IMiddlewareMarker<TransponderPlan> middleware)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
        }

        public long Count()
        {
            return inner.Count();
        }

        public long Count(FilterElement<TransponderPlan> filter)
        {
            if (middleware is ICountableMiddleware<TransponderPlan> countableMiddleware)
            {
                return countableMiddleware.OnCount(filter, inner.Count);
            }

            return inner.Count(filter);
        }

        public long Count(IQuery<TransponderPlan> query)
        {
            if (middleware is ICountableMiddleware<TransponderPlan> countableMiddleware)
            {
                return countableMiddleware.OnCount(query, inner.Count);
            }

            return inner.Count(query);
        }

        public IReadOnlyCollection<TransponderPlan> Create(IEnumerable<TransponderPlan> oToCreate)
        {
            if (middleware is IBulkCreatableMiddleware<TransponderPlan> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public TransponderPlan Create(TransponderPlan oToCreate)
        {
            if (middleware is ICreatableMiddleware<TransponderPlan> creatableMiddleware)
            {
                return creatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public IReadOnlyCollection<TransponderPlan> CreateOrUpdate(IEnumerable<TransponderPlan> oToCreateOrUpdate)
        {
            if (middleware is IBulkRepositoryMiddleware<TransponderPlan> bulkRepositoryMiddleware)
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

        public void Delete(IEnumerable<TransponderPlan> oToDelete)
        {
            if (middleware is IBulkDeletableMiddleware<TransponderPlan> bulkDeletableMiddleware)
            {
                bulkDeletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public void Delete(TransponderPlan oToDelete)
        {
            if (middleware is IDeletableMiddleware<TransponderPlan> deletableMiddleware)
            {
                deletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public IEnumerable<TransponderPlan> Read()
        {
            return inner.Read();
        }

        public TransponderPlan Read(Guid id)
        {
            return inner.Read(id);
        }

        public IEnumerable<TransponderPlan> Read(IEnumerable<Guid> ids)
        {
            return inner.Read(ids);
        }

        public IEnumerable<TransponderPlan> ReadByTransponder(Guid transponderId)
        {
            return inner.ReadByTransponder(transponderId);
        }

        public IEnumerable<TransponderPlan> Read(FilterElement<TransponderPlan> filter)
        {
            if (middleware is IReadableMiddleware<TransponderPlan> readableMiddleware)
            {
                return readableMiddleware.OnRead(filter, inner.Read);
            }

            return inner.Read(filter);
        }

        public IEnumerable<TransponderPlan> Read(IQuery<TransponderPlan> query)
        {
            if (middleware is IReadableMiddleware<TransponderPlan> readableMiddleware)
            {
                return readableMiddleware.OnRead(query, inner.Read);
            }

            return inner.Read(query);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged()
        {
            return inner.ReadPaged();
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(int pageSize)
        {
            return inner.ReadPaged(pageSize);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(FilterElement<TransponderPlan> filter)
        {
            if (middleware is IPageableMiddleware<TransponderPlan> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, inner.ReadPaged);
            }

            return inner.ReadPaged(filter);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(IQuery<TransponderPlan> query)
        {
            if (middleware is IPageableMiddleware<TransponderPlan> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, inner.ReadPaged);
            }

            return inner.ReadPaged(query);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(FilterElement<TransponderPlan> filter, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderPlan> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> ReadPaged(IQuery<TransponderPlan> query, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderPlan> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(query, pageSize);
        }

        public IReadOnlyCollection<TransponderPlan> Update(IEnumerable<TransponderPlan> oToUpdate)
        {
            if (middleware is IBulkUpdatableMiddleware<TransponderPlan> bulkUpdatableMiddleware)
            {
                return bulkUpdatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public TransponderPlan Update(TransponderPlan oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<TransponderPlan> updatableMiddleware)
            {
                return updatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public TransponderPlan Activate(Guid id)
        {
            return inner.Activate(id);
        }

        public TransponderPlan Activate(TransponderPlan transponderPlan)
        {
            return inner.Activate(transponderPlan);
        }

        public IReadOnlyCollection<TransponderPlan> Activate(IEnumerable<TransponderPlan> transponderPlans)
        {
            return inner.Activate(transponderPlans);
        }

        public IReadOnlyCollection<TransponderPlan> Activate(IEnumerable<Guid> transponderPlanIds)
        {
            return inner.Activate(transponderPlanIds);
        }

        public TransponderPlan Deprecate(Guid id)
        {
            return inner.Deprecate(id);
        }

        public TransponderPlan Deprecate(TransponderPlan transponderPlan)
        {
            return inner.Deprecate(transponderPlan);
        }

        public IReadOnlyCollection<TransponderPlan> Deprecate(IEnumerable<TransponderPlan> transponderPlans)
        {
            return inner.Deprecate(transponderPlans);
        }

        public IReadOnlyCollection<TransponderPlan> Deprecate(IEnumerable<Guid> transponderPlanIds)
        {
            return inner.Deprecate(transponderPlanIds);
        }

        public TransponderPlan Reactivate(Guid id)
        {
            return inner.Reactivate(id);
        }

        public TransponderPlan Reactivate(TransponderPlan transponderPlan)
        {
            return inner.Reactivate(transponderPlan);
        }

        public IReadOnlyCollection<TransponderPlan> Reactivate(IEnumerable<TransponderPlan> transponderPlans)
        {
            return inner.Reactivate(transponderPlans);
        }

        public IReadOnlyCollection<TransponderPlan> Reactivate(IEnumerable<Guid> transponderPlanIds)
        {
            return inner.Reactivate(transponderPlanIds);
        }
    }

    internal static class TransponderPlanRepositoryExtensions
    {
        public static ITransponderPlanRepository WithMiddleware(this ITransponderPlanRepository repository, IMiddlewareMarker<TransponderPlan> middleware)
        {
            return new TransponderPlanRepositoryMiddleware(repository, middleware);
        }
    }
}
