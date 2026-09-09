namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories
{
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using System;

    internal abstract class Repository
    {
        private readonly SatOpsApi satOpsApi;

        protected Repository(SatOpsApi satOpsApi)
        {
            this.satOpsApi = satOpsApi ?? throw new ArgumentNullException(nameof(satOpsApi));
        }

        /// <summary>
        /// Gets the SatOpsApi instance that is used to access the API.
        /// </summary>
        public SatOpsApi SatOpsApi => satOpsApi;
    }
}
