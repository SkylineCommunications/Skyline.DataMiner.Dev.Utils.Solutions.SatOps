namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation
{
    /// <summary>
    /// The lifecycle state of a reservation when it was read from MediaOps.
    /// </summary>
    public enum TransponderReservationState
    {
        /// <summary>The reservation has not been read or persisted.</summary>
        Unknown,

        /// <summary>The job is a draft and does not reserve resources.</summary>
        Draft,

        /// <summary>The reservation is tentative.</summary>
        Tentative,

        /// <summary>The reservation is confirmed.</summary>
        Confirmed,

        /// <summary>The reservation is running, including its pre-roll and post-roll.</summary>
        Active,

        /// <summary>The reservation has completed.</summary>
        Completed,

        /// <summary>The reservation has been canceled.</summary>
        Canceled,
    }
}
