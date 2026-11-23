namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TransponderValidationMiddleware : IBulkRepositoryMiddleware<Transponder>
    {
        private readonly ISatelliteRepository satelliteRepository;

        public TransponderValidationMiddleware(ISatelliteRepository satelliteRepository)
        {
            this.satelliteRepository = satelliteRepository ?? throw new ArgumentNullException(nameof(satelliteRepository));
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

            ValidateTransponders(oToCreate);

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

            ValidateTransponders(oToUpdate);

            return next(oToUpdate);
        }

        public IReadOnlyCollection<Transponder> OnCreateOrUpdate(IEnumerable<Transponder> oToCreateOrUpdate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateTransponders(oToCreateOrUpdate);

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
            ValidateTransponder(transponder, null);
        }

        /// <summary>
        /// Validates a single transponder.
        /// </summary>
        /// <param name="transponder">The transponder to validate.</param>
        /// <param name="knownSatelliteIds">
        /// The identifiers of the satellites that were already resolved for the whole batch, or <see langword="null"/>
        /// when the transponder is validated on its own and the satellite still has to be looked up.
        /// </param>
        private void ValidateTransponder(Transponder transponder, ISet<Guid> knownSatelliteIds)
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
            var satelliteExists = knownSatelliteIds != null
                ? knownSatelliteIds.Contains(satelliteId)
                : satelliteRepository.Read(satelliteId) != null;

            if (!satelliteExists)
                throw new ArgumentException($"Transponder satellite with id '{satelliteId}' does not exist.", nameof(transponder));
        }

        /// <summary>
        /// Validates every transponder of a batch, resolving the referenced satellites with a single repository read
        /// instead of one read per transponder.
        /// </summary>
        /// <remarks>
        /// The satellites are resolved up front, but the per-transponder checks keep their original order so that a
        /// batch containing an invalid transponder still reports the same error as before.
        /// </remarks>
        private void ValidateTransponders(IEnumerable<Transponder> transponders)
        {
            var transpondersToValidate = transponders as IReadOnlyCollection<Transponder> ?? transponders.ToList();
            var knownSatelliteIds = ReferenceValidationHelper.ReadExistingIds(transpondersToValidate, transponder => transponder.TransponderSatellite, satelliteRepository);

            foreach (var transponder in transpondersToValidate)
            {
                ValidateTransponder(transponder, knownSatelliteIds);
            }
        }
    }
}
