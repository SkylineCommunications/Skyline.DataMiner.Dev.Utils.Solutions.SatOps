namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
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
    /// For single-slot operations (<see cref="OnCreate(TransponderSlot, Func{TransponderSlot, TransponderSlot})"/>
    /// and <see cref="OnUpdate(TransponderSlot, Func{TransponderSlot, TransponderSlot})"/>), the check is performed
    /// against all existing slots for the same plan using the provided resolver.
    /// For bulk operations, the check is performed within the submitted batch.
    /// </remarks>
    internal sealed class SlotOverlapValidationMiddleware : IBulkRepositoryMiddleware<TransponderSlot>
    {
        private const double OverlapTolerance = 1e-9;

        private readonly Func<Guid, IEnumerable<TransponderSlot>> existingSlotsResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlotOverlapValidationMiddleware"/> class.
        /// </summary>
        /// <param name="existingSlotsResolver">
        /// A delegate that returns all existing slots for a given transponder plan identifier.
        /// Used to check single-slot create/update operations against persisted slots.
        /// </param>
        public SlotOverlapValidationMiddleware(Func<Guid, IEnumerable<TransponderSlot>> existingSlotsResolver)
        {
            this.existingSlotsResolver = existingSlotsResolver ?? throw new ArgumentNullException(nameof(existingSlotsResolver));
        }

        public TransponderSlot OnCreate(TransponderSlot oToCreate, Func<TransponderSlot, TransponderSlot> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateAgainstExisting(oToCreate, excludeId: Guid.Empty);
            return next(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> OnCreate(IEnumerable<TransponderSlot> oToCreate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBatch(oToCreate);
            return next(oToCreate);
        }

        public TransponderSlot OnUpdate(TransponderSlot oToUpdate, Func<TransponderSlot, TransponderSlot> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateAgainstExisting(oToUpdate, excludeId: oToUpdate.Id);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> OnUpdate(IEnumerable<TransponderSlot> oToUpdate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBatch(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> OnCreateOrUpdate(IEnumerable<TransponderSlot> oToCreateOrUpdate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBatch(oToCreateOrUpdate);
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
        /// Validates a single slot against existing slots for the same plan.
        /// </summary>
        /// <param name="slot">The slot to validate.</param>
        /// <param name="excludeId">The ID of the slot to exclude from comparison (use <see cref="Guid.Empty"/> for new slots, or the slot's own ID for updates).</param>
        private void ValidateAgainstExisting(TransponderSlot slot, Guid excludeId)
        {
            if (!slot.TransponderPlan.HasValue || slot.TransponderPlan.Value == Guid.Empty)
                return;

            if (!slot.SlotStartFrequency.HasValue || !slot.SlotEndFrequency.HasValue)
                return;

            var planId = slot.TransponderPlan.Value;
            var existing = (existingSlotsResolver(planId) ?? Enumerable.Empty<TransponderSlot>())
                .Where(s => s != null
                    && s.Id != excludeId
                    && s.SlotStartFrequency.HasValue
                    && s.SlotEndFrequency.HasValue);

            var overlappingSlot = existing.FirstOrDefault(existingSlot => SlotsOverlap(
                slot.SlotStartFrequency.Value, slot.SlotEndFrequency.Value,
                existingSlot.SlotStartFrequency.Value, existingSlot.SlotEndFrequency.Value));

            if (overlappingSlot != null)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.SlotOverlapDetected, slot.Name, overlappingSlot.Name),
                    nameof(slot));
            }
        }

        /// <summary>
        /// Validates a batch of slots for internal overlaps and duplicate names within each transponder plan group.
        /// </summary>
        private static void ValidateBatch(IEnumerable<TransponderSlot> slots)
        {
            var validSlots = slots
                .Where(s => s != null
                    && s.TransponderPlan.HasValue
                    && s.TransponderPlan.Value != Guid.Empty
                    && s.SlotStartFrequency.HasValue
                    && s.SlotEndFrequency.HasValue)
                .ToList();

            var byPlan = validSlots.GroupBy(s => s.TransponderPlan.Value);
            foreach (var group in byPlan)
            {
                var sorted = group.OrderBy(s => s.SlotStartFrequency.Value).ToList();
                var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < sorted.Count; i++)
                {
                    var current = sorted[i];

                    if (!string.IsNullOrEmpty(current.Name) && !seenNames.Add(current.Name))
                    {
                        throw new ArgumentException(
                            string.Format(ExceptionMessages.DuplicateSlotNameDetected, current.Name));
                    }

                    if (i + 1 >= sorted.Count)
                        continue;

                    var next = sorted[i + 1];
                    if (SlotsOverlap(
                        current.SlotStartFrequency.Value, current.SlotEndFrequency.Value,
                        next.SlotStartFrequency.Value, next.SlotEndFrequency.Value))
                    {
                        throw new ArgumentException(
                            string.Format(ExceptionMessages.SlotOverlapDetected, current.Name, next.Name));
                    }
                }
            }
        }

        private static bool SlotsOverlap(double start1, double end1, double start2, double end2)
        {
            return start1 < end2 - OverlapTolerance && end1 > start2 + OverlapTolerance;
        }
    }
}
