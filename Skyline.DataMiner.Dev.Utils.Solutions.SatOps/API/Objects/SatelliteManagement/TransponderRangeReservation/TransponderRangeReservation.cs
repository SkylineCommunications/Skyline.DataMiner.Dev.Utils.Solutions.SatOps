namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation
{
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using System;

    /// <summary>
    /// Represents a transponder range reservation in the SatOps API.
    /// Backed by a MediaOps.Plan <see cref="Job"/> containing a single
    /// <see cref="JobResourceNode"/> that targets a transponder resource
    /// and a <see cref="RangeCapacitySetting"/> that describes the reserved
    /// frequency range.
    /// </summary>
    public class TransponderRangeReservation : Skyline.DataMiner.SDM.SatOps.Common.API.ApiNamedObject
    {
        private string name;
        private Guid? transponder;
        private double? relativeStartFrequency;
        private double? relativeEndFrequency;
        private DateTime? startTimeUtc;
        private DateTime? endTimeUtc;
        private DateTime? preRollStartUtc;
        private DateTime? postRollEndUtc;
        private string satelliteName;
        private string nodeId;

        private TransponderRangeReservation()
        {
        }

        private TransponderRangeReservation(Guid id)
            : base(id)
        {
        }

        /// <summary>
        /// Gets or sets the name of the transponder range reservation.
        /// </summary>
        public override string Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// Gets or sets the SatOps <c>Transponder</c> DOM id that this reservation targets.
        /// The repository resolves this to the transponder's <c>DOMResource</c> when
        /// assigning it to the underlying <see cref="JobResourceNode"/>.
        /// </summary>
        public Guid? Transponder
        {
            get => transponder;
            set => transponder = value;
        }

        /// <summary>
        /// Gets or sets the relative start frequency (min of the bandwidth range capacity).
        /// </summary>
        public double? RelativeStartFrequency
        {
            get => relativeStartFrequency;
            set => relativeStartFrequency = value;
        }

        /// <summary>
        /// Gets or sets the relative end frequency (max of the bandwidth range capacity).
        /// </summary>
        public double? RelativeEndFrequency
        {
            get => relativeEndFrequency;
            set => relativeEndFrequency = value;
        }

        /// <summary>
        /// Gets or sets the reservation start time (UTC).
        /// </summary>
        public DateTime? StartTime
        {
            get => startTimeUtc;
            set => startTimeUtc = value == null ? (DateTime?)null : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets or sets the reservation end time (UTC).
        /// </summary>
        public DateTime? EndTime
        {
            get => endTimeUtc;
            set => endTimeUtc = value == null ? (DateTime?)null : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets or sets the pre-roll start (UTC) of the underlying MediaOps.Plan job.
        /// When <c>null</c>, the repository uses <see cref="StartTime"/> as the pre-roll start
        /// (i.e. a zero-length pre-roll).
        /// </summary>
        public DateTime? PreRollStart
        {
            get => preRollStartUtc;
            set => preRollStartUtc = value == null ? (DateTime?)null : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets or sets the post-roll end (UTC) of the underlying MediaOps.Plan job.
        /// When <c>null</c>, the repository uses <see cref="EndTime"/> as the post-roll end
        /// (i.e. a zero-length post-roll).
        /// </summary>
        public DateTime? PostRollEnd
        {
            get => postRollEndUtc;
            set => postRollEndUtc = value == null ? (DateTime?)null : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets or sets the satellite capability value written to the reservation job node
        /// (equivalent of the helper's <c>SatelliteCapability</c> profile parameter).
        /// </summary>
        public string SatelliteName
        {
            get => satelliteName;
            set => satelliteName = value;
        }

        /// <summary>
        /// Gets the id of the underlying job resource node (equivalent of the helper's node id).
        /// Populated by the repository on read; ignored on create/update.
        /// </summary>
        public string NodeId
        {
            get => nodeId;
            internal set => nodeId = value;
        }

        /// <summary>
        /// Gets the derived bandwidth size (end - start) of the reservation, or <c>null</c>
        /// when either bound is missing.
        /// </summary>
        public double? BandwidthSize
        {
            get
            {
                if (!relativeStartFrequency.HasValue || !relativeEndFrequency.HasValue)
                {
                    return null;
                }

                return relativeEndFrequency.Value - relativeStartFrequency.Value;
            }
        }

        /// <summary>
        /// Creates a new, empty <see cref="TransponderRangeReservation"/>.
        /// </summary>
        internal static TransponderRangeReservation CreateNew()
        {
            return new TransponderRangeReservation();
        }

        /// <summary>
        /// Creates a new <see cref="TransponderRangeReservation"/> with the specified id.
        /// </summary>
        internal static TransponderRangeReservation CreateWithId(Guid id)
        {
            return new TransponderRangeReservation(id);
        }
    }
}
