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

        /// <summary>
        /// Reserves a new frequency range on the reservation's job node.
        /// Equivalent of the classic <c>RangeReservationHelper.ReserveRange(jobId, nodeId, start, end)</c>:
        /// preserves the existing satellite value on the node and updates the bandwidth range.
        /// Throws when the underlying job is in <c>Running</c> or <c>Completed</c> state.
        /// </summary>
        /// <param name="reservationId">The reservation (job) identifier.</param>
        /// <param name="startFrequency">The relative start frequency to reserve.</param>
        /// <param name="endFrequency">The relative end frequency to reserve.</param>
        void ReserveRange(Guid reservationId, double startFrequency, double endFrequency);

        /// <summary>
        /// Reserves a new frequency range and (re)assigns the satellite capability.
        /// Equivalent of the classic <c>RangeReservationHelper.ReserveRange(jobId, nodeId, start, end, satelliteName)</c>,
        /// used immediately after a resource swap when the node has no pre-existing settings.
        /// Throws when the underlying job is in <c>Running</c> or <c>Completed</c> state.
        /// </summary>
        /// <param name="reservationId">The reservation (job) identifier.</param>
        /// <param name="startFrequency">The relative start frequency to reserve.</param>
        /// <param name="endFrequency">The relative end frequency to reserve.</param>
        /// <param name="satelliteName">The satellite capability discrete value to set, or <c>null</c> to skip.</param>
        void ReserveRange(Guid reservationId, double startFrequency, double endFrequency, string satelliteName);

        /// <summary>
        /// Upserts a slot-name property in the <c>SlcProperties</c> DOM, linked to the reservation's job.
        /// Equivalent of the classic <c>RangeReservationHelper.AddSlotNameProperty</c>.
        /// </summary>
        /// <param name="reservationId">The reservation (job) identifier.</param>
        /// <param name="slotName">The slot name value to store.</param>
        void AddSlotNameProperty(Guid reservationId, string slotName);

        /// <summary>
        /// Retrieves the id of the transponder job node on the reservation. When the underlying job
        /// contains multiple nodes (e.g. additional non-transponder resource nodes), the id returned
        /// is the id of the single transponder <c>JobResourceNode</c>.
        /// Equivalent of the classic <c>RangeReservationHelper.GetFirstNodeId</c>.
        /// </summary>
        /// <param name="reservationId">The reservation (job) identifier.</param>
        /// <returns>The transponder node id of the underlying job.</returns>
        string GetFirstNodeId(Guid reservationId);
    }
}
