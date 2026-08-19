namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder
{
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for managing <see cref="Transponder"/> entities.
    /// </summary>
    public interface ITransponderRepository : IRepository<Transponder>
    {
        /// <summary>
        /// Reads all transponders that belong to the specified satellite.
        /// </summary>
        /// <param name="satelliteId">The unique identifier of the satellite.</param>
        /// <returns>An enumerable of <see cref="Transponder"/> instances for the satellite.</returns>
        IEnumerable<Transponder> ReadBySatellite(Guid satelliteId);

        /// <summary>
        /// Activates a transponder by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transponder to activate.</param>
        /// <returns>The activated <see cref="Transponder"/>.</returns>
        Transponder Activate(Guid id);

        /// <summary>
        /// Activates the specified transponder.
        /// </summary>
        /// <param name="transponder">The transponder to activate.</param>
        /// <returns>The activated <see cref="Transponder"/>.</returns>
        Transponder Activate(Transponder transponder);

        /// <summary>
        /// Activates multiple transponders.
        /// </summary>
        /// <param name="transponders">The collection of transponders to activate.</param>
        /// <returns>A read-only collection of activated transponders.</returns>
        IReadOnlyCollection<Transponder> Activate(IEnumerable<Transponder> transponders);

        /// <summary>
        /// Activates multiple transponders by their unique identifiers.
        /// </summary>
        /// <param name="transponderIds">The collection of transponder identifiers to activate.</param>
        /// <returns>A read-only collection of activated transponders.</returns>
        IReadOnlyCollection<Transponder> Activate(IEnumerable<Guid> transponderIds);

        /// <summary>
        /// Deprecates a transponder by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transponder to deprecate.</param>
        /// <returns>The deprecated <see cref="Transponder"/>.</returns>
        Transponder Deprecate(Guid id);

        /// <summary>
        /// Deprecates the specified transponder.
        /// </summary>
        /// <param name="transponder">The transponder to deprecate.</param>
        /// <returns>The deprecated <see cref="Transponder"/>.</returns>
        Transponder Deprecate(Transponder transponder);

        /// <summary>
        /// Deprecates multiple transponders.
        /// </summary>
        /// <param name="transponders">The collection of transponders to deprecate.</param>
        /// <returns>A read-only collection of deprecated transponders.</returns>
        IReadOnlyCollection<Transponder> Deprecate(IEnumerable<Transponder> transponders);

        /// <summary>
        /// Deprecates multiple transponders by their unique identifiers.
        /// </summary>
        /// <param name="transponderIds">The collection of transponder identifiers to deprecate.</param>
        /// <returns>A read-only collection of deprecated transponders.</returns>
        IReadOnlyCollection<Transponder> Deprecate(IEnumerable<Guid> transponderIds);

        /// <summary>
        /// Reactivates a transponder by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transponder to reactivate.</param>
        /// <returns>The reactivated <see cref="Transponder"/>.</returns>
        Transponder Reactivate(Guid id);

        /// <summary>
        /// Reactivates the specified transponder.
        /// </summary>
        /// <param name="transponder">The transponder to reactivate.</param>
        /// <returns>The reactivated <see cref="Transponder"/>.</returns>
        Transponder Reactivate(Transponder transponder);

        /// <summary>
        /// Reactivates multiple transponders.
        /// </summary>
        /// <param name="transponders">The collection of transponders to reactivate.</param>
        /// <returns>A read-only collection of reactivated transponders.</returns>
        IReadOnlyCollection<Transponder> Reactivate(IEnumerable<Transponder> transponders);

        /// <summary>
        /// Reactivates multiple transponders by their unique identifiers.
        /// </summary>
        /// <param name="transponderIds">The collection of transponder identifiers to reactivate.</param>
        /// <returns>A read-only collection of reactivated transponders.</returns>
        IReadOnlyCollection<Transponder> Reactivate(IEnumerable<Guid> transponderIds);
    }
}
