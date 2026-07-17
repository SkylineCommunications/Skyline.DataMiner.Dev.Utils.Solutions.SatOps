namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying.TransponderRangeReservation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;

    /// <summary>
    /// Translates <see cref="TransponderRangeReservation"/> filters into MediaOps.Plan <see cref="Job"/>
    /// filters (limited to Id/Name, the only exposers Jobs supports natively) and provides a
    /// client-side evaluator that applies the full filter tree against materialized reservations.
    /// </summary>
    internal class TransponderRangeReservationFilterTranslator
    {
        private static readonly Dictionary<string, Func<TransponderRangeReservation, Comparer, object, bool>> Predicates =
            new Dictionary<string, Func<TransponderRangeReservation, Comparer, object, bool>>
            {
                [TransponderRangeReservationExposers.ReservationName.fieldName] = (r, c, v) => CompareString(r.Name, c, (string)v),
                [TransponderRangeReservationExposers.Transponder.fieldName] = (r, c, v) => CompareGuid(r.Transponder, c, (Guid)v),
                [TransponderRangeReservationExposers.RelativeStartFrequency.fieldName] = (r, c, v) => CompareDouble(r.RelativeStartFrequency, c, (double)v),
                [TransponderRangeReservationExposers.RelativeEndFrequency.fieldName] = (r, c, v) => CompareDouble(r.RelativeEndFrequency, c, (double)v),
                [TransponderRangeReservationExposers.StartTime.fieldName] = (r, c, v) => CompareDateTime(r.StartTime, c, (DateTime)v),
                [TransponderRangeReservationExposers.EndTime.fieldName] = (r, c, v) => CompareDateTime(r.EndTime, c, (DateTime)v),
            };

        /// <summary>
        /// Translates a reservation filter into a Job filter that can be sent to <see cref="IJobsRepository"/>.
        /// Because <c>JobExposers</c> only supports Id and Name, this returns a
        /// <see cref="TRUEFilterElement{Job}"/> for any other field; the repository then narrows the
        /// results client-side via <see cref="ApplyClientSide"/>.
        /// </summary>
        public FilterElement<Job> Translate(FilterElement<TransponderRangeReservation> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new TRUEFilterElement<Job>();
        }

        /// <summary>
        /// Applies the filter tree client-side to already-materialized reservations.
        /// </summary>
        public IEnumerable<TransponderRangeReservation> ApplyClientSide(
            FilterElement<TransponderRangeReservation> filter,
            IEnumerable<TransponderRangeReservation> reservations)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (reservations == null)
            {
                return Enumerable.Empty<TransponderRangeReservation>();
            }

            return reservations.Where(r => Evaluate(filter, r));
        }

        private static bool Evaluate(FilterElement<TransponderRangeReservation> filter, TransponderRangeReservation reservation)
        {
            switch (filter)
            {
                case TRUEFilterElement<TransponderRangeReservation> _:
                    return true;

                case FALSEFilterElement<TransponderRangeReservation> _:
                    return false;

                case ANDFilterElement<TransponderRangeReservation> and:
                    return and.subFilters.All(sub => Evaluate(sub, reservation));

                case ORFilterElement<TransponderRangeReservation> or:
                    return or.subFilters.Any(sub => Evaluate(sub, reservation));

                case NOTFilterElement<TransponderRangeReservation> not:
                    return !Evaluate(not.original, reservation);

                case ManagedFilterIdentifier managed:
                    return EvaluateManaged(managed, reservation);

                default:
                    throw new NotSupportedException($"Unsupported filter: {filter}");
            }
        }

        private static bool EvaluateManaged(ManagedFilterIdentifier managed, TransponderRangeReservation reservation)
        {
            var fieldName = managed.getFieldName().fieldName;
            if (!Predicates.TryGetValue(fieldName, out var predicate))
            {
                throw new NotSupportedException($"Unsupported field: {fieldName}");
            }

            return predicate(reservation, managed.getComparer(), managed.getValue());
        }

        private static bool CompareString(string actual, Comparer comparer, string expected)
        {
            var cmp = string.Compare(actual, expected, StringComparison.Ordinal);
            return ApplyComparer(cmp, actual == null && expected == null, comparer);
        }

        private static bool CompareGuid(Guid? actual, Comparer comparer, Guid expected)
        {
            if (!actual.HasValue)
            {
                return comparer == Comparer.NotEquals;
            }

            var cmp = actual.Value.CompareTo(expected);
            return ApplyComparer(cmp, actual.Value == expected, comparer);
        }

        private static bool CompareDouble(double? actual, Comparer comparer, double expected)
        {
            if (!actual.HasValue)
            {
                return comparer == Comparer.NotEquals;
            }

            var cmp = actual.Value.CompareTo(expected);
            return ApplyComparer(cmp, actual.Value == expected, comparer);
        }

        private static bool CompareDateTime(DateTime? actual, Comparer comparer, DateTime expected)
        {
            if (!actual.HasValue)
            {
                return comparer == Comparer.NotEquals;
            }

            var cmp = actual.Value.CompareTo(expected);
            return ApplyComparer(cmp, actual.Value == expected, comparer);
        }

        private static bool ApplyComparer(int compareResult, bool equal, Comparer comparer)
        {
            switch (comparer)
            {
                case Comparer.Equals:
                    return equal;
                case Comparer.NotEquals:
                    return !equal;
                case Comparer.LT:
                    return compareResult < 0;
                case Comparer.LTE:
                    return compareResult <= 0;
                case Comparer.GT:
                    return compareResult > 0;
                case Comparer.GTE:
                    return compareResult >= 0;
                default:
                    throw new NotSupportedException($"Unsupported comparer: {comparer}");
            }
        }
    }
}
