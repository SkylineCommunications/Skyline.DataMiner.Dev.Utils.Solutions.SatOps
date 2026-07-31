namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using SLDataGateway.API.Types.Querying;

    internal sealed class TransponderRangeReservationRepositoryMiddleware : ITransponderRangeReservationRepository
    {
        private readonly ITransponderRangeReservationRepository inner;
        private readonly IMiddlewareMarker<TransponderRangeReservation> middleware;

        public TransponderRangeReservationRepositoryMiddleware(ITransponderRangeReservationRepository inner, IMiddlewareMarker<TransponderRangeReservation> middleware)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.middleware = middleware;
        }

        public TransponderRangeReservation Initialize()
        {
            return inner.Initialize();
        }

        public long Count()
        {
            return inner.Count();
        }

        public long Count(FilterElement<TransponderRangeReservation> filter)
        {
            if (middleware is ICountableMiddleware<TransponderRangeReservation> countableMiddleware)
            {
                return countableMiddleware.OnCount(filter, inner.Count);
            }

            return inner.Count(filter);
        }

        public long Count(IQuery<TransponderRangeReservation> query)
        {
            if (middleware is ICountableMiddleware<TransponderRangeReservation> countableMiddleware)
            {
                return countableMiddleware.OnCount(query, inner.Count);
            }

            return inner.Count(query);
        }

        public IReadOnlyCollection<TransponderRangeReservation> Create(IEnumerable<TransponderRangeReservation> oToCreate)
        {
            if (middleware is IBulkCreatableMiddleware<TransponderRangeReservation> bulkCreatableMiddleware)
            {
                return bulkCreatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public TransponderRangeReservation Create(TransponderRangeReservation oToCreate)
        {
            if (middleware is ICreatableMiddleware<TransponderRangeReservation> creatableMiddleware)
            {
                return creatableMiddleware.OnCreate(oToCreate, inner.Create);
            }

            return inner.Create(oToCreate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> CreateOrUpdate(IEnumerable<TransponderRangeReservation> oToCreateOrUpdate)
        {
            if (middleware is IBulkRepositoryMiddleware<TransponderRangeReservation> bulkRepositoryMiddleware)
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

        public void Delete(IEnumerable<TransponderRangeReservation> oToDelete)
        {
            if (middleware is IBulkDeletableMiddleware<TransponderRangeReservation> bulkDeletableMiddleware)
            {
                bulkDeletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public void Delete(TransponderRangeReservation oToDelete)
        {
            if (middleware is IDeletableMiddleware<TransponderRangeReservation> deletableMiddleware)
            {
                deletableMiddleware.OnDelete(oToDelete, inner.Delete);
                return;
            }

            inner.Delete(oToDelete);
        }

        public IEnumerable<TransponderRangeReservation> Read()
        {
            return inner.Read();
        }

        public IEnumerable<TransponderRangeReservation> ReadByTransponder(Guid transponderId)
        {
            return inner.ReadByTransponder(transponderId);
        }

        public IEnumerable<TransponderRangeReservation> ReadByTimeWindow(DateTime startTimeUtc, DateTime endTimeUtc)
        {
            return inner.ReadByTimeWindow(startTimeUtc, endTimeUtc);
        }

        public IEnumerable<TransponderRangeReservation> ReadByTransponderAndTimeWindow(Guid transponderId, DateTime startTimeUtc, DateTime endTimeUtc)
        {
            return inner.ReadByTransponderAndTimeWindow(transponderId, startTimeUtc, endTimeUtc);
        }

        public void ReserveRange(Guid reservationId, double startFrequency, double endFrequency)
        {
            inner.ReserveRange(reservationId, startFrequency, endFrequency);
        }

        public void ReserveRange(Guid reservationId, double startFrequency, double endFrequency, string satelliteName)
        {
            inner.ReserveRange(reservationId, startFrequency, endFrequency, satelliteName);
        }

        public void AddSlotNameProperty(Guid reservationId, string slotName)
        {
            inner.AddSlotNameProperty(reservationId, slotName);
        }

        public string GetSlotName(Guid reservationId)
        {
            return inner.GetSlotName(reservationId);
        }

        public void RefreshSlotNameNodeLink(Guid reservationId)
        {
            inner.RefreshSlotNameNodeLink(reservationId);
        }

        public string GetFirstNodeId(Guid reservationId)
        {
            return inner.GetFirstNodeId(reservationId);
        }

        public IEnumerable<TransponderRangeReservation> Read(FilterElement<TransponderRangeReservation> filter)
        {
            if (middleware is IReadableMiddleware<TransponderRangeReservation> readableMiddleware)
            {
                return readableMiddleware.OnRead(filter, inner.Read);
            }

            return inner.Read(filter);
        }

        public IEnumerable<TransponderRangeReservation> Read(IQuery<TransponderRangeReservation> query)
        {
            if (middleware is IReadableMiddleware<TransponderRangeReservation> readableMiddleware)
            {
                return readableMiddleware.OnRead(query, inner.Read);
            }

            return inner.Read(query);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged()
        {
            return inner.ReadPaged();
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(int pageSize)
        {
            return inner.ReadPaged(pageSize);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(FilterElement<TransponderRangeReservation> filter)
        {
            if (middleware is IPageableMiddleware<TransponderRangeReservation> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, inner.ReadPaged);
            }

            return inner.ReadPaged(filter);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(IQuery<TransponderRangeReservation> query)
        {
            if (middleware is IPageableMiddleware<TransponderRangeReservation> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, inner.ReadPaged);
            }

            return inner.ReadPaged(query);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(FilterElement<TransponderRangeReservation> filter, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderRangeReservation> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(filter, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(IQuery<TransponderRangeReservation> query, int pageSize)
        {
            if (middleware is IPageableMiddleware<TransponderRangeReservation> pageableMiddleware)
            {
                return pageableMiddleware.OnReadPaged(query, pageSize, inner.ReadPaged);
            }

            return inner.ReadPaged(query, pageSize);
        }

        public TransponderRangeReservation Read(Guid id)
        {
            return inner.Read(id);
        }

        public IEnumerable<TransponderRangeReservation> Read(IEnumerable<Guid> ids)
        {
            return inner.Read(ids);
        }

        public IReadOnlyCollection<TransponderRangeReservation> Update(IEnumerable<TransponderRangeReservation> oToUpdate)
        {
            if (middleware is IBulkUpdatableMiddleware<TransponderRangeReservation> bulkUpdatableMiddleware)
            {
                return bulkUpdatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }

        public TransponderRangeReservation Update(TransponderRangeReservation oToUpdate)
        {
            if (middleware is IUpdatableMiddleware<TransponderRangeReservation> updatableMiddleware)
            {
                return updatableMiddleware.OnUpdate(oToUpdate, inner.Update);
            }

            return inner.Update(oToUpdate);
        }
    }

    internal static class TransponderRangeReservationRepositoryExtensions
    {
        public static ITransponderRangeReservationRepository WithMiddleware(this ITransponderRangeReservationRepository repository, IMiddlewareMarker<TransponderRangeReservation> middleware)
        {
            return new TransponderRangeReservationRepositoryMiddleware(repository, middleware);
        }
    }
}
