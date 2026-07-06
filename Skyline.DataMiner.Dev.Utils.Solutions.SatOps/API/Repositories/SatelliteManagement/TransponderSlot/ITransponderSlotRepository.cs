namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories;

    /// <summary>
    /// Represents a repository for managing <see cref="TransponderSlot"/> entities.
    /// </summary>
    public interface ITransponderSlotRepository : IRepository<TransponderSlot>
    {
        /// <summary>
        /// Initializes a new <see cref="TransponderSlot"/> instance.
        /// </summary>
        /// <returns>A new <see cref="TransponderSlot"/> instance.</returns>
        TransponderSlot Initialize();
    }
}
