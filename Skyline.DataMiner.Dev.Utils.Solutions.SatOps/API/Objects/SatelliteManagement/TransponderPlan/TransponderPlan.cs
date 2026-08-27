namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan
{
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DomModel = DOM.Model;

    /// <summary>
    /// Represents a transponder plan in the SatOps API.
    /// </summary>
    public class TransponderPlan : ApiNamedObject
    {
        private readonly DomModel.TransponderPlansInstance originalInstance;
        private readonly DomModel.TransponderPlansInstance updatedInstance;

        /// <summary>
        /// Initializes a new, in-memory <see cref="TransponderPlan"/>.
        /// The plan is not persisted until it is passed to the create method of the transponder plan repository.
        /// </summary>
        public TransponderPlan()
            : this(new DomModel.TransponderPlansInstance())
        {
        }

        private TransponderPlan(DomModel.TransponderPlansInstance instance)
            : this(instance, instance.Clone())
        {
        }

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
        /// <remarks>
        /// When the plan is permanent, this value is set to <see cref="DateTime.MinValue"/> while the plan is created or updated.
        /// </remarks>
        public DateTime? StartTime
        {
            get => updatedInstance.TransponderPlan?.StartTime;
            set => updatedInstance.TransponderPlan.StartTime = value;
        }

        /// <summary>
        /// Gets or sets the plan end time.
        /// </summary>
        /// <remarks>
        /// When the plan is permanent, this value is set to <see cref="DateTime.MaxValue"/> while the plan is created or updated.
        /// </remarks>
        public DateTime? EndTime
        {
            get => updatedInstance.TransponderPlan?.EndTime;
            set => updatedInstance.TransponderPlan.EndTime = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plan is permanent.
        /// </summary>
        /// <remarks>
        /// A permanent plan covers the entire timeline: creating or updating it through the API forces
        /// <see cref="StartTime"/> to <see cref="DateTime.MinValue"/> and <see cref="EndTime"/> to <see cref="DateTime.MaxValue"/>.
        /// </remarks>
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
