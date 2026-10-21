namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlanRow
{
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for managing <see cref="TransponderPlanRow"/> entities.
    /// </summary>
    public interface ITransponderPlanRowRepository : IRepository<TransponderPlanRow>
    {
        /// <summary>
        /// Reads the rows belonging to a transponder plan.
        /// </summary>
        /// <param name="transponderPlanId">The plan identifier.</param>
        /// <returns>The plan rows, or an empty sequence if the plan does not exist.</returns>
        IEnumerable<TransponderPlanRow> ReadByTransponderPlan(Guid transponderPlanId);
    }
}
