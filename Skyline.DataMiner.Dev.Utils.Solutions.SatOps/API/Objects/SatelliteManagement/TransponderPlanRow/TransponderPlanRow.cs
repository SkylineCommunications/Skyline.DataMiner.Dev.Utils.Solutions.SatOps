namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using DomModel = DOM.Model;

    /// <summary>
    /// Represents a transponder plan row in the SatOps API.
    /// </summary>
    public class TransponderPlanRow : ApiObject
    {
        private readonly DomModel.TransponderPlanRowsInstance originalInstance;
        private readonly DomModel.TransponderPlanRowsInstance updatedInstance;

        private TransponderPlanRow(DomModel.TransponderPlanRowsInstance original, DomModel.TransponderPlanRowsInstance updated)
            : base(original.ID.Id)
        {
            originalInstance = original;
            updatedInstance = updated;
        }

        /// <summary>
        /// Creates a <see cref="TransponderPlanRow"/> from an existing <see cref="DomModel.TransponderPlanRowsInstance"/>.
        /// </summary>
        internal static TransponderPlanRow FromInstance(DomModel.TransponderPlanRowsInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new TransponderPlanRow(instance, instance.Clone());
        }

        /// <summary>
        /// Creates a new <see cref="TransponderPlanRow"/> backed by a new <see cref="DomModel.TransponderPlanRowsInstance"/>.
        /// </summary>
        internal static TransponderPlanRow CreateNewTransponderPlanRow()
        {
            var instance = new DomModel.TransponderPlanRowsInstance();
            return new TransponderPlanRow(instance, instance.Clone());
        }

        /// <summary>
        /// Gets or sets the parent transponder plan identifier.
        /// </summary>
        public Guid? TransponderPlan
        {
            get => originalInstance.TransponderPlanRow?.TransponderPlan;
            set => updatedInstance.TransponderPlanRow.TransponderPlan = value;
        }

        /// <summary>
        /// Gets or sets the bandwidth.
        /// </summary>
        public double? Bandwidth
        {
            get => originalInstance.TransponderPlanRow?.Bandwidth;
            set => updatedInstance.TransponderPlanRow.Bandwidth = value;
        }

        /// <summary>
        /// Gets or sets the step size.
        /// </summary>
        public double? StepSize
        {
            get => originalInstance.TransponderPlanRow?.StepSize;
            set => updatedInstance.TransponderPlanRow.StepSize = value;
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public double? Offset
        {
            get => originalInstance.TransponderPlanRow?.Offset;
            set => updatedInstance.TransponderPlanRow.Offset = value;
        }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public double? Limit
        {
            get => originalInstance.TransponderPlanRow?.Limit;
            set => updatedInstance.TransponderPlanRow.Limit = value;
        }

        /// <summary>
        /// Returns the updated instance used by the repository to persist changes.
        /// </summary>
        internal DomModel.TransponderPlanRowsInstance ToUpdatedInstance() => updatedInstance;

        /// <summary>
        /// Returns the original instance used by the repository for reference comparison.
        /// </summary>
        internal DomModel.TransponderPlanRowsInstance ToOriginalInstance() => originalInstance;

        internal static IEnumerable<TransponderPlanRow> InstantiateTransponderPlanRows(IEnumerable<DomModel.TransponderPlanRowsInstance> instances)
        {
            if (instances == null)
                throw new ArgumentNullException(nameof(instances));
            if (!instances.Any())
                return Enumerable.Empty<TransponderPlanRow>();

            return InstantiateTransponderPlanRowsIterator(instances);
        }

        private static IEnumerable<TransponderPlanRow> InstantiateTransponderPlanRowsIterator(IEnumerable<DomModel.TransponderPlanRowsInstance> instances)
        {
            foreach (var instance in instances)
            {
                yield return new TransponderPlanRow(instance, instance.Clone());
            }
        }
    }
}
