namespace Skyline.DataMiner.Solutions.SatOps.GQI.Extensions
{
    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Extensions;
    using System;

    /// <summary>
    /// Provides extension methods for the <see cref="GQIDMS"/> class.
    /// </summary>
    public static class GqiDmsExtensions
    {
        /// <summary>
        /// Gets an instance of the SatOps API from the specified GQI DMS connection.
        /// </summary>
        /// <param name="dms">The GQI DMS instance.</param>
        /// <returns>An instance of <see cref="ISatOpsApi"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dms"/> is <see langword="null"/>.</exception>
        public static ISatOpsApi GetSatOpsApi(this GQIDMS dms)
        {
            if (dms == null)
            {
                throw new ArgumentNullException(nameof(dms));
            }
            return dms.GetConnection().GetSatOpsApi();
        }

        /// <summary>
        /// Gets an instance of the SatOps API from the specified GQI DMS connection.
        /// </summary>
        /// <param name="args">The initialization arguments containing the DMS instance.</param>
        /// <returns>An instance of <see cref="ISatOpsApi"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is <see langword="null"/>.</exception>
        public static ISatOpsApi GetSatOpsApi(this OnInitInputArgs args)
        {
            if(args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            return GetSatOpsApi(args.DMS);
        }
    }
}
