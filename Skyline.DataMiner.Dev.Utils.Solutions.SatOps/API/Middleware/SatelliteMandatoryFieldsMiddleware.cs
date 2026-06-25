namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;

    internal sealed class SatelliteMandatoryFieldsMiddleware : BulkRepositoryMiddlewareBase<Satellite>
    {
        public override Satellite OnCreate(Satellite oToCreate, Func<Satellite, Satellite> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSatellite(oToCreate);
            return next(oToCreate);
        }

        public override Satellite OnUpdate(Satellite oToUpdate, Func<Satellite, Satellite> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateSatellite(oToUpdate);
            return next(oToUpdate);
        }

        public override IReadOnlyCollection<Satellite> OnCreateOrUpdate(IEnumerable<Satellite> oToCreateOrUpdate, Func<IEnumerable<Satellite>, IReadOnlyCollection<Satellite>> next)
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
