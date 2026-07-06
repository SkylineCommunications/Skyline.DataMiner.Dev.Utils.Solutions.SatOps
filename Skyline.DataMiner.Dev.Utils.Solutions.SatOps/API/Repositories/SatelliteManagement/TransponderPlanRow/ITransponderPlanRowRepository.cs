namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow
{
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories;

    /// <summary>
    /// Represents a repository for managing <see cref="TransponderPlanRow"/> entities.
    /// </summary>
    public interface ITransponderPlanRowRepository : IRepository<TransponderPlanRow>
    {
        /// <summary>
        /// Initializes a new <see cref="TransponderPlanRow"/> instance.
        /// </summary>
        /// <returns>A new <see cref="TransponderPlanRow"/> instance.</returns>
        TransponderPlanRow Initialize();
    }
}
