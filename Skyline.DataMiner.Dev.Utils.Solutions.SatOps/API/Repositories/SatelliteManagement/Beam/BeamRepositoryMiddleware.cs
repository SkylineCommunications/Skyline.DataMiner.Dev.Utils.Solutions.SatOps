namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Beam
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;

    internal sealed class BeamRepositoryMiddleware : IBeamRepository
    {
        private readonly IBeamRepository inner;
        private readonly IMiddlewareMarker<Beam> middleware;

        public BeamRepositoryMiddleware(IBeamRepository inner, IMiddlewareMarker<Beam> middleware)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
        }

        public Beam Initialize()
        {
            return inner.Initialize();
        }

        public long Count()
        {
            return inner.Count();
        }

        public long Count(FilterElement<Beam> filter)
        {
            if (middleware is ICountableMiddleware<Beam> countableMiddleware)
            {
                return countableMiddleware.OnCount(filter, inner.Count);
            }

            return inner.Count(filter);
        }

        public long Count(IQuery<Beam> query)
        {
            if (middleware is ICountableMiddleware<Beam> countableMiddleware)
            {
                return countableMiddleware.OnCount(query, inner.Count);
            }

            return inner.Count(query);
        }

        public IReadOnlyCollection<Beam> Create(IEnumerable<Beam> oToCreate)
        {
            if (middleware is IBulkCreatableMiddleware<Beam> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public Beam Create(Beam oToCreate)
        {
            if (middleware is ICreatableMiddleware<Beam> creatableMiddleware)
            {
                return creatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public IReadOnlyCollection<Beam> CreateOrUpdate(IEnumerable<Beam> oToCreateOrUpdate)
        {
            if (middleware is IBulkRepositoryMiddleware<Beam> bulkRepositoryMiddleware)
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

        public void Delete(IEnumerable<Beam> oToDelete)
        {
            if (middleware is IBulkDeletableMiddleware<Beam> bulkDeletableMiddleware)
            {
                bulkDeletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public void Delete(Beam oToDelete)
        {
            if (middleware is IDeletableMiddleware<Beam> deletableMiddleware)
            {
                deletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public IEnumerable<Beam> Read(FilterElement<Beam> filter)
        {
            if (middleware is IReadableMiddleware<Beam> readableMiddleware)
            {
                return readableMiddleware.OnRead(filter, inner.Read);
            }

            return inner.Read(filter);
        }

        public IEnumerable<Beam> Read(IQuery<Beam> query)
        {
            if (middleware is IReadableMiddleware<Beam> readableMiddleware)
            {
                return readableMiddleware.OnRead(query, inner.Read);
            }

            return inner.Read(query);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged()
        {
            return inner.ReadPaged();
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(int pageSize)
        {
            return inner.ReadPaged(pageSize);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(FilterElement<Beam> filter)
        {
            if (middleware is IPageableMiddleware<Beam> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, inner.ReadPaged);
            }

            return inner.ReadPaged(filter);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(IQuery<Beam> query)
        {
            if (middleware is IPageableMiddleware<Beam> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, inner.ReadPaged);
            }

            return inner.ReadPaged(query);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(FilterElement<Beam> filter, int pageSize)
        {
            if (middleware is IPageableMiddleware<Beam> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Beam>> ReadPaged(IQuery<Beam> query, int pageSize)
        {
            if (middleware is IPageableMiddleware<Beam> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(query, pageSize);
        }

        public Beam Read(Guid id)
        {
            return inner.Read(id);
        }

        public IEnumerable<Beam> Read(IEnumerable<Guid> ids)
        {
            return inner.Read(ids);
        }

        public IEnumerable<Beam> Read()
        {
            return inner.Read();
        }

        public IReadOnlyCollection<Beam> Update(IEnumerable<Beam> oToUpdate)
        {
            if (middleware is IBulkUpdatableMiddleware<Beam> bulkUpdatableMiddleware)
            {
                return bulkUpdatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public Beam Update(Beam oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<Beam> updatableMiddleware)
            {
                return updatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public Beam Activate(Guid id)
        {
            return inner.Activate(id);
        }

        public Beam Activate(Beam beam)
        {
            return inner.Activate(beam);
        }

        public IReadOnlyCollection<Beam> Activate(IEnumerable<Beam> beams)
        {
            return inner.Activate(beams);
        }

        public IReadOnlyCollection<Beam> Activate(IEnumerable<Guid> beamIds)
        {
            return inner.Activate(beamIds);
        }

        public Beam Deprecate(Guid id)
        {
            return inner.Deprecate(id);
        }

        public Beam Deprecate(Beam beam)
        {
            return inner.Deprecate(beam);
        }

        public IReadOnlyCollection<Beam> Deprecate(IEnumerable<Beam> beams)
        {
            return inner.Deprecate(beams);
        }

        public IReadOnlyCollection<Beam> Deprecate(IEnumerable<Guid> beamIds)
        {
            return inner.Deprecate(beamIds);
        }

        public Beam Reactivate(Guid id)
        {
            return inner.Reactivate(id);
        }

        public Beam Reactivate(Beam beam)
        {
            return inner.Reactivate(beam);
        }

        public IReadOnlyCollection<Beam> Reactivate(IEnumerable<Beam> beams)
        {
            return inner.Reactivate(beams);
        }

        public IReadOnlyCollection<Beam> Reactivate(IEnumerable<Guid> beamIds)
        {
            return inner.Reactivate(beamIds);
        }
    }

    internal static class BeamRepositoryExtensions
    {
        public static IBeamRepository WithMiddleware(this IBeamRepository repository, IMiddlewareMarker<Beam> middleware)
        {
            return new BeamRepositoryMiddleware(repository, middleware);
        }
    }
}
