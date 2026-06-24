namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Transponder
{
    internal class TransponderRepository : Repository, ITransponderRepository
    {
        public TransponderRepository(SatOpsApi satOpsApi) : base(satOpsApi)
        {
        }
    }
}
