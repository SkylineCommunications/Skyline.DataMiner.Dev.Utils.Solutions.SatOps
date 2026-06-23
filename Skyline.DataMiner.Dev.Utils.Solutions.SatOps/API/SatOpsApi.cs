namespace Skyline.DataMiner.SDM.SatOps.Common.API
{
    using System;
    using System.Linq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.Registration;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Helpers;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;

    public class SatOpsApi : ISatOpsApi
    {
        private const string CatalogItemId = "08798aa7-6c1f-42a9-bdd2-4b3d8b4afea1";

        public SatOpsApi(IConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ValidateConnection(connection);

            SlcSatelliteManagementHelper = new SlcSatelliteManagementHelper(connection);

            Satellites = new SatelliteRepository(this);
            Beams = new BeamRepository(this);
            Transponders = new TransponderRepository(this);
            TransponderPlans = new TransponderPlanRepository(this);
            TransponderPlanRows = new TransponderPlanRowRepository(this);
            TransponderSlots = new TransponderSlotRepository(this);
        }

        public IConnection Connection { get; }

        public SatelliteRepository Satellites { get; }

        public BeamRepository Beams { get; }

        public TransponderRepository Transponders { get; }

        public TransponderPlanRepository TransponderPlans { get; }

        public TransponderPlanRowRepository TransponderPlanRows { get; }

        public TransponderSlotRepository TransponderSlots { get; }

        internal SlcSatelliteManagementHelper SlcSatelliteManagementHelper { get; }

        internal ILogger Logger { get; private set; }


        public void InstallDomModules()
        {
            Action<string> logAction = x => Logger?.Information(x);

            //TODO: Add Dom module install logic.

        }

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
