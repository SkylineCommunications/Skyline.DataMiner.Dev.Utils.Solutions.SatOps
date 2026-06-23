namespace Skyline.DataMiner.SDM.SatOps.Common.API
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;

    public interface ISatOpsApi
    {
        IConnection Connection { get; }

        SatelliteRepository Satellites { get; }

        BeamRepository Beams { get; }

        TransponderRepository Transponders { get; }

        TransponderPlanRepository TransponderPlans { get; }

        TransponderPlanRowRepository TransponderPlanRows { get; }

        TransponderSlotRepository TransponderSlots { get; }

        void InstallDomModules();

        bool IsInstalled();

        bool IsInstalled(out string version);

        void SetLogger(ILogger logger);

    }
}
