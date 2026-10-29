namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation
{
    using System;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;

    /// <summary>
    /// An enriched reservation and movement metadata captured from the same MediaOps job read.
    /// This is a point-in-time read, not a lock or a guarantee that a subsequent write is safe.
    /// </summary>
    public sealed class TransponderReservationAllocationContext
    {
        internal TransponderReservationAllocationContext(TransponderRangeReservation reservation, Job job, JobResourceNode node)
        {
            Reservation = reservation;
            JobState = job.State;
            JobStartTimeUtc = job.Start.UtcDateTime;
            JobEndTimeUtc = job.End.UtcDateTime;
            NodeId = node.Id;
            ResourceId = node.ResourceId;
            ResourcePoolId = node.ResourcePoolId;
        }

        /// <summary>Gets the reservation, including its stored slot name.</summary>
        public TransponderRangeReservation Reservation { get; }

        /// <summary>Gets the underlying job state at read time.</summary>
        public JobState JobState { get; }

        /// <summary>Gets the complete job start time in UTC.</summary>
        public DateTime JobStartTimeUtc { get; }

        /// <summary>Gets the complete job end time in UTC.</summary>
        public DateTime JobEndTimeUtc { get; }

        /// <summary>Gets the single transponder resource node identifier.</summary>
        public string NodeId { get; }

        /// <summary>Gets the transponder resource identifier.</summary>
        public Guid ResourceId { get; }

        /// <summary>Gets the transponder resource pool identifier.</summary>
        public Guid ResourcePoolId { get; }
    }
}
