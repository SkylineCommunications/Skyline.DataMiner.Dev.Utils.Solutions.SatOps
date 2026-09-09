namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TransponderSlotValidationMiddleware : IBulkRepositoryMiddleware<TransponderSlot>
    {
        private readonly ITransponderPlanRepository transponderPlanRepository;

        public TransponderSlotValidationMiddleware(ITransponderPlanRepository transponderPlanRepository)
        {
            this.transponderPlanRepository = transponderPlanRepository ?? throw new ArgumentNullException(nameof(transponderPlanRepository));
        }

        public TransponderSlot OnCreate(TransponderSlot oToCreate, Func<TransponderSlot, TransponderSlot> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponderSlot(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<TransponderSlot> OnCreate(IEnumerable<TransponderSlot> oToCreate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponderSlots(oToCreate);

            return next(oToCreate);
        }

        public TransponderSlot OnUpdate(TransponderSlot oToUpdate, Func<TransponderSlot, TransponderSlot> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponderSlot(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> OnUpdate(IEnumerable<TransponderSlot> oToUpdate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponderSlots(oToUpdate);

            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> OnCreateOrUpdate(IEnumerable<TransponderSlot> oToCreateOrUpdate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponderSlots(oToCreateOrUpdate);

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
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponderSlot in oToDelete)
            {
                ValidateTransponderSlot(transponderSlot);
            }

            next(oToDelete);
        }

        public void OnDelete(TransponderSlot oToDelete, Action<TransponderSlot> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

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

        private void ValidateTransponderSlot(TransponderSlot transponderSlot)
        {
            ValidateTransponderSlot(transponderSlot, null);
        }

        /// <summary>
        /// Validates a single transponder slot.
        /// </summary>
        /// <param name="transponderSlot">The slot to validate.</param>
        /// <param name="knownTransponderPlanIds">
        /// The identifiers of the transponder plans that were already resolved for the whole batch, or
        /// <see langword="null"/> when the slot is validated on its own and the plan still has to be looked up.
        /// </param>
        private void ValidateTransponderSlot(TransponderSlot transponderSlot, ISet<Guid> knownTransponderPlanIds)
        {
            if (transponderSlot == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponderSlot));

            if (!transponderSlot.TransponderPlan.HasValue || transponderSlot.TransponderPlan.Value == Guid.Empty)
                throw new ArgumentException("TransponderPlan is required.", nameof(transponderSlot));

            if (string.IsNullOrWhiteSpace(transponderSlot.Name))
                throw new ArgumentException("Slot Name is required.", nameof(transponderSlot));

            if (!transponderSlot.SlotStartFrequency.HasValue)
                throw new ArgumentException("Slot Start Frequency is required.", nameof(transponderSlot));

            if (!transponderSlot.SlotEndFrequency.HasValue)
                throw new ArgumentException("Slot End Frequency is required.", nameof(transponderSlot));

            if (!transponderSlot.Bandwidth.HasValue)
                throw new ArgumentException("Bandwidth is required.", nameof(transponderSlot));

            if (!transponderSlot.UplinkFreq.HasValue)
                throw new ArgumentException("Uplink Frequency is required.", nameof(transponderSlot));

            if (!transponderSlot.DownlinkFreq.HasValue)
                throw new ArgumentException("Downlink Frequency is required.", nameof(transponderSlot));

            var transponderPlanId = transponderSlot.TransponderPlan.Value;
            var transponderPlanExists = knownTransponderPlanIds != null
                ? knownTransponderPlanIds.Contains(transponderPlanId)
                : transponderPlanRepository.Read(transponderPlanId) != null;

            if (!transponderPlanExists)
                throw new ArgumentException($"Transponder plan with id '{transponderPlanId}' does not exist.", nameof(transponderSlot));
        }

        /// <summary>
        /// Validates every slot of a batch, resolving the referenced transponder plans with a single repository read
        /// instead of one read per slot.
        /// </summary>
        /// <remarks>
        /// The plans are resolved up front, but the per-slot checks keep their original order so that a batch
        /// containing an invalid slot still reports the same error as before.
        /// </remarks>
        private void ValidateTransponderSlots(IEnumerable<TransponderSlot> transponderSlots)
        {
            var slotsToValidate = transponderSlots as IReadOnlyCollection<TransponderSlot> ?? transponderSlots.ToList();
            var knownTransponderPlanIds = ReferenceValidationHelper.ReadExistingIds(slotsToValidate, slot => slot.TransponderPlan, transponderPlanRepository);

            foreach (var transponderSlot in slotsToValidate)
            {
                ValidateTransponderSlot(transponderSlot, knownTransponderPlanIds);
            }
        }
    }
}
