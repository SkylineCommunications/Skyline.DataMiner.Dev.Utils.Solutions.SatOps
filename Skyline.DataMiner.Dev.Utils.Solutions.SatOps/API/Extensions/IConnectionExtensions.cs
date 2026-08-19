using Skyline.DataMiner.SDM.SatOps.Common.API;

namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Extensions
{
    using Skyline.DataMiner.Net;
    using System;

    /// <summary>
    /// Provides extension methods for <see cref="IConnection"/>.
    /// </summary>
    public static class IConnectionExtensions
    {
        /// <summary>
        /// Gets the SatOps API instance for the specified connection.
        /// </summary>
        /// <param name="connection">The connection to extend.</param>
        /// <returns>An <see cref="ISatOpsApi"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="connection"/> is <see langword="null"/>.</exception>
        public static ISatOpsApi GetSatOpsApi(this IConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            return new SatOpsApi(connection);
        }
    }
}
