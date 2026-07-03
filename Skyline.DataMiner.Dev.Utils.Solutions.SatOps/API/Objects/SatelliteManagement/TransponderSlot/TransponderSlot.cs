namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using DomModel = DOM.Model;

    /// <summary>
    /// Represents a transponder slot in the SatOps API.
    /// </summary>
    public class TransponderSlot : ApiNamedObject
    {
        private readonly DomModel.TransponderSlotsInstance originalInstance;
        private readonly DomModel.TransponderSlotsInstance updatedInstance;

        private TransponderSlot(DomModel.TransponderSlotsInstance original, DomModel.TransponderSlotsInstance updated)
            : base(original.ID.Id)
        {
            originalInstance = original;
            updatedInstance = updated;
        }

        /// <summary>
        /// Creates a <see cref="TransponderSlot"/> from an existing <see cref="DomModel.TransponderSlotsInstance"/>.
        /// </summary>
        internal static TransponderSlot FromInstance(DomModel.TransponderSlotsInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new TransponderSlot(instance, instance.Clone());
        }

        /// <summary>
        /// Creates a new <see cref="TransponderSlot"/> backed by a new <see cref="DomModel.TransponderSlotsInstance"/>.
        /// </summary>
        internal static TransponderSlot CreateNewTransponderSlot()
        {
            var instance = new DomModel.TransponderSlotsInstance();
            return new TransponderSlot(instance, instance.Clone());
        }

        /// <summary>
        /// Gets or sets the parent transponder plan identifier.
        /// </summary>
        public Guid? TransponderPlan
        {
            get => originalInstance.TransponderSlot?.TransponderPlan;
            set => updatedInstance.TransponderSlot.TransponderPlan = value;
        }

        /// <summary>
        /// Gets or sets the slot name.
        /// </summary>
        public override string Name
        {
            get => originalInstance.TransponderSlot?.SlotName;
            set => updatedInstance.TransponderSlot.SlotName = value;
        }

        /// <summary>
        /// Gets or sets the slot start frequency.
        /// </summary>
        public double? SlotStartFrequency
        {
            get => originalInstance.TransponderSlot?.SlotStartFrequency;
            set => updatedInstance.TransponderSlot.SlotStartFrequency = value;
        }

        /// <summary>
        /// Gets or sets the slot end frequency.
        /// </summary>
        public double? SlotEndFrequency
        {
            get => originalInstance.TransponderSlot?.SlotEndFrequency;
            set => updatedInstance.TransponderSlot.SlotEndFrequency = value;
        }

        /// <summary>
        /// Gets or sets the bandwidth.
        /// </summary>
        public double? Bandwidth
        {
            get => originalInstance.TransponderSlot?.Bandwidth;
            set => updatedInstance.TransponderSlot.Bandwidth = value;
        }

        /// <summary>
        /// Gets or sets the uplink frequency.
        /// </summary>
        public double? UplinkFreq
        {
            get => originalInstance.TransponderSlot?.UplinkFreq;
            set => updatedInstance.TransponderSlot.UplinkFreq = value;
        }

        /// <summary>
        /// Gets or sets the downlink frequency.
        /// </summary>
        public double? DownlinkFreq
        {
            get => originalInstance.TransponderSlot?.DownlinkFreq;
            set => updatedInstance.TransponderSlot.DownlinkFreq = value;
        }

        /// <summary>
        /// Returns the updated instance used by the repository to persist changes.
        /// </summary>
        internal DomModel.TransponderSlotsInstance ToUpdatedInstance() => updatedInstance;

        /// <summary>
        /// Returns the original instance used by the repository for reference comparison.
        /// </summary>
        internal DomModel.TransponderSlotsInstance ToOriginalInstance() => originalInstance;

        internal static IEnumerable<TransponderSlot> InstantiateTransponderSlots(IEnumerable<DomModel.TransponderSlotsInstance> instances)
        {
            if (instances == null)
                throw new ArgumentNullException(nameof(instances));
            if (!instances.Any())
                return Enumerable.Empty<TransponderSlot>();

            return InstantiateTransponderSlotsIterator(instances);
        }

        private static IEnumerable<TransponderSlot> InstantiateTransponderSlotsIterator(IEnumerable<DomModel.TransponderSlotsInstance> instances)
        {
            foreach (var instance in instances)
            {
                yield return new TransponderSlot(instance, instance.Clone());
            }
        }
    }
}
