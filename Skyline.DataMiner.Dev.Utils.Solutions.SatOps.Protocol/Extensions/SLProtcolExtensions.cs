namespace Skyline.DataMiner.Solutions.SatOps.Protocol.Extensions
{
    using System;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.SDM.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Extensions;

    /// <summary>
    /// Defines extension methods on the <see cref="SLProtcolExtensions"/> class.
    /// </summary>
    public static class SLProtcolExtensions
    {
        /// <summary>
        ///  Retrieves an instance of the <see cref="ISatOpsApi"/> interface.
        /// </summary>
        /// <param name="protocol">The <see cref="SLProtocol"/> instance.</param>
        /// <returns>Instance of the <see cref="ISatOpsApi"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="protocol"/>is <see langword="null"/>.</exception>
        public static ISatOpsApi GetSatOpsApi(this SLProtocol protocol)
        {
            if (protocol == null)
            {
                throw new ArgumentNullException(nameof(protocol));
            }

            return protocol.GetUserConnection().GetSatOpsApi();
        }
    }
}
