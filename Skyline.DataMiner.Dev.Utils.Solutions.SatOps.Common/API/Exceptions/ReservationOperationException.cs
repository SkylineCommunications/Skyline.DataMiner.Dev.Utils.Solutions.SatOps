namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;

    /// <summary>
    /// A reservation operation was rejected by MediaOps lifecycle validation or the SatOps
    /// Active allocation policy. MediaOps failures are retained as the inner exception.
    /// </summary>
    public sealed class ReservationOperationException : InvalidOperationException
    {
        internal ReservationOperationException(
            Guid reservationId,
            TransponderReservationState lastKnownState,
            string operation,
            IReadOnlyCollection<ReservationOperationError> errors,
            Exception innerException = null)
            : base($"Booking '{reservationId}' cannot complete '{operation}' (last observed state: {lastKnownState}). {string.Join(" ", errors.Select(error => error.Message))}", innerException)
        {
            ReservationId = reservationId;
            LastKnownState = lastKnownState;
            Operation = operation;
            Errors = errors.ToList().AsReadOnly();
        }

        /// <summary>Gets the reservation identifier.</summary>
        public Guid ReservationId { get; }

        /// <summary>
        /// Gets the last observed state, not a guarantee of the current persisted state.
        /// </summary>
        public TransponderReservationState LastKnownState { get; }

        /// <summary>Gets the operation that was rejected.</summary>
        public string Operation { get; }

        /// <summary>Gets all reasons returned for the rejected operation.</summary>
        public IReadOnlyCollection<ReservationOperationError> Errors { get; }
    }
}
