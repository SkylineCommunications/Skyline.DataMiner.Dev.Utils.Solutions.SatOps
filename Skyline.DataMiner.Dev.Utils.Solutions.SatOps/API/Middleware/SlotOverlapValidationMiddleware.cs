namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Middleware that validates there are no overlapping frequency ranges or duplicate slot names
    /// within the same transponder plan when creating or updating transponder slots.
    /// </summary>
    /// <remarks>
    /// Single-slot and bulk operations share the same validation routine: the submitted slots are combined
    /// with the already persisted slots of the same transponder plan (looked up through the slot repository,
    /// excluding the slots that are part of the submission itself) and the resulting set is validated as a whole.
    /// Conflicts that only exist between already persisted slots are ignored, so pre-existing data cannot
    /// block an unrelated create or update.
    /// </remarks>
    internal sealed class SlotOverlapValidationMiddleware : IBulkRepositoryMiddleware<TransponderSlot>
    {
        private const double OverlapTolerance = 1e-9;

        private readonly ITransponderSlotRepository slotRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlotOverlapValidationMiddleware"/> class.
        /// </summary>
        /// <param name="slotRepository">
        /// The repository used to look up the slots that are already persisted for a transponder plan.
        /// This must be the repository instance without this middleware applied, to avoid re-entering validation.
        /// </param>
        public SlotOverlapValidationMiddleware(ITransponderSlotRepository slotRepository)
        {
            this.slotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
        }

        public TransponderSlot OnCreate(TransponderSlot oToCreate, Func<TransponderSlot, TransponderSlot> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSlots(new[] { oToCreate }, nameof(oToCreate));
            return next(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> OnCreate(IEnumerable<TransponderSlot> oToCreate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSlots(oToCreate, nameof(oToCreate));
            return next(oToCreate);
        }

        public TransponderSlot OnUpdate(TransponderSlot oToUpdate, Func<TransponderSlot, TransponderSlot> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSlots(new[] { oToUpdate }, nameof(oToUpdate));
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> OnUpdate(IEnumerable<TransponderSlot> oToUpdate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSlots(oToUpdate, nameof(oToUpdate));
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> OnCreateOrUpdate(IEnumerable<TransponderSlot> oToCreateOrUpdate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSlots(oToCreateOrUpdate, nameof(oToCreateOrUpdate));
            return next(oToCreateOrUpdate);
        }

        public long OnCount(FilterElement<TransponderSlot> filter, Func<FilterElement<TransponderSlot>, long> next)
        {
            return next(filter);
        }

        public long OnCount(IQuery<TransponderSlot> query, Func<IQuery<TransponderSlot>, long> next)
        {
            return next(query);
        }

        public void OnDelete(IEnumerable<TransponderSlot> oToDelete, Action<IEnumerable<TransponderSlot>> next)
        {
            next(oToDelete);
        }

        public void OnDelete(TransponderSlot oToDelete, Action<TransponderSlot> next)
        {
            next(oToDelete);
        }

        public IEnumerable<TransponderSlot> OnRead(FilterElement<TransponderSlot> filter, Func<FilterElement<TransponderSlot>, IEnumerable<TransponderSlot>> next)
        {
            return next(filter);
        }

        public IEnumerable<TransponderSlot> OnRead(IQuery<TransponderSlot> query, Func<IQuery<TransponderSlot>, IEnumerable<TransponderSlot>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> OnReadPaged(FilterElement<TransponderSlot> filter, Func<FilterElement<TransponderSlot>, IEnumerable<IPagedResult<TransponderSlot>>> next)
        {
            return next(filter);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> OnReadPaged(IQuery<TransponderSlot> query, Func<IQuery<TransponderSlot>, IEnumerable<IPagedResult<TransponderSlot>>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> OnReadPaged(FilterElement<TransponderSlot> filter, int pageSize, Func<FilterElement<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>> next)
        {
            return next(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderSlot>> OnReadPaged(IQuery<TransponderSlot> query, int pageSize, Func<IQuery<TransponderSlot>, int, IEnumerable<IPagedResult<TransponderSlot>>> next)
        {
            return next(query, pageSize);
        }

        /// <summary>
        /// Validates the submitted slots against each other and against the slots that are already persisted
        /// for the same transponder plan. This is the single validation entry point used by both the
        /// single-slot and the bulk operations.
        /// </summary>
        /// <param name="slots">The submitted slots.</param>
        /// <param name="parameterName">The name of the public parameter reported on validation failure.</param>
        private void ValidateSlots(IEnumerable<TransponderSlot> slots, string parameterName)
        {
            var submitted = slots.Where(HasValidatableRange).ToList();
            if (submitted.Count == 0)
                return;

            foreach (var group in submitted.GroupBy(s => s.TransponderPlan.Value))
            {
                ValidatePlan(group.Key, group.ToList(), parameterName);
            }
        }

        /// <summary>
        /// Validates all slots of a single transponder plan: the submitted ones combined with the persisted ones
        /// that are not part of the submission.
        /// </summary>
        private void ValidatePlan(Guid planId, IReadOnlyCollection<TransponderSlot> submitted, string parameterName)
        {
            var submittedIds = new HashSet<Guid>(submitted.Select(s => s.Id));

            var persisted = (slotRepository.ReadByTransponderPlan(planId) ?? Enumerable.Empty<TransponderSlot>())
                .Where(s => HasValidatableRange(s) && !submittedIds.Contains(s.Id))
                .Select(s => new SlotCandidate(s, isSubmitted: false));

            var sorted = submitted
                .Select(s => new SlotCandidate(s, isSubmitted: true))
                .Concat(persisted)
                .OrderBy(c => c.Start)
                .ToList();

            var namesSeen = new Dictionary<string, SlotCandidate>(StringComparer.OrdinalIgnoreCase);
            var overlapping = new List<SlotCandidate>();

            foreach (var current in sorted)
            {
                ValidateName(current, namesSeen, parameterName);

                overlapping.RemoveAll(c => !SlotsOverlap(c.Start, c.End, current.Start, current.End));
                ValidateOverlap(current, overlapping, parameterName);

                overlapping.Add(current);
            }
        }

        /// <summary>
        /// Verifies that the name of the given slot is not already used by another slot of the same plan.
        /// Collisions between two persisted slots are ignored.
        /// </summary>
        private static void ValidateName(SlotCandidate current, IDictionary<string, SlotCandidate> namesSeen, string parameterName)
        {
            if (string.IsNullOrEmpty(current.Slot.Name))
                return;

            if (!namesSeen.TryGetValue(current.Slot.Name, out var previous))
            {
                namesSeen.Add(current.Slot.Name, current);
                return;
            }

            if (current.IsSubmitted || previous.IsSubmitted)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.DuplicateSlotNameDetected, current.Slot.Name),
                    parameterName);
            }
        }

        /// <summary>
        /// Verifies that the given slot does not overlap any of the still open slots that start before it.
        /// Overlaps between two persisted slots are ignored.
        /// </summary>
        private static void ValidateOverlap(SlotCandidate current, IEnumerable<SlotCandidate> overlapping, string parameterName)
        {
            foreach (var other in overlapping)
            {
                if (!current.IsSubmitted && !other.IsSubmitted)
                    continue;

                var reported = current.IsSubmitted && !other.IsSubmitted ? current : other;
                var conflicting = ReferenceEquals(reported, current) ? other : current;

                throw new ArgumentException(
                    string.Format(ExceptionMessages.SlotOverlapDetected, reported.Slot.Name, conflicting.Slot.Name),
                    parameterName);
            }
        }

        private static bool HasValidatableRange(TransponderSlot slot)
        {
            return slot != null
                && slot.TransponderPlan.HasValue
                && slot.TransponderPlan.Value != Guid.Empty
                && slot.SlotStartFrequency.HasValue
                && slot.SlotEndFrequency.HasValue;
        }

        private static bool SlotsOverlap(double start1, double end1, double start2, double end2)
        {
            return start1 < end2 - OverlapTolerance && end1 > start2 + OverlapTolerance;
        }

        /// <summary>
        /// Pairs a slot with its origin so that conflicts between two already persisted slots can be ignored.
        /// </summary>
        private sealed class SlotCandidate
        {
            public SlotCandidate(TransponderSlot slot, bool isSubmitted)
            {
                Slot = slot;
                IsSubmitted = isSubmitted;
            }

            public TransponderSlot Slot { get; }

            public bool IsSubmitted { get; }

            public double Start => Slot.SlotStartFrequency.Value;

            public double End => Slot.SlotEndFrequency.Value;
        }
    }
}
