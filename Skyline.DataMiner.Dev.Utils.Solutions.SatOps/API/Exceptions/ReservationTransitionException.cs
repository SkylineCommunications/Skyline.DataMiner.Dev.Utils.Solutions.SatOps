namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions
{
    using System;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;

    /// <summary>
    /// A persisted reservation could not complete a lifecycle transition.
    /// The reservation is not automatically deleted or rolled back.
    /// </summary>
    public sealed class ReservationTransitionException : InvalidOperationException
    {
        internal ReservationTransitionException(
            Guid reservationId,
            TransponderReservationState lastKnownState,
            string operation,
            string reason,
            Exception innerException = null)
            : base($"Reservation '{reservationId}' could not complete '{operation}'. Last known state: {lastKnownState}. {reason}", innerException)
        {
            ReservationId = reservationId;
            LastKnownState = lastKnownState;
            Operation = operation;
        }

        /// <summary>Gets the persisted reservation identifier.</summary>
        public Guid ReservationId { get; }

        /// <summary>
        /// Gets the last observed state. Read the reservation again to determine its current state.
        /// </summary>
        public TransponderReservationState LastKnownState { get; }

        /// <summary>Gets the MediaOps transition operation that did not complete successfully.</summary>
        public string Operation { get; }
    }
}
