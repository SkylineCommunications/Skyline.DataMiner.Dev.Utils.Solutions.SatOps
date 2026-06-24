namespace Skyline.DataMiner.SDM.SatOps.Common.API
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Beam;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TrasponderPlanRow;
    using Skyline.DataMiner.SDM.SatOps.Common.Logging;

    public interface ISatOpsApi
    {
        IConnection Connection { get; }

        ISatelliteRepository Satellites { get; }

        IBeamRepository Beams { get; }

        ITransponderRepository Transponders { get; }

        ITransponderPlanRepository TransponderPlans { get; }

        ITransponderPlanRowRepository TransponderPlanRows { get; }

        ITransponderSlotRepository TransponderSlots { get; }

        bool IsInstalled();

        bool IsInstalled(out string version);

        void SetLogger(ILogger logger);

    }
}
