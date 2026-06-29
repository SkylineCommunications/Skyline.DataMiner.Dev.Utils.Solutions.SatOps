namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using SLDataGateway.API.Types.Querying;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;

    internal sealed class SatelliteValidationMiddleware : IBulkRepositoryMiddleware<Satellite>
    {
        public Satellite OnCreate(Satellite oToCreate, Func<Satellite, Satellite> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSatellite(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<Satellite> OnCreate(IEnumerable<Satellite> oToCreate, Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var satellite in oToCreate)
            {
                ValidateSatellite(satellite);
            }

            return next(oToCreate);
        }

        public Satellite OnUpdate(Satellite oToUpdate, Func<Satellite, Satellite> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSatellite(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<Satellite> OnUpdate(IEnumerable<Satellite> oToUpdate, Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var satellite in oToUpdate)
            {
                ValidateSatellite(satellite);
            }

            return next(oToUpdate);
        }

        public IReadOnlyCollection<Satellite> OnCreateOrUpdate(IEnumerable<Satellite> oToCreateOrUpdate, Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var satellite in oToCreateOrUpdate)
            {
                ValidateSatellite(satellite);
            }

            return next(oToCreateOrUpdate);
        }

        public long OnCount(FilterElement<Satellite> filter, Func<FilterElement<Satellite>, long> next)
        {
            return next(filter);
        }

        public long OnCount(IQuery<Satellite> query, Func<IQuery<Satellite>, long> next)
        {
            return next(query);
        }

        public void OnDelete(IEnumerable<Satellite> oToDelete, Action<IEnumerable<Satellite>> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var satellite in oToDelete)
            {
                ValidateSatellite(satellite);
            }

            next(oToDelete);
        }

        public void OnDelete(Satellite oToDelete, Action<Satellite> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            next(oToDelete);
        }

        public IEnumerable<Satellite> OnRead(FilterElement<Satellite> filter, Func<FilterElement<Satellite>, IEnumerable<Satellite>> next)
        {
            return next(filter);
        }

        public IEnumerable<Satellite> OnRead(IQuery<Satellite> query, Func<IQuery<Satellite>, IEnumerable<Satellite>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<Satellite>> OnReadPaged(FilterElement<Satellite> filter, Func<FilterElement<Satellite>, IEnumerable<IPagedResult<Satellite>>> next)
        {
            return next(filter);
        }

        public IEnumerable<IPagedResult<Satellite>> OnReadPaged(IQuery<Satellite> query, Func<IQuery<Satellite>, IEnumerable<IPagedResult<Satellite>>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<Satellite>> OnReadPaged(FilterElement<Satellite> filter, int pageSize, Func<FilterElement<Satellite>, int, IEnumerable<IPagedResult<Satellite>>> next)
        {
            return next(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Satellite>> OnReadPaged(IQuery<Satellite> query, int pageSize, Func<IQuery<Satellite>, int, IEnumerable<IPagedResult<Satellite>>> next)
        {
            return next(query, pageSize);
        }

        private static void ValidateSatellite(Satellite satellite)
        {
            if (satellite == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(satellite));

            if (string.IsNullOrWhiteSpace(satellite.Name))
                throw new ArgumentException(ExceptionMessages.SatelliteNameIsRequired, nameof(satellite));

            if (string.IsNullOrWhiteSpace(satellite.Abbreviation))
                throw new ArgumentException(ExceptionMessages.SatelliteAbbreviationIsRequired, nameof(satellite));
        }
    }
}
