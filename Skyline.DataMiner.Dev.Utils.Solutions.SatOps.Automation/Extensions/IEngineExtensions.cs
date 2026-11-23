namespace Skyline.DataMiner.Solutions.SatOps.Automation.Extensions
{
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Extensions;
    using System;

    /// <summary>
    /// Provides extension methods for the IEngine interface.
    /// </summary>
    public static class IEngineExtensions
    {
        /// <summary>
        /// Gets the SatOps API instance for the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance.</param>
        /// <returns>An instance of <see cref="ISatOpsApi"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="engine"/> is <c>null</c>.</exception>
        public static ISatOpsApi GetSatOpsApi(this IEngine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            return engine.GetUserConnection().GetSatOpsApi();
        }
    }
}
