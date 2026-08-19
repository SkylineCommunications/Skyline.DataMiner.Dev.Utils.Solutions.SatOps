namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TransponderValidationMiddleware : IBulkRepositoryMiddleware<Transponder>
    {
        private readonly Func<Guid, Satellite> satelliteResolver;

        public TransponderValidationMiddleware(Func<Guid, Satellite> satelliteResolver)
        {
            this.satelliteResolver = satelliteResolver ?? throw new ArgumentNullException(nameof(satelliteResolver));
        }

        public Transponder OnCreate(Transponder oToCreate, Func<Transponder, Transponder> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponder(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<Transponder> OnCreate(IEnumerable<Transponder> oToCreate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponder in oToCreate)
            {
                ValidateTransponder(transponder);
            }

            return next(oToCreate);
        }

        public Transponder OnUpdate(Transponder oToUpdate, Func<Transponder, Transponder> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponder(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<Transponder> OnUpdate(IEnumerable<Transponder> oToUpdate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponder in oToUpdate)
            {
                ValidateTransponder(transponder);
            }

            return next(oToUpdate);
        }

        public IReadOnlyCollection<Transponder> OnCreateOrUpdate(IEnumerable<Transponder> oToCreateOrUpdate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var transponder in oToCreateOrUpdate)
            {
                ValidateTransponder(transponder);
            }

            return next(oToCreateOrUpdate);
        }

        public long OnCount(FilterElement<Transponder> filter, Func<FilterElement<Transponder>, long> next)
        {
            return next(filter);
        }

        public long OnCount(IQuery<Transponder> query, Func<IQuery<Transponder>, long> next)
        {
            return next(query);
        }

        public void OnDelete(IEnumerable<Transponder> oToDelete, Action<IEnumerable<Transponder>> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            if(oToDelete.Any(transponder  => transponder == null))
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToDelete));

            next(oToDelete);
        }

        public void OnDelete(Transponder oToDelete, Action<Transponder> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            next(oToDelete);
        }

        public IEnumerable<Transponder> OnRead(FilterElement<Transponder> filter, Func<FilterElement<Transponder>, IEnumerable<Transponder>> next)
        {
            return next(filter);
        }

        public IEnumerable<Transponder> OnRead(IQuery<Transponder> query, Func<IQuery<Transponder>, IEnumerable<Transponder>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<Transponder>> OnReadPaged(FilterElement<Transponder> filter, Func<FilterElement<Transponder>, IEnumerable<IPagedResult<Transponder>>> next)
        {
            return next(filter);
        }

        public IEnumerable<IPagedResult<Transponder>> OnReadPaged(IQuery<Transponder> query, Func<IQuery<Transponder>, IEnumerable<IPagedResult<Transponder>>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<Transponder>> OnReadPaged(FilterElement<Transponder> filter, int pageSize, Func<FilterElement<Transponder>, int, IEnumerable<IPagedResult<Transponder>>> next)
        {
            return next(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Transponder>> OnReadPaged(IQuery<Transponder> query, int pageSize, Func<IQuery<Transponder>, int, IEnumerable<IPagedResult<Transponder>>> next)
        {
            return next(query, pageSize);
        }

        private void ValidateTransponder(Transponder transponder)
        {
            if (transponder == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(transponder));

            if (string.IsNullOrWhiteSpace(transponder.Name))
                throw new ArgumentException("Transponder Name is required.", nameof(transponder));

            if (!transponder.TransponderSatellite.HasValue || transponder.TransponderSatellite.Value == Guid.Empty)
                throw new ArgumentException("Transponder Satellite is required.", nameof(transponder));

            if (!transponder.Bandwidth.HasValue)
                throw new ArgumentException("Bandwidth is required.", nameof(transponder));

            if (!transponder.StartFrequency.HasValue)
                throw new ArgumentException("Start  Frequency is required.", nameof(transponder));

            if (!transponder.StopFrequency.HasValue)
                throw new ArgumentException("StopFrequency is required.", nameof(transponder));

            if (!transponder.DownlinkStartFreq.HasValue)
                throw new ArgumentException("Downlink Start Frequency is required.", nameof(transponder));

            if (!transponder.DownlinkEndFreq.HasValue)
                throw new ArgumentException("Downlink End Frequency is required.", nameof(transponder));

            if (!transponder.HardEndDate.HasValue)
                throw new ArgumentException("Hard End Date is required.", nameof(transponder));

            if (!transponder.DOMResource.HasValue || transponder.DOMResource.Value == Guid.Empty)
                throw new ArgumentException("Resource DOM is required.", nameof(transponder));

            var satelliteId = transponder.TransponderSatellite.Value;
            if (satelliteResolver(satelliteId) == null)
                throw new ArgumentException($"Transponder satellite with id '{satelliteId}' does not exist.", nameof(transponder));
        }
    }
}
