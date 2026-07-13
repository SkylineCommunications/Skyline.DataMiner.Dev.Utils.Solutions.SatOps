namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;

    internal sealed class TransponderRangeReservationValidationMiddleware : IBulkRepositoryMiddleware<TransponderRangeReservation>
    {
        private readonly Func<Guid, Transponder> transponderResolver;

        public TransponderRangeReservationValidationMiddleware(Func<Guid, Transponder> transponderResolver)
        {
            this.transponderResolver = transponderResolver ?? throw new ArgumentNullException(nameof(transponderResolver));
        }

        public TransponderRangeReservation OnCreate(TransponderRangeReservation oToCreate, Func<TransponderRangeReservation, TransponderRangeReservation> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateReservation(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> OnCreate(IEnumerable<TransponderRangeReservation> oToCreate, Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var reservation in oToCreate)
            {
                ValidateReservation(reservation);
            }

            return next(oToCreate);
        }

        public TransponderRangeReservation OnUpdate(TransponderRangeReservation oToUpdate, Func<TransponderRangeReservation, TransponderRangeReservation> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateReservation(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> OnUpdate(IEnumerable<TransponderRangeReservation> oToUpdate, Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var reservation in oToUpdate)
            {
                ValidateReservation(reservation);
            }

            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> OnCreateOrUpdate(IEnumerable<TransponderRangeReservation> oToCreateOrUpdate, Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var reservation in oToCreateOrUpdate)
            {
                ValidateReservation(reservation);
            }

            return next(oToCreateOrUpdate);
        }

        public long OnCount(FilterElement<TransponderRangeReservation> filter, Func<FilterElement<TransponderRangeReservation>, long> next)
        {
            return next(filter);
        }

        public long OnCount(IQuery<TransponderRangeReservation> query, Func<IQuery<TransponderRangeReservation>, long> next)
        {
            return next(query);
        }

        public void OnDelete(IEnumerable<TransponderRangeReservation> oToDelete, Action<IEnumerable<TransponderRangeReservation>> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            next(oToDelete);
        }

        public void OnDelete(TransponderRangeReservation oToDelete, Action<TransponderRangeReservation> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            next(oToDelete);
        }

        public IEnumerable<TransponderRangeReservation> OnRead(FilterElement<TransponderRangeReservation> filter, Func<FilterElement<TransponderRangeReservation>, IEnumerable<TransponderRangeReservation>> next)
        {
            return next(filter);
        }

        public IEnumerable<TransponderRangeReservation> OnRead(IQuery<TransponderRangeReservation> query, Func<IQuery<TransponderRangeReservation>, IEnumerable<TransponderRangeReservation>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> OnReadPaged(FilterElement<TransponderRangeReservation> filter, Func<FilterElement<TransponderRangeReservation>, IEnumerable<IPagedResult<TransponderRangeReservation>>> next)
        {
            return next(filter);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> OnReadPaged(IQuery<TransponderRangeReservation> query, Func<IQuery<TransponderRangeReservation>, IEnumerable<IPagedResult<TransponderRangeReservation>>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> OnReadPaged(FilterElement<TransponderRangeReservation> filter, int pageSize, Func<FilterElement<TransponderRangeReservation>, int, IEnumerable<IPagedResult<TransponderRangeReservation>>> next)
        {
            return next(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> OnReadPaged(IQuery<TransponderRangeReservation> query, int pageSize, Func<IQuery<TransponderRangeReservation>, int, IEnumerable<IPagedResult<TransponderRangeReservation>>> next)
        {
            return next(query, pageSize);
        }

        private void ValidateReservation(TransponderRangeReservation reservation)
        {
            if (reservation == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(reservation));

            if (!reservation.Transponder.HasValue || reservation.Transponder.Value == Guid.Empty)
                throw new ArgumentException("Transponder is required.", nameof(reservation));

            if (!reservation.RelativeStartFrequency.HasValue)
                throw new ArgumentException("Relative Start Frequency is required.", nameof(reservation));

            if (!reservation.RelativeEndFrequency.HasValue)
                throw new ArgumentException("Relative End Frequency is required.", nameof(reservation));

            if (!reservation.StartTime.HasValue)
                throw new ArgumentException("Start Time is required.", nameof(reservation));

            if (!reservation.EndTime.HasValue)
                throw new ArgumentException("End Time is required.", nameof(reservation));

            if (reservation.RelativeStartFrequency.Value >= reservation.RelativeEndFrequency.Value)
                throw new ArgumentException("Relative Start Frequency must be lower than Relative End Frequency.", nameof(reservation));

            if (reservation.StartTime.Value >= reservation.EndTime.Value)
                throw new ArgumentException("Start Time must be earlier than End Time.", nameof(reservation));

            var transponderId = reservation.Transponder.Value;
            if (transponderResolver(transponderId) == null)
                throw new ArgumentException($"Transponder with id '{transponderId}' does not exist.", nameof(reservation));
        }
    }
}
