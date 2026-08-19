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

            ValidateTransponderPlan(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<TransponderPlan> OnCreate(IEnumerable<TransponderPlan> oToCreate, Func<IEnumerable<TransponderPlan>, IReadOnlyCollection<TransponderPlan>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponderPlan in oToCreate)
            {
                ValidateTransponderPlan(transponderPlan);
            }

            return next(oToCreate);
        }

        public TransponderPlan OnUpdate(TransponderPlan oToUpdate, Func<TransponderPlan, TransponderPlan> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponderPlan(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderPlan> OnUpdate(IEnumerable<TransponderPlan> oToUpdate, Func<IEnumerable<TransponderPlan>, IReadOnlyCollection<TransponderPlan>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponderPlan in oToUpdate)
            {
                ValidateTransponderPlan(transponderPlan);
            }

            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderPlan> OnCreateOrUpdate(IEnumerable<TransponderPlan> oToCreateOrUpdate, Func<IEnumerable<TransponderPlan>, IReadOnlyCollection<TransponderPlan>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponderPlan in oToCreateOrUpdate)
            {
                ValidateTransponderPlan(transponderPlan);
            }

            return next(oToCreateOrUpdate);
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
            if (transponderRepository.Read(transponderId) == null)
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderWithIdDoesNotExist, transponderId), nameof(transponderPlan));

            ValidatePlanConstraints(transponderPlan, transponderId);
        }

        private void ValidatePlanConstraints(TransponderPlan transponderPlan, Guid transponderId)
        {
            var existingPlans = (transponderPlanRepository.ReadByTransponder(transponderId) ?? Enumerable.Empty<TransponderPlan>())
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
