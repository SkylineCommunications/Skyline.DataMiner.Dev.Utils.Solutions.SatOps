namespace Skyline.DataMiner.SDM.SatOps.Common.API
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.Registration;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Beam;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;
    using Skyline.DataMiner.SDM.SatOps.Common.Storage.DOM.Helpers;
    using System;
    using System.Linq;

    /// <summary>
    /// Provides access to the SatOps (Satellite Operations) API and its repositories.
    /// </summary>
    public class SatOpsApi : ISatOpsApi
    {
        private const string CatalogItemId = "08798aa7-6c1f-42a9-bdd2-4b3d8b4afea1";

        private readonly Lazy<ISatelliteRepository> lazySatelliteRepository;
        private readonly Lazy<IBeamRepository> lazyBeamRepository;
        private readonly Lazy<ITransponderRepository> lazyTransponderRepository;
        private readonly Lazy<ITransponderPlanRepository> lazyTransponderPlanRepository;
        private readonly Lazy<ITransponderPlanRowRepository> lazyTransponderPlanRowRepository;
        private readonly Lazy<ITransponderSlotRepository> lazyTransponderSlotRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SatOpsApi"/> class.
        /// </summary>
        /// <param name="connection">The DataMiner connection to use for all operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="connection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the provided connection is shutting down.</exception>
        public SatOpsApi(IConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ValidateConnection(connection);

            SlcSatelliteManagementHelper = new SlcSatelliteManagementHelper(connection);

            lazySatelliteRepository = new Lazy<ISatelliteRepository>
                (
                    () => new SatelliteRepository(this)
                              .WithMiddleware(new SatelliteValidationMiddleware())
                );
            lazyBeamRepository = new Lazy<IBeamRepository>
                (
                    () => new BeamRepository(this)
                              .WithMiddleware(new BeamValidationMiddleware(satelliteId => Satellites.Read(satelliteId)))
                );
            lazyTransponderRepository = new Lazy<ITransponderRepository>
                (
                    () => new TransponderRepository(this)
                              .WithMiddleware(new TransponderValidationMiddleware(satelliteId => Satellites.Read(satelliteId)))
                              .WithMiddleware(new TransponderResourceCreationMiddleware(connection, satelliteId => Satellites.Read(satelliteId), Logger))
                );
            lazyTransponderPlanRepository = new Lazy<ITransponderPlanRepository>
                (
                    () => new TransponderPlanRepository(this)
                              .WithMiddleware(new TransponderPlanValidationMiddleware(transponderId => Transponders.Read(transponderId)))
                );
            lazyTransponderPlanRowRepository = new Lazy<ITransponderPlanRowRepository>
                (
                    () => new TransponderPlanRowRepository(this)
                              .WithMiddleware(new TransponderPlanRowValidationMiddleware(transponderPlanId => TransponderPlans.Read(transponderPlanId)))
                );
            lazyTransponderSlotRepository = new Lazy<ITransponderSlotRepository>
                (
                    () => new TransponderSlotRepository(this)
                              .WithMiddleware(new TransponderSlotValidationMiddleware(transponderPlanId => TransponderPlans.Read(transponderPlanId)))
                );
        }

        /// <summary>
        /// Gets the DataMiner connection used by this API instance.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Gets the repository for managing satellite entities.
        /// </summary>
        public ISatelliteRepository Satellites => lazySatelliteRepository.Value;

        /// <summary>
        /// Gets the repository for managing beam entities.
        /// </summary>
        public IBeamRepository Beams => lazyBeamRepository.Value;

        /// <summary>
        /// Gets the repository for managing transponder entities.
        /// </summary>
        public ITransponderRepository Transponders => lazyTransponderRepository.Value;

        /// <summary>
        /// Gets the repository for managing transponder plan entities.
        /// </summary>
        public ITransponderPlanRepository TransponderPlans => lazyTransponderPlanRepository.Value;

        /// <summary>
        /// Gets the repository for managing transponder plan row entities.
        /// </summary>
        public ITransponderPlanRowRepository TransponderPlanRows => lazyTransponderPlanRowRepository.Value;

        /// <summary>
        /// Gets the repository for managing transponder slot entities.
        /// </summary>
        public ITransponderSlotRepository TransponderSlots => lazyTransponderSlotRepository.Value;

        /// <summary>
        /// Gets the helper for satellite management operations.
        /// </summary>
        internal SlcSatelliteManagementHelper SlcSatelliteManagementHelper { get; }

        /// <summary>
        /// Gets the logger instance for this API.
        /// </summary>
        internal ILogger Logger { get; private set; }

        /// <summary>
        /// Checks whether the SatOps solution is installed on the DataMiner system.
        /// </summary>
        /// <returns><c>true</c> if the SatOps solution is installed; otherwise, <c>false</c>.</returns>
        public bool IsInstalled()
        {
            return IsInstalled(out _);
        }

        /// <summary>
        /// Checks whether the SatOps solution is installed on the DataMiner system.
        /// </summary>
        /// <param name="version">When this method returns, contains the version of the installed solution if it exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the SatOps solution is installed; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Sets the logger instance for this API.
        /// </summary>
        /// <param name="logger">The logger to use for logging operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <c>null</c>.</exception>
        public void SetLogger(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Validates that the provided connection is not null and not shutting down.
        /// </summary>
        /// <param name="connection">The connection to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="connection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the connection is shutting down.</exception>
        private void ValidateConnection(IConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (connection.IsShuttingDown)
            {
                throw new InvalidOperationException("The provided connection is shutting down.");
            }
        }
    }
}
