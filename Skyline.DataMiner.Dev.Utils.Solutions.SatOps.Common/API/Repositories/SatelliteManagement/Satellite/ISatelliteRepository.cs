namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for managing <see cref="Satellite"/> entities.
    /// </summary>
    public interface ISatelliteRepository : IRepository<Satellite>
    {
        /// <summary>
        /// Reads satellites with optional server-side status and hemisphere filters.
        /// </summary>
        /// <param name="status">The lifecycle status, or null to include every status.</param>
        /// <param name="hemisphere">The hemisphere, or null to include both hemispheres.</param>
        /// <param name="filter">An additional API filter, such as a name filter.</param>
        /// <returns>The matching satellites.</returns>
        IEnumerable<Satellite> ReadFiltered(InstanceStatus? status = null, HemisphereType? hemisphere = null, FilterElement<Satellite> filter = null);

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

    }
}
