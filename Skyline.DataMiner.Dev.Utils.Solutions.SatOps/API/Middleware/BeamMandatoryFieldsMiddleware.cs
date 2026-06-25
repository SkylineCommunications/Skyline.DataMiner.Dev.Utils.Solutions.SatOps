namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;

    internal sealed class BeamMandatoryFieldsMiddleware : BulkRepositoryMiddlewareBase<Beam>
    {
        public override Beam OnCreate(Beam oToCreate, Func<Beam, Beam> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBeam(oToCreate);
            return next(oToCreate);
        }

        public override Beam OnUpdate(Beam oToUpdate, Func<Beam, Beam> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateBeam(oToUpdate);
            return next(oToUpdate);
        }

        public override IReadOnlyCollection<Beam> OnCreateOrUpdate(IEnumerable<Beam> oToCreateOrUpdate, Func<IEnumerable<Beam>, IReadOnlyCollection<Beam>> next)
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

        private static void ValidateBeam(Beam beam)
        {
            if (beam == null)
                throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(beam));

            if (!beam.BeamSatellite.HasValue || beam.BeamSatellite.Value == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.BeamSatelliteIsRequired, nameof(beam));
        }
    }
}
