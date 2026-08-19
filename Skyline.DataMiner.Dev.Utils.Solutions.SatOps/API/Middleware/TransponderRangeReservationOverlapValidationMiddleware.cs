namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Middleware that validates there are no overlapping reservations on the same transponder.
    /// </summary>
    internal sealed class TransponderRangeReservationOverlapValidationMiddleware : IBulkRepositoryMiddleware<TransponderRangeReservation>
    {
        private const double OverlapTolerance = 1e-9;

        private readonly Func<Guid, IEnumerable<TransponderRangeReservation>> existingReservationsResolver;

        public TransponderRangeReservationOverlapValidationMiddleware(Func<Guid, IEnumerable<TransponderRangeReservation>> existingReservationsResolver)
        {
            this.existingReservationsResolver = existingReservationsResolver ?? throw new ArgumentNullException(nameof(existingReservationsResolver));
        }

        public TransponderRangeReservation OnCreate(TransponderRangeReservation oToCreate, Func<TransponderRangeReservation, TransponderRangeReservation> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateAgainstExisting(oToCreate, Guid.Empty);
            return next(oToCreate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> OnCreate(IEnumerable<TransponderRangeReservation> oToCreate, Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBatch(oToCreate);
            return next(oToCreate);
        }

        public TransponderRangeReservation OnUpdate(TransponderRangeReservation oToUpdate, Func<TransponderRangeReservation, TransponderRangeReservation> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateAgainstExisting(oToUpdate, oToUpdate.Id);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> OnUpdate(IEnumerable<TransponderRangeReservation> oToUpdate, Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBatch(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderRangeReservation> OnCreateOrUpdate(IEnumerable<TransponderRangeReservation> oToCreateOrUpdate, Func<IEnumerable<TransponderRangeReservation>, IReadOnlyCollection<TransponderRangeReservation>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBatch(oToCreateOrUpdate);
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
            next(oToDelete);
        }

        public void OnDelete(TransponderRangeReservation oToDelete, Action<TransponderRangeReservation> next)
        {
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

        private void ValidateAgainstExisting(TransponderRangeReservation reservation, Guid excludeId)
        {
            if (!HasRequiredRanges(reservation))
            {
                return;
            }

            var existingReservations = (existingReservationsResolver(reservation.Transponder.Value) ?? Enumerable.Empty<TransponderRangeReservation>())
                .Where(existing => existing != null
                    && existing.Id != excludeId
                    && HasRequiredRanges(existing));

            var overlappingReservation = existingReservations
                .FirstOrDefault(existingReservation => ReservationsOverlap(reservation, existingReservation));

            if (overlappingReservation != null)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.ReservationOverlapDetected, reservation.Name, overlappingReservation.Name),
                    nameof(reservation));
            }
        }

        private static void ValidateBatch(IEnumerable<TransponderRangeReservation> reservations)
        {
            var validReservations = reservations.Where(HasRequiredRanges).ToList();
            var groups = validReservations.GroupBy(reservation => reservation.Transponder.Value);
            foreach (var group in groups)
            {
                var sorted = group.OrderBy(reservation => reservation.StartTime.Value).ToList();
                for (var index = 0; index < sorted.Count - 1; index++)
                {
                    var current = sorted[index];
                    var next = sorted[index + 1];

                    if (ReservationsOverlap(current, next))
                    {
                        throw new ArgumentException(string.Format(ExceptionMessages.ReservationOverlapDetected, current.Name, next.Name));
                    }
                }
            }
        }

        private static bool ReservationsOverlap(TransponderRangeReservation first, TransponderRangeReservation second)
        {
            return TimeRangesOverlap(first.StartTime.Value, first.EndTime.Value, second.StartTime.Value, second.EndTime.Value)
                && FrequencyRangesOverlap(first.RelativeStartFrequency.Value, first.RelativeEndFrequency.Value, second.RelativeStartFrequency.Value, second.RelativeEndFrequency.Value);
        }

        private static bool TimeRangesOverlap(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd)
        {
            return firstStart < secondEnd && firstEnd > secondStart;
        }

        private static bool FrequencyRangesOverlap(double firstStart, double firstEnd, double secondStart, double secondEnd)
        {
            return firstStart < secondEnd - OverlapTolerance && firstEnd > secondStart + OverlapTolerance;
        }

        private static bool HasRequiredRanges(TransponderRangeReservation reservation)
        {
            return reservation != null
                && reservation.Transponder.HasValue
                && reservation.Transponder.Value != Guid.Empty
                && reservation.StartTime.HasValue
                && reservation.EndTime.HasValue
                && reservation.RelativeStartFrequency.HasValue
                && reservation.RelativeEndFrequency.HasValue;
        }
    }
}
