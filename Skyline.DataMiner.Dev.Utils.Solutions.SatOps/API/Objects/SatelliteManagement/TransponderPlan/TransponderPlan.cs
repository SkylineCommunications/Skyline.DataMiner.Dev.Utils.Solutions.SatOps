namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement;
    using DomModel = DOM.Model;

    /// <summary>
    /// Represents a transponder plan in the SatOps API.
    /// </summary>
    public class TransponderPlan : ApiNamedObject
    {
        private readonly DomModel.TransponderPlansInstance originalInstance;
        private readonly DomModel.TransponderPlansInstance updatedInstance;

        private TransponderPlan(DomModel.TransponderPlansInstance original, DomModel.TransponderPlansInstance updated)
            : base(original.ID.Id)
        {
            originalInstance = original;
            updatedInstance = updated;
        }

        /// <summary>
        /// Creates a <see cref="TransponderPlan"/> from an existing <see cref="DomModel.TransponderPlansInstance"/>.
        /// </summary>
        internal static TransponderPlan FromInstance(DomModel.TransponderPlansInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new TransponderPlan(instance, instance.Clone());
        }

        /// <summary>
        /// Creates a new <see cref="TransponderPlan"/> backed by a new <see cref="DomModel.TransponderPlansInstance"/>.
        /// </summary>
        internal static TransponderPlan CreateNewTransponderPlan()
        {
            var instance = new DomModel.TransponderPlansInstance();
            return new TransponderPlan(instance, instance.Clone());
        }

        /// <summary>
        /// Gets the current lifecycle status of the transponder plan.
        /// </summary>
        public InstanceStatus Status
        {
            get
            {
                switch (originalInstance.Status)
                {
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.TransponderPlansBehavior.StatusesEnum.Active:
                        return InstanceStatus.Active;
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.TransponderPlansBehavior.StatusesEnum.Deprecated:
                        return InstanceStatus.Deprecated;
                    case DomModel.SlcSatellite_ManagementIds.Behaviors.TransponderPlansBehavior.StatusesEnum.Error:
                        return InstanceStatus.Error;
                    default:
                        return InstanceStatus.Draft;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the transponder plan.
        /// </summary>
        public override string Name
        {
            get => updatedInstance.TransponderPlan?.PlanName;
            set => updatedInstance.TransponderPlan.PlanName = value;
        }

        /// <summary>
        /// Gets or sets the plan start time.
        /// </summary>
        public DateTime? StartTime
        {
            get => updatedInstance.TransponderPlan?.StartTime;
            set => updatedInstance.TransponderPlan.StartTime = value;
        }

        /// <summary>
        /// Gets or sets the plan end time.
        /// </summary>
        public DateTime? EndTime
        {
            get => updatedInstance.TransponderPlan?.EndTime;
            set => updatedInstance.TransponderPlan.EndTime = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plan is permanent.
        /// </summary>
        public bool? IsPermanent
        {
            get => updatedInstance.TransponderPlan?.IsPermanent;
            set => updatedInstance.TransponderPlan.IsPermanent = value;
        }

        /// <summary>
        /// Gets or sets the default slot size.
        /// </summary>
        public double? DefaultSlotSize
        {
            get => updatedInstance.TransponderPlan?.DefaultSlotSize;
            set => updatedInstance.TransponderPlan.DefaultSlotSize = value;
        }

        /// <summary>
        /// Gets or sets the related transponder identifier.
        /// </summary>
        public Guid? Transponder
        {
            get => updatedInstance.TransponderPlan?.Transponder;
            set => updatedInstance.TransponderPlan.Transponder = value;
        }

        /// <summary>
        /// Returns the updated instance used by the repository to persist changes.
        /// </summary>
        internal DomModel.TransponderPlansInstance ToUpdatedInstance() => updatedInstance;

        /// <summary>
        /// Returns the original instance used by the repository for reference comparison.
        /// </summary>
        internal DomModel.TransponderPlansInstance ToOriginalInstance() => originalInstance;

        internal static IEnumerable<TransponderPlan> InstantiateTransponderPlans(IEnumerable<DomModel.TransponderPlansInstance> instances)
        {
            if (instances == null)
                throw new ArgumentNullException(nameof(instances));
            if (!instances.Any())
                return Enumerable.Empty<TransponderPlan>();

            return InstantiateTransponderPlansIterator(instances);
        }

        private static IEnumerable<TransponderPlan> InstantiateTransponderPlansIterator(IEnumerable<DomModel.TransponderPlansInstance> instances)
        {
            foreach (var instance in instances)
            {
                yield return new TransponderPlan(instance, instance.Clone());
            }
        }
    }
}
