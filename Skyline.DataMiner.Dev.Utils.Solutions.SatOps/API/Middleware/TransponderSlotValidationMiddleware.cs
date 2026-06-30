namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;

    internal sealed class TransponderSlotValidationMiddleware : IBulkRepositoryMiddleware<TransponderSlot>
    {
        private readonly Func<Guid, TransponderPlan> transponderPlanResolver;

        public TransponderSlotValidationMiddleware(Func<Guid, TransponderPlan> transponderPlanResolver)
        {
            this.transponderPlanResolver = transponderPlanResolver ?? throw new ArgumentNullException(nameof(transponderPlanResolver));
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

            foreach (var transponderSlot in oToCreate)
            {
                ValidateTransponderSlot(transponderSlot);
            }

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

            foreach (var transponderSlot in oToUpdate)
            {
                ValidateTransponderSlot(transponderSlot);
            }

            return next(oToUpdate);
        }

        public IReadOnlyCollection<TransponderSlot> OnCreateOrUpdate(IEnumerable<TransponderSlot> oToCreateOrUpdate, Func<IEnumerable<TransponderSlot>, IReadOnlyCollection<TransponderSlot>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponderSlot in oToCreateOrUpdate)
            {
                ValidateTransponderSlot(transponderSlot);
            }

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
            if (transponderSlot == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponderSlot));

            if (!transponderSlot.TransponderPlan.HasValue || transponderSlot.TransponderPlan.Value == Guid.Empty)
                throw new ArgumentException("TransponderPlan is required.", nameof(transponderSlot));

            if (string.IsNullOrWhiteSpace(transponderSlot.SlotName))
                throw new ArgumentException("SlotName is required.", nameof(transponderSlot));

            if (!transponderSlot.SlotStartFrequency.HasValue)
                throw new ArgumentException("SlotStartFrequency is required.", nameof(transponderSlot));

            if (!transponderSlot.SlotEndFrequency.HasValue)
                throw new ArgumentException("SlotEndFrequency is required.", nameof(transponderSlot));

            if (!transponderSlot.Bandwidth.HasValue)
                throw new ArgumentException("Bandwidth is required.", nameof(transponderSlot));

            if (!transponderSlot.UplinkFreq.HasValue)
                throw new ArgumentException("UplinkFreq is required.", nameof(transponderSlot));

            if (!transponderSlot.DownlinkFreq.HasValue)
                throw new ArgumentException("DownlinkFreq is required.", nameof(transponderSlot));

            var transponderPlanId = transponderSlot.TransponderPlan.Value;
            if (transponderPlanResolver(transponderPlanId) == null)
                throw new ArgumentException($"Transponder plan with id '{transponderPlanId}' does not exist.", nameof(transponderSlot));
        }
    }
}
