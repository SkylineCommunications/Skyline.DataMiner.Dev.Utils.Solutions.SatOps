namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using SLDataGateway.API.Types.Querying;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;

    internal sealed class BeamValidationMiddleware : IMiddlewareMarker<Beam>, ICreatableMiddleware<Beam>, IBulkCreatableMiddleware<Beam>, IUpdatableMiddleware<Beam>, IBulkUpdatableMiddleware<Beam>, IBulkRepositoryMiddleware<Beam>
    {
        private readonly Func<Guid, Satellite> satelliteResolver;

        public BeamValidationMiddleware(Func<Guid, Satellite> satelliteResolver)
        {
            this.satelliteResolver = satelliteResolver ?? throw new ArgumentNullException(nameof(satelliteResolver));
        }

        public Beam OnCreate(Beam oToCreate, Func<Beam, Beam> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBeam(oToCreate);
            return next(oToCreate);
        }

        public IReadOnlyCollection<Beam> OnCreate(IEnumerable<Beam> oToCreate, Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var beam in oToCreate)
            {
                ValidateBeam(beam);
            }

            return next(oToCreate);
        }

        public Beam OnUpdate(Beam oToUpdate, Func<Beam, Beam> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBeam(oToUpdate);
            return next(oToUpdate);
        }

        public IReadOnlyCollection<Beam> OnUpdate(IEnumerable<Beam> oToUpdate, Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var beam in oToUpdate)
            {
                ValidateBeam(beam);
            }

            return next(oToUpdate);
        }

        public IReadOnlyCollection<Beam> OnCreateOrUpdate(IEnumerable<Beam> oToCreateOrUpdate, Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            foreach (var beam in oToCreateOrUpdate)
            {
                ValidateBeam(beam);
            }

            return next(oToCreateOrUpdate);
        }

        public long OnCount(FilterElement<Beam> filter, Func<FilterElement<Beam>, long> next)
        {
            return next(filter);
        }

        public long OnCount(IQuery<Beam> query, Func<IQuery<Beam>, long> next)
        {
            return next(query);
        }

        public void OnDelete(IEnumerable<Beam> oToDelete, Action<IEnumerable<Beam>> next)
        {
            next(oToDelete);
        }

        public void OnDelete(Beam oToDelete, Action<Beam> next)
        {
            next(oToDelete);
        }

        public IEnumerable<Beam> OnRead(FilterElement<Beam> filter, Func<FilterElement<Beam>, IEnumerable<Beam>> next)
        {
            return next(filter);
        }

        public IEnumerable<Beam> OnRead(IQuery<Beam> query, Func<IQuery<Beam>, IEnumerable<Beam>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<Beam>> OnReadPaged(FilterElement<Beam> filter, Func<FilterElement<Beam>, IEnumerable<IPagedResult<Beam>>> next)
        {
            return next(filter);
        }

        public IEnumerable<IPagedResult<Beam>> OnReadPaged(IQuery<Beam> query, Func<IQuery<Beam>, IEnumerable<IPagedResult<Beam>>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<Beam>> OnReadPaged(FilterElement<Beam> filter, int pageSize, Func<FilterElement<Beam>, int, IEnumerable<IPagedResult<Beam>>> next)
        {
            return next(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Beam>> OnReadPaged(IQuery<Beam> query, int pageSize, Func<IQuery<Beam>, int, IEnumerable<IPagedResult<Beam>>> next)
        {
            return next(query, pageSize);
        }

        private void ValidateBeam(Beam beam)
        {
            if (beam == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(beam));

            if (!beam.BeamSatellite.HasValue || beam.BeamSatellite.Value == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.BeamSatelliteIsRequired, nameof(beam));

            var satelliteId = beam.BeamSatellite.Value;
            if (satelliteResolver(satelliteId) == null)
                throw new ArgumentException(string.Format(ExceptionMessages.BeamSatelliteDoesNotExist, satelliteId), nameof(beam));
        }
    }
}
