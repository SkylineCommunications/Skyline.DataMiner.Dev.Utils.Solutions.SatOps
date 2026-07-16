namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation
{
    using System;
    using Skyline.DataMiner.SDM.SatOps.Common.API;

    /// <summary>
    /// Represents a transponder range reservation in the SatOps API.
    /// Backed by a MediaOps.Plan <see cref="Job"/> containing a single
    /// <see cref="JobResourceNode"/> that targets a transponder resource
    /// and a <see cref="RangeCapacitySetting"/> that describes the reserved
    /// frequency range.
    /// </summary>
    public class TransponderRangeReservation : ApiNamedObject
    {
        private string name;
        private Guid? transponder;
        private double? relativeStartFrequency;
        private double? relativeEndFrequency;
        private DateTime? startTimeUtc;
        private DateTime? endTimeUtc;

        private TransponderRangeReservation()
        {
        }

        private TransponderRangeReservation(Guid id)
            : base(id)
        {
        }

        /// <summary>
        /// Gets or sets the reservation name (mapped to <see cref="Job.Name"/>).
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
