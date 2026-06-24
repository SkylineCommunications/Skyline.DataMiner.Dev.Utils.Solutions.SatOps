namespace Skyline.DataMiner.SDM.SatOps.Common.API.Repositories
{
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
