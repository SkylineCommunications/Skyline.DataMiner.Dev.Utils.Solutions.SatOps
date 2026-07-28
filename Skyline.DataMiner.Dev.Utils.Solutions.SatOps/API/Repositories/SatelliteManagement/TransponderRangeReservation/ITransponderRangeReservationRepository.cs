namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories;

    /// <summary>
    /// Represents a repository for managing <see cref="TransponderRangeReservation"/> entities.
    /// </summary>
    public interface ITransponderRangeReservationRepository : IRepository<TransponderRangeReservation>
    {
        /// <summary>
        /// Initializes a new <see cref="TransponderRangeReservation"/> instance.
        /// </summary>
        /// <returns>A new <see cref="TransponderRangeReservation"/> instance.</returns>
        TransponderRangeReservation Initialize();

        /// <summary>
        /// Reads all reservations for a transponder.
        /// </summary>
        /// <param name="transponderId">The transponder identifier.</param>
        /// <returns>The matching reservations.</returns>
        IEnumerable<TransponderRangeReservation> ReadByTransponder(Guid transponderId);

        /// <summary>
        /// Reads all reservations that overlap the specified time window.
        /// </summary>
        /// <param name="startTimeUtc">The inclusive window start (UTC).</param>
        /// <param name="endTimeUtc">The exclusive window end (UTC).</param>
        /// <returns>The matching reservations.</returns>
        IEnumerable<TransponderRangeReservation> ReadByTimeWindow(DateTime startTimeUtc, DateTime endTimeUtc);

        /// <summary>
        /// Reads all reservations for a transponder that overlap the specified time window.
        /// </summary>
        /// <param name="transponderId">The transponder identifier.</param>
        /// <param name="startTimeUtc">The inclusive window start (UTC).</param>
        /// <param name="endTimeUtc">The exclusive window end (UTC).</param>
        /// <returns>The matching reservations.</returns>
        IEnumerable<TransponderRangeReservation> ReadByTransponderAndTimeWindow(Guid transponderId, DateTime startTimeUtc, DateTime endTimeUtc);
    }
}
