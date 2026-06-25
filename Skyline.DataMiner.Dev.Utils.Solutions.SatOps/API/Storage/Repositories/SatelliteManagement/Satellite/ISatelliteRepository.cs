namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.Satellite
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;

    public interface ISatelliteRepository : IRepository<Satellite>
    {
        /// <summary>
        /// Transitions a satellite from Draft to Active.
        /// </summary>
        /// <param name="id">The ID of the satellite to activate.</param>
        /// <returns>The updated <see cref="Satellite"/> after the transition.</returns>
        Satellite Activate(Guid id);

        /// <summary>
        /// Transitions a satellite from Draft to Active.
        /// </summary>
        /// <param name="satellite">The satellite to activate.</param>
        /// <returns>The updated <see cref="Satellite"/> after the transition.</returns>
        Satellite Activate(Satellite satellite);

        /// <summary>
        /// Transitions multiple satellites from Draft to Active.
        /// </summary>
        /// <param name="satellites">The satellites to activate.</param>
        /// <returns>A collection of updated <see cref="Satellite"/> instances after the transition.</returns>
        IReadOnlyCollection<Satellite> Activate(IEnumerable<Satellite> satellites);

        /// <summary>
        /// Transitions multiple satellites from Draft to Active.
        /// </summary>
        /// <param name="satelliteIds">The IDs of the satellites to activate.</param>
        /// <returns>A collection of updated <see cref="Satellite"/> instances after the transition.</returns>
        IReadOnlyCollection<Satellite> Activate(IEnumerable<Guid> satelliteIds);

        /// <summary>
        /// Transitions a satellite from Active to Deprecated.
        /// </summary>
        /// <param name="id">The ID of the satellite to deprecate.</param>
        /// <returns>The updated <see cref="Satellite"/> after the transition.</returns>
        Satellite Deprecate(Guid id);

        /// <summary>
        /// Transitions a satellite from Active to Deprecated.
        /// </summary>
        /// <param name="satellite">The satellite to deprecate.</param>
        /// <returns>The updated <see cref="Satellite"/> after the transition.</returns>
        Satellite Deprecate(Satellite satellite);

        /// <summary>
        /// Transitions multiple satellites from Active to Deprecated.
        /// </summary>
        /// <param name="satellites">The satellites to deprecate.</param>
        /// <returns>A collection of updated <see cref="Satellite"/> instances after the transition.</returns>
        IReadOnlyCollection<Satellite> Deprecate(IEnumerable<Satellite> satellites);

        /// <summary>
        /// Transitions multiple satellites from Active to Deprecated.
        /// </summary>
        /// <param name="satelliteIds">The IDs of the satellites to deprecate.</param>
        /// <returns>A collection of updated <see cref="Satellite"/> instances after the transition.</returns>
        IReadOnlyCollection<Satellite> Deprecate(IEnumerable<Guid> satelliteIds);

        /// <summary>
        /// Transitions a satellite from Deprecated back to Active.
        /// </summary>
        /// <param name="id">The ID of the satellite to reactivate.</param>
        /// <returns>The updated <see cref="Satellite"/> after the transition.</returns>
        Satellite Reactivate(Guid id);

        /// <summary>
        /// Transitions a satellite from Deprecated back to Active.
        /// </summary>
        /// <param name="satellite">The satellite to reactivate.</param>
        /// <returns>The updated <see cref="Satellite"/> after the transition.</returns>
        Satellite Reactivate(Satellite satellite);

        /// <summary>
        /// Transitions multiple satellites from Deprecated back to Active.
        /// </summary>
        /// <param name="satellites">The satellites to reactivate.</param>
        /// <returns>A collection of updated <see cref="Satellite"/> instances after the transition.</returns>
        IReadOnlyCollection<Satellite> Reactivate(IEnumerable<Satellite> satellites);

        /// <summary>
        /// Transitions multiple satellites from Deprecated back to Active.
        /// </summary>
        /// <param name="satelliteIds">The IDs of the satellites to reactivate.</param>
        /// <returns>A collection of updated <see cref="Satellite"/> instances after the transition.</returns>
        IReadOnlyCollection<Satellite> Reactivate(IEnumerable<Guid> satelliteIds);

        /// <summary>
        /// Initializes a new instance of a <see cref="Satellite"/> object.
        /// </summary>
        /// <returns>A new <see cref="Satellite"/> instance.</returns>
        Satellite Initialize();
    }
}
