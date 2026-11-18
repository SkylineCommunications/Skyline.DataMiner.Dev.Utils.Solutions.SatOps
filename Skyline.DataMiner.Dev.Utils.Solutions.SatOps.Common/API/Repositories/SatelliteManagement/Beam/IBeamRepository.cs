namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Beam
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for managing <see cref="Beam"/> entities.
    /// </summary>
    public interface IBeamRepository : IRepository<Beam>
    {
        /// <summary>
        /// Reads beams with optional server-side status, transmission and link filters.
        /// </summary>
        /// <param name="status">The lifecycle status, or null to include every status.</param>
        /// <param name="transmissionType">The transmission type, or null to include all types.</param>
        /// <param name="linkType">The link type, or null to include all types.</param>
        /// <param name="filter">An additional API filter, such as a satellite or name filter.</param>
        /// <returns>The matching beams.</returns>
        IEnumerable<Beam> ReadFiltered(InstanceStatus? status = null, BeamTransmissionType? transmissionType = null, BeamLinkType? linkType = null, FilterElement<Beam> filter = null);

        /// <summary>
        /// Reads the beams belonging to a satellite.
        /// </summary>
        /// <param name="satelliteId">The satellite identifier.</param>
        /// <returns>The matching beams, or an empty sequence if the satellite does not exist.</returns>
        IEnumerable<Beam> ReadBySatellite(Guid satelliteId);

        /// <summary>
        /// Activates a beam by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the beam to activate.</param>
        /// <returns>The activated <see cref="Beam"/>.</returns>
        Beam Activate(Guid id);

        /// <summary>
        /// Activates the specified beam.
        /// </summary>
        /// <param name="beam">The beam to activate.</param>
        /// <returns>The activated <see cref="Beam"/>.</returns>
        Beam Activate(Beam beam);

        /// <summary>
        /// Activates multiple beams.
        /// </summary>
        /// <param name="beams">The collection of beams to activate.</param>
        /// <returns>A read-only collection of activated beams.</returns>
        IReadOnlyCollection<Beam> Activate(IEnumerable<Beam> beams);

        /// <summary>
        /// Activates multiple beams by their unique identifiers.
        /// </summary>
        /// <param name="beamIds">The collection of beam identifiers to activate.</param>
        /// <returns>A read-only collection of activated beams.</returns>
        IReadOnlyCollection<Beam> Activate(IEnumerable<Guid> beamIds);

        /// <summary>
        /// Deprecates a beam by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the beam to deprecate.</param>
        /// <returns>The deprecated <see cref="Beam"/>.</returns>
        Beam Deprecate(Guid id);

        /// <summary>
        /// Deprecates the specified beam.
        /// </summary>
        /// <param name="beam">The beam to deprecate.</param>
        /// <returns>The deprecated <see cref="Beam"/>.</returns>
        Beam Deprecate(Beam beam);

        /// <summary>
        /// Deprecates multiple beams.
        /// </summary>
        /// <param name="beams">The collection of beams to deprecate.</param>
        /// <returns>A read-only collection of deprecated beams.</returns>
        IReadOnlyCollection<Beam> Deprecate(IEnumerable<Beam> beams);

        /// <summary>
        /// Deprecates multiple beams by their unique identifiers.
        /// </summary>
        /// <param name="beamIds">The collection of beam identifiers to deprecate.</param>
        /// <returns>A read-only collection of deprecated beams.</returns>
        IReadOnlyCollection<Beam> Deprecate(IEnumerable<Guid> beamIds);

        /// <summary>
        /// Reactivates a beam by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the beam to reactivate.</param>
        /// <returns>The reactivated <see cref="Beam"/>.</returns>
        Beam Reactivate(Guid id);

        /// <summary>
        /// Reactivates the specified beam.
        /// </summary>
        /// <param name="beam">The beam to reactivate.</param>
        /// <returns>The reactivated <see cref="Beam"/>.</returns>
        Beam Reactivate(Beam beam);

        /// <summary>
        /// Reactivates multiple beams.
        /// </summary>
        /// <param name="beams">The collection of beams to reactivate.</param>
        /// <returns>A read-only collection of reactivated beams.</returns>
        IReadOnlyCollection<Beam> Reactivate(IEnumerable<Beam> beams);

        /// <summary>
        /// Reactivates multiple beams by their unique identifiers.
        /// </summary>
        /// <param name="beamIds">The collection of beam identifiers to reactivate.</param>
        /// <returns>A read-only collection of reactivated beams.</returns>
        IReadOnlyCollection<Beam> Reactivate(IEnumerable<Guid> beamIds);
    }
}
