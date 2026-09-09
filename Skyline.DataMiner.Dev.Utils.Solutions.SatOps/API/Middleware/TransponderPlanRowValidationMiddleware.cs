namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TransponderPlanRowValidationMiddleware : IBulkRepositoryMiddleware<TransponderPlanRow>
    {
        private readonly ITransponderPlanRepository transponderPlanRepository;
        private readonly ITransponderPlanRowRepository transponderPlanRowRepository;

        public TransponderPlanRowValidationMiddleware(ITransponderPlanRepository transponderPlanRepository, ITransponderPlanRowRepository transponderPlanRowRepository)
        {
            this.transponderPlanRepository = transponderPlanRepository ?? throw new ArgumentNullException(nameof(transponderPlanRepository));
            this.transponderPlanRowRepository = transponderPlanRowRepository ?? throw new ArgumentNullException(nameof(transponderPlanRowRepository));
        }

        public TransponderPlanRow OnCreate(TransponderPlanRow oToCreate, Func<TransponderPlanRow, TransponderPlanRow> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponderPlanRow(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<TransponderPlanRow> OnCreate(IEnumerable<TransponderPlanRow> oToCreate, Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var rowsToCreate = oToCreate.ToList();
            ValidateTransponderPlanRows(rowsToCreate);

            ValidateDuplicateBandwidthInBatch(rowsToCreate);
            return next(rowsToCreate);
        }

        public TransponderPlanRow OnUpdate(TransponderPlanRow oToUpdate, Func<TransponderPlanRow, TransponderPlanRow> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponderPlanRow(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderPlanRow> OnUpdate(IEnumerable<TransponderPlanRow> oToUpdate, Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var rowsToUpdate = oToUpdate.ToList();
            ValidateTransponderPlanRows(rowsToUpdate);

            ValidateDuplicateBandwidthInBatch(rowsToUpdate);
            return next(rowsToUpdate);
        }

        public IReadOnlyCollection<TransponderPlanRow> OnCreateOrUpdate(IEnumerable<TransponderPlanRow> oToCreateOrUpdate, Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var rowsToCreateOrUpdate = oToCreateOrUpdate.ToList();
            ValidateTransponderPlanRows(rowsToCreateOrUpdate);

            ValidateDuplicateBandwidthInBatch(rowsToCreateOrUpdate);
            return next(rowsToCreateOrUpdate);
        }

        public long OnCount(FilterElement<TransponderPlanRow> filter, Func<FilterElement<TransponderPlanRow>, long> next)
        {
            return next(filter);
        }

        public long OnCount(IQuery<TransponderPlanRow> query, Func<IQuery<TransponderPlanRow>, long> next)
        {
            return next(query);
        }

        public void OnDelete(IEnumerable<TransponderPlanRow> oToDelete, Action<IEnumerable<TransponderPlanRow>> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponderPlanRow in oToDelete)
            {
                ValidateTransponderPlanRow(transponderPlanRow);
            }

            next(oToDelete);
        }

        public void OnDelete(TransponderPlanRow oToDelete, Action<TransponderPlanRow> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            next(oToDelete);
        }

        public IEnumerable<TransponderPlanRow> OnRead(FilterElement<TransponderPlanRow> filter, Func<FilterElement<TransponderPlanRow>, IEnumerable<TransponderPlanRow>> next)
        {
            return next(filter);
        }

        public IEnumerable<TransponderPlanRow> OnRead(IQuery<TransponderPlanRow> query, Func<IQuery<TransponderPlanRow>, IEnumerable<TransponderPlanRow>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> OnReadPaged(FilterElement<TransponderPlanRow> filter, Func<FilterElement<TransponderPlanRow>, IEnumerable<IPagedResult<TransponderPlanRow>>> next)
        {
            return next(filter);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> OnReadPaged(IQuery<TransponderPlanRow> query, Func<IQuery<TransponderPlanRow>, IEnumerable<IPagedResult<TransponderPlanRow>>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> OnReadPaged(FilterElement<TransponderPlanRow> filter, int pageSize, Func<FilterElement<TransponderPlanRow>, int, IEnumerable<IPagedResult<TransponderPlanRow>>> next)
        {
            return next(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderPlanRow>> OnReadPaged(IQuery<TransponderPlanRow> query, int pageSize, Func<IQuery<TransponderPlanRow>, int, IEnumerable<IPagedResult<TransponderPlanRow>>> next)
        {
            return next(query, pageSize);
        }

        private void ValidateTransponderPlanRow(TransponderPlanRow transponderPlanRow)
        {
            ValidateTransponderPlanRow(transponderPlanRow, null, null);
        }

        /// <summary>
        /// Validates a single transponder plan row.
        /// </summary>
        /// <param name="transponderPlanRow">The row to validate.</param>
        /// <param name="knownTransponderPlanIds">
        /// The identifiers of the transponder plans that were already resolved for the whole batch, or
        /// <see langword="null"/> when the row is validated on its own and the plan still has to be looked up.
        /// </param>
        /// <param name="rowCache">
        /// Cache of the persisted rows per transponder plan, shared by all rows of the same batch, or
        /// <see langword="null"/> when the row is validated on its own.
        /// </param>
        private void ValidateTransponderPlanRow(TransponderPlanRow transponderPlanRow, ISet<Guid> knownTransponderPlanIds, IDictionary<Guid, IReadOnlyList<TransponderPlanRow>> rowCache)
        {
            if (transponderPlanRow == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponderPlanRow));

            if (!transponderPlanRow.TransponderPlan.HasValue || transponderPlanRow.TransponderPlan.Value == Guid.Empty)
                throw new ArgumentException("Transponder Plan is required.", nameof(transponderPlanRow));

            if (!transponderPlanRow.Bandwidth.HasValue)
                throw new ArgumentException(ExceptionMessages.TransponderPlanRowBandwidthIsRequired, nameof(transponderPlanRow));

            if (transponderPlanRow.Bandwidth.Value <= 0)
                throw new ArgumentException(ExceptionMessages.TransponderPlanRowBandwidthMustBeGreaterThanZero, nameof(transponderPlanRow));

            if (!transponderPlanRow.StepSize.HasValue)
                throw new ArgumentException(ExceptionMessages.TransponderPlanRowStepSizeIsRequired, nameof(transponderPlanRow));

            if (transponderPlanRow.StepSize.Value <= 0)
                throw new ArgumentException(ExceptionMessages.TransponderPlanRowStepSizeMustBeGreaterThanZero, nameof(transponderPlanRow));

            if (transponderPlanRow.StepSize.Value < transponderPlanRow.Bandwidth.Value)
                throw new ArgumentException(ExceptionMessages.TransponderPlanRowStepSizeMustBeGreaterThanOrEqualToBandwidth, nameof(transponderPlanRow));

            if (!transponderPlanRow.Offset.HasValue)
                throw new ArgumentException("Offset is required.", nameof(transponderPlanRow));

            var transponderPlanId = transponderPlanRow.TransponderPlan.Value;
            var transponderPlanExists = knownTransponderPlanIds != null
                ? knownTransponderPlanIds.Contains(transponderPlanId)
                : transponderPlanRepository.Read(transponderPlanId) != null;

            if (!transponderPlanExists)
                throw new ArgumentException($"Transponder plan with id '{transponderPlanId}' does not exist.", nameof(transponderPlanRow));

            ValidateDuplicateBandwidthAgainstExistingRows(transponderPlanRow, transponderPlanId, rowCache);
        }

        /// <summary>
        /// Validates every row of a batch, resolving the referenced transponder plans with a single repository read and
        /// reading the persisted rows once per distinct plan instead of once per submitted row.
        /// </summary>
        /// <remarks>
        /// The plans are resolved up front, but the per-row checks keep their original order so that a batch containing
        /// an invalid row still reports the same error as before.
        /// </remarks>
        private void ValidateTransponderPlanRows(IReadOnlyCollection<TransponderPlanRow> transponderPlanRows)
        {
            var knownTransponderPlanIds = ReferenceValidationHelper.ReadExistingIds(transponderPlanRows, row => row.TransponderPlan, transponderPlanRepository);
            var rowCache = new Dictionary<Guid, IReadOnlyList<TransponderPlanRow>>();

            foreach (var transponderPlanRow in transponderPlanRows)
            {
                ValidateTransponderPlanRow(transponderPlanRow, knownTransponderPlanIds, rowCache);
            }
        }

        /// <summary>
        /// Returns the persisted rows of a transponder plan, reusing the batch cache when one is available.
        /// </summary>
        private IReadOnlyList<TransponderPlanRow> ReadRowsByTransponderPlan(Guid transponderPlanId, IDictionary<Guid, IReadOnlyList<TransponderPlanRow>> rowCache)
        {
            return ReferenceValidationHelper.ReadCached(
                transponderPlanId,
                rowCache,
                planId => transponderPlanRowRepository.Read(TransponderPlanRowExposers.TransponderPlan.Equal(planId)));
        }

        private void ValidateDuplicateBandwidthAgainstExistingRows(TransponderPlanRow transponderPlanRow, Guid transponderPlanId, IDictionary<Guid, IReadOnlyList<TransponderPlanRow>> rowCache)
        {
            var duplicateExists = ReadRowsByTransponderPlan(transponderPlanId, rowCache)
                .Where(row => row != null && row.Id != transponderPlanRow.Id && row.Bandwidth.HasValue)
                .Any(row => row.Bandwidth.Value == transponderPlanRow.Bandwidth.Value);

            if (!duplicateExists)
                return;

            throw new ArgumentException(
                string.Format(ExceptionMessages.DuplicateTransponderPlanRowBandwidthDetected, transponderPlanRow.Bandwidth.Value, transponderPlanId),
                nameof(transponderPlanRow));
        }

        private static void ValidateDuplicateBandwidthInBatch(IEnumerable<TransponderPlanRow> transponderPlanRows)
        {
            var duplicate = transponderPlanRows
                .Where(row => row != null
                    && row.TransponderPlan.HasValue
                    && row.TransponderPlan.Value != Guid.Empty
                    && row.Bandwidth.HasValue)
                .GroupBy(row => new { PlanId = row.TransponderPlan.Value, Bandwidth = row.Bandwidth.Value })
                .FirstOrDefault(group => group.Count() > 1);

            if (duplicate == null)
                return;

            throw new ArgumentException(
                string.Format(ExceptionMessages.DuplicateTransponderPlanRowBandwidthDetected, duplicate.Key.Bandwidth, duplicate.Key.PlanId));
        }
    }
}
