namespace Skyline.DataMiner.Solutions.SatOps.Common.API
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;

    /// <summary>
    /// Defines the main API interface for SatOps operations.
    /// </summary>
    public interface ISatOpsApi
    {
        /// <summary>
        /// Gets the DataMiner connection.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Gets the satellite repository.
        /// </summary>
        ISatelliteRepository Satellites { get; }

        /// <summary>
        /// Gets the beam repository.
        /// </summary>
        IBeamRepository Beams { get; }

        /// <summary>
        /// Gets the transponder repository.
        /// </summary>
        ITransponderRepository Transponders { get; }

        /// <summary>
        /// Gets the transponder plan repository.
        /// </summary>
        ITransponderPlanRepository TransponderPlans { get; }

        /// <summary>
        /// Gets the transponder plan row repository.
        /// </summary>
        ITransponderPlanRowRepository TransponderPlanRows { get; }

        /// <summary>
        /// Gets the transponder range reservation repository.
        /// </summary>
        ITransponderRangeReservationRepository TransponderRangeReservations { get; }

        /// <summary>
        /// Gets the transponder slot repository.
        /// </summary>
        ITransponderSlotRepository TransponderSlots { get; }

        /// <summary>
        /// Checks whether the SatOps solution is installed.
        /// </summary>
        /// <returns><c>true</c> if the solution is installed; otherwise, <c>false</c>.</returns>
        bool IsInstalled();

        /// <summary>
        /// Checks whether the SatOps solution is installed and retrieves the version.
        /// </summary>
        /// <param name="version">When this method returns, contains the version of the installed solution if it is installed; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the solution is installed; otherwise, <c>false</c>.</returns>
        bool IsInstalled(out string version);

        /// <summary>
        /// Sets the logger for the API.
        /// </summary>
        /// <param name="logger">The logger instance to use.</param>
        void SetLogger(ILogger logger);
    }
}
