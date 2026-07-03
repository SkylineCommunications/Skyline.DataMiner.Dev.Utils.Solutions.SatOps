namespace Skyline.DataMiner.SDM.SatOps.Common.API.Storage.DOM.Helpers
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Storage.DOM.Tools;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.Utils.DOM.Extensions;
    using System;
    using System.Collections.Generic;

    internal class SlcSatelliteManagementHelper : DomModuleHelperBase
    {
        public SlcSatelliteManagementHelper(IConnection connection) : base(SlcSatellite_ManagementIds.ModuleId, connection)
        {
        }

        public long CountSatelliteManagementInstances(FilterElement<DomInstance> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return DomHelper.DomInstances.Count(filter);
        }

        /// <summary>
        /// Gets beams matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>An enumerable of <see cref="Beam"/>.</returns>
        public IEnumerable<BeamsInstance> GetBeams(FilterElement<DomInstance> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return GetBeamsIterator(filter);
        }

        internal IEnumerable<IEnumerable<BeamsInstance>> GetBeamsPaged(FilterElement<DomInstance> paramFilter, int pageSize)
        {
            if(paramFilter == null)
            {
                throw new ArgumentNullException(nameof(paramFilter));
            }
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            }

            var pages = DomHelper.DomInstances.ReadPaged(paramFilter, pageSize);
            return InstanceFactory.CreateInstances(pages, instance => new BeamsInstance(instance));
        }

        private IEnumerable<BeamsInstance> GetBeamsIterator(FilterElement<DomInstance> filter)
        {
           return InstanceFactory.ReadAndCreateInstances(DomHelper, filter, instance => new BeamsInstance(instance));
        }
    }
}
