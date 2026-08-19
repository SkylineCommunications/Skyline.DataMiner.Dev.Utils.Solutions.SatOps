namespace Skyline.DataMiner.SDM.SatOps.Automation.Extensions
{
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Extensions;
    using System;

    /// <summary>
    /// Provides extension methods for the Engine class.
    /// </summary>
    public static class EngineExtensions
    {
        /// <summary>
        /// Gets the SatOps API instance for the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance.</param>
        /// <returns>An <see cref="ISatOpsApi"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="engine"/> is <see langword="null"/>.</exception>
        public static ISatOpsApi GetSatOpsApi(this Engine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            return engine.GetUserConnection().GetSatOpsApi();
        }
    }
}
