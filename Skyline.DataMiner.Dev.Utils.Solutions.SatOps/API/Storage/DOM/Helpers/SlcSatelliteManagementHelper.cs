using Skyline.DataMiner.Net;
namespace Skyline.DataMiner.SDM.SatOps.Common.API.Storage.DOM.Helpers
{
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;

    internal class SlcSatelliteManagementHelper : DomModuleHelperBase
    {
        public SlcSatelliteManagementHelper(IConnection connection) : base(SlcSatellite_ManagementIds.ModuleId, connection)
        {
        }
    }
}
