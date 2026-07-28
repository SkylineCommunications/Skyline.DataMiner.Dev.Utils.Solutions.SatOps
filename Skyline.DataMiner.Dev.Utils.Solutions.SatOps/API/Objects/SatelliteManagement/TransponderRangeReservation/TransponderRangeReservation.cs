namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using DomModel = DOM.Model;

    /// <summary>
    /// Represents a transponder range reservation in the SatOps API.
    /// </summary>
    public class TransponderRangeReservation : ApiNamedObject
    {
        private readonly DomModel.TransponderReservationsInstance originalInstance;
        private readonly DomModel.TransponderReservationsInstance updatedInstance;

        private TransponderRangeReservation(DomModel.TransponderReservationsInstance original, DomModel.TransponderReservationsInstance updated)
            : base(original.ID.Id)
        {
            originalInstance = original;
            updatedInstance = updated;
        }

        /// <summary>
        /// Creates a <see cref="TransponderRangeReservation"/> from an existing <see cref="DomModel.TransponderReservationsInstance"/>.
        /// </summary>
        internal static TransponderRangeReservation FromInstance(DomModel.TransponderReservationsInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return new TransponderRangeReservation(instance, instance.Clone());
        }

        /// <summary>
        /// Creates a new <see cref="TransponderRangeReservation"/> backed by a new <see cref="DomModel.TransponderReservationsInstance"/>.
        /// </summary>
        internal static TransponderRangeReservation CreateNew()
        {
            return CreateNewTransponderRangeReservation();
        }

        internal static TransponderRangeReservation CreateNewTransponderRangeReservation()
        {
            var instance = new DomModel.TransponderReservationsInstance();
            return new TransponderRangeReservation(instance, instance.Clone());
        }

        /// <summary>
        /// Gets or sets the reservation name.
        /// </summary>
        public override string Name
        {
            get => updatedInstance.TransponderReservation?.ReservationName;
            set => updatedInstance.TransponderReservation.ReservationName = value;
        }

        /// <summary>
        /// Gets or sets the referenced transponder identifier.
        /// </summary>
        public Guid? Transponder
        {
            get => updatedInstance.TransponderReservation?.Transponder;
            set => updatedInstance.TransponderReservation.Transponder = value;
        }

        /// <summary>
        /// Gets or sets the relative start frequency.
        /// </summary>
        public double? RelativeStartFrequency
        {
            get => updatedInstance.TransponderReservation?.RelativeStartFrequency;
            set => updatedInstance.TransponderReservation.RelativeStartFrequency = value;
        }

        /// <summary>
        /// Gets or sets the relative end frequency.
        /// </summary>
        public double? RelativeEndFrequency
        {
            get => updatedInstance.TransponderReservation?.RelativeEndFrequency;
            set => updatedInstance.TransponderReservation.RelativeEndFrequency = value;
        }

        /// <summary>
        /// Gets or sets the reservation start time.
        /// </summary>
        public DateTime? StartTime
        {
            get => updatedInstance.TransponderReservation?.StartTime;
            set => updatedInstance.TransponderReservation.StartTime = value == null ? (DateTime?)null : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets or sets the reservation end time.
        /// </summary>
        public DateTime? EndTime
        {
            get => updatedInstance.TransponderReservation?.EndTime;
            set => updatedInstance.TransponderReservation.EndTime = value == null ? (DateTime?)null : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the updated instance used by the repository to persist changes.
        /// </summary>
        internal DomModel.TransponderReservationsInstance ToUpdatedInstance() => updatedInstance;

        /// <summary>
        /// Returns the original instance used by the repository for reference comparison.
        /// </summary>
        internal DomModel.TransponderReservationsInstance ToOriginalInstance() => originalInstance;

        internal static IEnumerable<TransponderRangeReservation> InstantiateTransponderRangeReservations(IEnumerable<DomModel.TransponderReservationsInstance> instances)
        {
            if (instances == null)
            {
                throw new ArgumentNullException(nameof(instances));
            }

            if (!instances.Any())
            {
                return Enumerable.Empty<TransponderRangeReservation>();
            }

            return InstantiateTransponderRangeReservationsIterator(instances);
        }

        private static IEnumerable<TransponderRangeReservation> InstantiateTransponderRangeReservationsIterator(IEnumerable<DomModel.TransponderReservationsInstance> instances)
        {
            foreach (var instance in instances)
            {
                yield return new TransponderRangeReservation(instance, instance.Clone());
            }
        }
    }
}
