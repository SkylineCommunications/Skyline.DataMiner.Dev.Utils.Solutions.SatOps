namespace Skyline.DataMiner.SDM.SatOps.Common.API
{
    using System;
    using System.Linq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.Registration;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Beam;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TrasponderPlanRow;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Helpers;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;

    public class SatOpsApi : ISatOpsApi
    {
        private const string CatalogItemId = "08798aa7-6c1f-42a9-bdd2-4b3d8b4afea1";

        private readonly Lazy<ISatelliteRepository> lazySatelliteRepository;
        private readonly Lazy<IBeamRepository> lazyBeamRepository;
        private readonly Lazy<ITransponderRepository> lazyTransponderRepository;
        private readonly Lazy<ITransponderPlanRepository> lazyTransponderPlanRepository;
        private readonly Lazy<ITransponderPlanRowRepository> lazyTransponderPlanRowRepository;
        private readonly Lazy<ITransponderSlotRepository> lazyTransponderSlotRepository;

        public SatOpsApi(IConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ValidateConnection(connection);

            SlcSatelliteManagementHelper = new SlcSatelliteManagementHelper(connection);

            lazySatelliteRepository = new Lazy<ISatelliteRepository>(() => new SatelliteRepository(this));
            lazyBeamRepository = new Lazy<IBeamRepository>(() => new BeamRepository(this));
            lazyTransponderRepository = new Lazy<ITransponderRepository>(() => new TransponderRepository(this));
            lazyTransponderPlanRepository = new Lazy<ITransponderPlanRepository>(() => new TransponderPlanRepository(this));
            lazyTransponderPlanRowRepository = new Lazy<ITransponderPlanRowRepository>(() => new TransponderPlanRowRepository(this));
            lazyTransponderSlotRepository = new Lazy<ITransponderSlotRepository>(() => new TransponderSlotRepository(this));
        }

        public IConnection Connection { get; }

        public ISatelliteRepository Satellites => lazySatelliteRepository.Value;

        public IBeamRepository Beams => lazyBeamRepository.Value;

        public ITransponderRepository Transponders => lazyTransponderRepository.Value;

        public ITransponderPlanRepository TransponderPlans => lazyTransponderPlanRepository.Value;

        public ITransponderPlanRowRepository TransponderPlanRows => lazyTransponderPlanRowRepository.Value;

        public ITransponderSlotRepository TransponderSlots => lazyTransponderSlotRepository.Value;

        internal SlcSatelliteManagementHelper SlcSatelliteManagementHelper { get; }

        internal ILogger Logger { get; private set; }

        public bool IsInstalled()
        {
            return IsInstalled(out _);
        }

        public bool IsInstalled(out string version)
        {
            var registrar = Connection.GetSdmRegistrar();
            var satOpsRegistration = registrar.Solutions.Read(SolutionRegistrationExposers.ID.Equal(CatalogItemId)).FirstOrDefault();
            if (satOpsRegistration is null)
            {
                version = null;
                return false;
            }

            version = satOpsRegistration.Version;
            return true;
        }

        public void SetLogger(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void ValidateConnection(IConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (connection.IsShuttingDown)
            {
                throw new InvalidOperationException("The the provided connection is shutting down.");
            }
        }
    }
}
