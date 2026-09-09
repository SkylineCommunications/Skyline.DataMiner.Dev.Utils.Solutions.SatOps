namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TransponderPlanValidationMiddleware : IBulkRepositoryMiddleware<TransponderPlan>
    {
        private readonly ITransponderRepository transponderRepository;
        private readonly ITransponderPlanRepository transponderPlanRepository;

        public TransponderPlanValidationMiddleware(ITransponderRepository transponderRepository, ITransponderPlanRepository transponderPlanRepository)
        {
            this.transponderRepository = transponderRepository ?? throw new ArgumentNullException(nameof(transponderRepository));
            this.transponderPlanRepository = transponderPlanRepository ?? throw new ArgumentNullException(nameof(transponderPlanRepository));
        }

        public TransponderPlan OnCreate(TransponderPlan oToCreate, Func<TransponderPlan, TransponderPlan> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            NormalizePermanentPlan(oToCreate);
            ValidateTransponderPlan(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<TransponderPlan> OnCreate(IEnumerable<TransponderPlan> oToCreate, Func<IEnumerable<TransponderPlan>, IReadOnlyCollection<TransponderPlan>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var plansToCreate = Materialize(oToCreate);
            ValidateTransponderPlans(plansToCreate);

            return next(plansToCreate);
        }

        public TransponderPlan OnUpdate(TransponderPlan oToUpdate, Func<TransponderPlan, TransponderPlan> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            NormalizePermanentPlan(oToUpdate);
            ValidateTransponderPlan(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderPlan> OnUpdate(IEnumerable<TransponderPlan> oToUpdate, Func<IEnumerable<TransponderPlan>, IReadOnlyCollection<TransponderPlan>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var plansToUpdate = Materialize(oToUpdate);
            ValidateTransponderPlans(plansToUpdate);

            return next(plansToUpdate);
        }

        public IReadOnlyCollection<TransponderPlan> OnCreateOrUpdate(IEnumerable<TransponderPlan> oToCreateOrUpdate, Func<IEnumerable<TransponderPlan>, IReadOnlyCollection<TransponderPlan>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var plansToCreateOrUpdate = Materialize(oToCreateOrUpdate);
            ValidateTransponderPlans(plansToCreateOrUpdate);

            return next(plansToCreateOrUpdate);
        }

        public long OnCount(FilterElement<TransponderPlan> filter, Func<FilterElement<TransponderPlan>, long> next)
        {
            return next(filter);
        }

        public long OnCount(IQuery<TransponderPlan> query, Func<IQuery<TransponderPlan>, long> next)
        {
            return next(query);
        }

        public void OnDelete(IEnumerable<TransponderPlan> oToDelete, Action<IEnumerable<TransponderPlan>> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponderPlan in oToDelete)
            {
                ValidateTransponderPlan(transponderPlan);
            }

            next(oToDelete);
        }

        public void OnDelete(TransponderPlan oToDelete, Action<TransponderPlan> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            next(oToDelete);
        }

        public IEnumerable<TransponderPlan> OnRead(FilterElement<TransponderPlan> filter, Func<FilterElement<TransponderPlan>, IEnumerable<TransponderPlan>> next)
        {
            return next(filter);
        }

        public IEnumerable<TransponderPlan> OnRead(IQuery<TransponderPlan> query, Func<IQuery<TransponderPlan>, IEnumerable<TransponderPlan>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> OnReadPaged(FilterElement<TransponderPlan> filter, Func<FilterElement<TransponderPlan>, IEnumerable<IPagedResult<TransponderPlan>>> next)
        {
            return next(filter);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> OnReadPaged(IQuery<TransponderPlan> query, Func<IQuery<TransponderPlan>, IEnumerable<IPagedResult<TransponderPlan>>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> OnReadPaged(FilterElement<TransponderPlan> filter, int pageSize, Func<FilterElement<TransponderPlan>, int, IEnumerable<IPagedResult<TransponderPlan>>> next)
        {
            return next(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderPlan>> OnReadPaged(IQuery<TransponderPlan> query, int pageSize, Func<IQuery<TransponderPlan>, int, IEnumerable<IPagedResult<TransponderPlan>>> next)
        {
            return next(query, pageSize);
        }

        private void ValidateTransponderPlan(TransponderPlan transponderPlan)
        {
            ValidateTransponderPlan(transponderPlan, null, null);
        }

        /// <summary>
        /// Validates a single transponder plan.
        /// </summary>
        /// <param name="transponderPlan">The plan to validate.</param>
        /// <param name="knownTransponderIds">
        /// The identifiers of the transponders that were already resolved for the whole batch, or
        /// <see langword="null"/> when the plan is validated on its own and the transponder still has to be looked up.
        /// </param>
        /// <param name="planCache">
        /// Cache of the persisted plans per transponder, shared by all plans of the same batch, or
        /// <see langword="null"/> when the plan is validated on its own.
        /// </param>
        private void ValidateTransponderPlan(TransponderPlan transponderPlan, ISet<Guid> knownTransponderIds, IDictionary<Guid, IReadOnlyList<TransponderPlan>> planCache)
        {
            if (transponderPlan == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponderPlan));

            if (string.IsNullOrWhiteSpace(transponderPlan.Name))
                throw new ArgumentException(ExceptionMessages.TransponderPlanNameIsRequired, nameof(transponderPlan));

            if (!transponderPlan.DefaultSlotSize.HasValue)
                throw new ArgumentException(ExceptionMessages.TransponderPlanDefaultSlotSizeIsRequired, nameof(transponderPlan));

            if (transponderPlan.DefaultSlotSize.Value <= 0)
                throw new ArgumentException(ExceptionMessages.TransponderPlanDefaultSlotSizeMustBeGreaterThanZero, nameof(transponderPlan));

            if (!transponderPlan.Transponder.HasValue || transponderPlan.Transponder.Value == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.TransponderPlanTransponderIsRequired, nameof(transponderPlan));

            var transponderId = transponderPlan.Transponder.Value;
            var transponderExists = knownTransponderIds != null
                ? knownTransponderIds.Contains(transponderId)
                : transponderRepository.Read(transponderId) != null;

            if (!transponderExists)
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderWithIdDoesNotExist, transponderId), nameof(transponderPlan));

            ValidatePlanConstraints(transponderPlan, transponderId, planCache);
        }

        /// <summary>
        /// Validates every plan of a batch, resolving the referenced transponders with a single repository read and
        /// reading the persisted plans once per distinct transponder instead of once per submitted plan.
        /// </summary>
        /// <remarks>
        /// The transponders are resolved up front, but the per-plan checks keep their original order so that a batch
        /// containing an invalid plan still reports the same error as before.
        /// </remarks>
        private void ValidateTransponderPlans(IReadOnlyCollection<TransponderPlan> plansToValidate)
        {
            foreach (var transponderPlan in plansToValidate)
            {
                NormalizePermanentPlan(transponderPlan);
            }

            var knownTransponderIds = ReferenceValidationHelper.ReadExistingIds(plansToValidate, plan => plan.Transponder, transponderRepository);
            var planCache = new Dictionary<Guid, IReadOnlyList<TransponderPlan>>();

            foreach (var transponderPlan in plansToValidate)
            {
                ValidateTransponderPlan(transponderPlan, knownTransponderIds, planCache);
            }
        }

        /// <summary>
        /// Materializes the submitted sequence so that the plans that are normalized and validated are the exact
        /// instances that are handed over to the next middleware.
        /// </summary>
        private static IReadOnlyCollection<TransponderPlan> Materialize(IEnumerable<TransponderPlan> transponderPlans)
        {
            return transponderPlans as IReadOnlyCollection<TransponderPlan> ?? transponderPlans.ToList();
        }

        /// <summary>
        /// Aligns the time window of a permanent plan with the convention used across the solution: a permanent plan
        /// covers the entire timeline, so its start and end time are set to <see cref="DateTime.MinValue"/> and
        /// <see cref="DateTime.MaxValue"/>.
        /// </summary>
        /// <param name="transponderPlan">The plan to normalize. Non-permanent and <see langword="null"/> plans are left untouched.</param>
        private static void NormalizePermanentPlan(TransponderPlan transponderPlan)
        {
            if (transponderPlan == null || !transponderPlan.IsPermanent.GetValueOrDefault())
                return;

            transponderPlan.StartTime = DateTime.MinValue;
            transponderPlan.EndTime = DateTime.MaxValue;
        }

        /// <summary>
        /// Returns the persisted plans of a transponder, reusing the batch cache when one is available.
        /// </summary>
        private IReadOnlyList<TransponderPlan> ReadPlansByTransponder(Guid transponderId, IDictionary<Guid, IReadOnlyList<TransponderPlan>> planCache)
        {
            return ReferenceValidationHelper.ReadCached(transponderId, planCache, transponderPlanRepository.ReadByTransponder);
        }

        private void ValidatePlanConstraints(TransponderPlan transponderPlan, Guid transponderId, IDictionary<Guid, IReadOnlyList<TransponderPlan>> planCache)
        {
            var existingPlans = ReadPlansByTransponder(transponderId, planCache)
                .Where(plan => plan != null && plan.Status != InstanceStatus.Deprecated && plan.Id != transponderPlan.Id)
                .ToList();

            if (transponderPlan.IsPermanent.GetValueOrDefault())
            {
                if (existingPlans.Any(plan => plan.IsPermanent.GetValueOrDefault()))
                {
                    throw new ArgumentException(string.Format(ExceptionMessages.PermanentTransponderPlanAlreadyExists, transponderId), nameof(transponderPlan));
                }

                return;
            }

            if (!transponderPlan.StartTime.HasValue)
                throw new ArgumentException(ExceptionMessages.NonPermanentTransponderPlanStartTimeIsRequired, nameof(transponderPlan));

            if (!transponderPlan.EndTime.HasValue)
                throw new ArgumentException(ExceptionMessages.NonPermanentTransponderPlanEndTimeIsRequired, nameof(transponderPlan));

            if (transponderPlan.StartTime.Value >= transponderPlan.EndTime.Value)
                throw new ArgumentException(ExceptionMessages.NonPermanentTransponderPlanTimeWindowInvalid, nameof(transponderPlan));

            foreach (var existingPlan in existingPlans.Where(plan => !plan.IsPermanent.GetValueOrDefault()))
            {
                if (!existingPlan.StartTime.HasValue || !existingPlan.EndTime.HasValue)
                    continue;

                if (transponderPlan.StartTime.Value < existingPlan.EndTime.Value && existingPlan.StartTime.Value < transponderPlan.EndTime.Value)
                {
                    throw new ArgumentException(string.Format(ExceptionMessages.TransponderPlanTimeRangeOverlaps, existingPlan.Name), nameof(transponderPlan));
                }
            }
        }
    }
}
