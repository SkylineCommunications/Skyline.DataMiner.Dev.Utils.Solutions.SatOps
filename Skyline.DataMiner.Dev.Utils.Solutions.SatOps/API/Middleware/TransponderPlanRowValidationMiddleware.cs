namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;

    internal sealed class TransponderPlanRowValidationMiddleware : IBulkRepositoryMiddleware<TransponderPlanRow>
    {
        private readonly Func<Guid, TransponderPlan> transponderPlanResolver;

        public TransponderPlanRowValidationMiddleware(Func<Guid, TransponderPlan> transponderPlanResolver)
        {
            this.transponderPlanResolver = transponderPlanResolver ?? throw new ArgumentNullException(nameof(transponderPlanResolver));
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

            foreach (var transponderPlanRow in oToCreate)
            {
                ValidateTransponderPlanRow(transponderPlanRow);
            }

            return next(oToCreate);
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

            foreach (var transponderPlanRow in oToUpdate)
            {
                ValidateTransponderPlanRow(transponderPlanRow);
            }

            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderPlanRow> OnCreateOrUpdate(IEnumerable<TransponderPlanRow> oToCreateOrUpdate, Func<IEnumerable<TransponderPlanRow>, IReadOnlyCollection<TransponderPlanRow>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponderPlanRow in oToCreateOrUpdate)
            {
                ValidateTransponderPlanRow(transponderPlanRow);
            }

            return next(oToCreateOrUpdate);
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
            if (transponderPlanRow == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponderPlanRow));

            if (!transponderPlanRow.TransponderPlan.HasValue || transponderPlanRow.TransponderPlan.Value == Guid.Empty)
                throw new ArgumentException("Transponder Plan is required.", nameof(transponderPlanRow));

            if (!transponderPlanRow.Bandwidth.HasValue)
                throw new ArgumentException("Bandwidth is required.", nameof(transponderPlanRow));

            if (!transponderPlanRow.StepSize.HasValue)
                throw new ArgumentException("Step Size is required.", nameof(transponderPlanRow));

            if (!transponderPlanRow.Offset.HasValue)
                throw new ArgumentException("Offset is required.", nameof(transponderPlanRow));

            var transponderPlanId = transponderPlanRow.TransponderPlan.Value;
            if (transponderPlanResolver(transponderPlanId) == null)
                throw new ArgumentException($"Transponder plan with id '{transponderPlanId}' does not exist.", nameof(transponderPlanRow));
        }
    }
}
