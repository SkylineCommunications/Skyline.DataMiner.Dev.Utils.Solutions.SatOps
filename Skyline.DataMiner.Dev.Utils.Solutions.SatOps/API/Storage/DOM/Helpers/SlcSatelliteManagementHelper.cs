namespace Skyline.DataMiner.SDM.SatOps.Common.API.Storage.DOM.Helpers
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlanRow;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Storage.DOM.Tools;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.Utils.DOM.Extensions;
    using System;
    using System.Collections.Generic;

    using ExceptionMessages = Logging.ExceptionMessages;

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

            return InstanceFactory.ReadAndCreateInstances(DomHelper, filter, instance => new BeamsInstance(instance));
        }

        /// <summary>
        /// Gets satellites matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>An enumerable of <see cref="Satellite"/>.</returns>
        public IEnumerable<SatellitesInstance> GetSatellites(FilterElement<DomInstance> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return InstanceFactory.ReadAndCreateInstances(DomHelper, filter, instance => new SatellitesInstance(instance));
        }

        /// <summary>
        /// Gets transponders matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>An enumerable of <see cref="Transponder"/>.</returns>
        public IEnumerable<TranspondersInstance> GetTransponders(FilterElement<DomInstance> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return InstanceFactory.ReadAndCreateInstances(DomHelper, filter, instance => new TranspondersInstance(instance));
        }

        /// <summary>
        /// Gets transponder plans matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>An enumerable of <see cref="TransponderPlan"/>.</returns>
        public IEnumerable<TransponderPlansInstance> GetTransponderPlans(FilterElement<DomInstance> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return InstanceFactory.ReadAndCreateInstances(DomHelper, filter, instance => new TransponderPlansInstance(instance));
        }

        /// <summary>
        /// Gets transponder plan rows matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>An enumerable of <see cref="TransponderPlanRow"/>.</returns>
        public IEnumerable<TransponderPlanRowsInstance> GetTransponderPlanRows(FilterElement<DomInstance> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return InstanceFactory.ReadAndCreateInstances(DomHelper, filter, instance => new TransponderPlanRowsInstance(instance));
        }

        /// <summary>
        /// Gets transponder slots matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>An enumerable of <see cref="TransponderSlot"/>.</returns>
        public IEnumerable<TransponderSlotsInstance> GetTransponderSlots(FilterElement<DomInstance> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return InstanceFactory.ReadAndCreateInstances(DomHelper, filter, instance => new TransponderSlotsInstance(instance));
        }

        internal IEnumerable<IEnumerable<BeamsInstance>> GetBeamsPaged(FilterElement<DomInstance> paramFilter, int pageSize)
        {
            if (paramFilter == null)
            {
                throw new ArgumentNullException(nameof(paramFilter));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), ExceptionMessages.PageSizeNumberException);
            }

            var pages = DomHelper.DomInstances.ReadPaged(paramFilter, pageSize);
            return InstanceFactory.CreateInstances(pages, instance => new BeamsInstance(instance));
        }

        internal IEnumerable<IEnumerable<SatellitesInstance>> GetSatellitesPaged(FilterElement<DomInstance> paramFilter, int pageSize)
        {
            if (paramFilter == null)
            {
                throw new ArgumentNullException(nameof(paramFilter));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), ExceptionMessages.PageSizeNumberException);
            }

            var pages = DomHelper.DomInstances.ReadPaged(paramFilter, pageSize);
            return InstanceFactory.CreateInstances(pages, instance => new SatellitesInstance(instance));
        }

        internal IEnumerable<IEnumerable<TranspondersInstance>> GetTranspondersPaged(FilterElement<DomInstance> paramFilter, int pageSize)
        {
            if (paramFilter == null)
            {
                throw new ArgumentNullException(nameof(paramFilter));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), ExceptionMessages.PageSizeNumberException);
            }

            var pages = DomHelper.DomInstances.ReadPaged(paramFilter, pageSize);
            return InstanceFactory.CreateInstances(pages, instance => new TranspondersInstance(instance));
        }

        internal IEnumerable<IEnumerable<TransponderPlansInstance>> GetTransponderPlansPaged(FilterElement<DomInstance> paramFilter, int pageSize)
        {
            if (paramFilter == null)
            {
                throw new ArgumentNullException(nameof(paramFilter));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), ExceptionMessages.PageSizeNumberException);
            }

            var pages = DomHelper.DomInstances.ReadPaged(paramFilter, pageSize);
            return InstanceFactory.CreateInstances(pages, instance => new TransponderPlansInstance(instance));
        }

        internal IEnumerable<IEnumerable<TransponderPlanRowsInstance>> GetTransponderPlanRowsPaged(FilterElement<DomInstance> paramFilter, int pageSize)
        {
            if (paramFilter == null)
            {
                throw new ArgumentNullException(nameof(paramFilter));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), ExceptionMessages.PageSizeNumberException);
            }

            var pages = DomHelper.DomInstances.ReadPaged(paramFilter, pageSize);
            return InstanceFactory.CreateInstances(pages, instance => new TransponderPlanRowsInstance(instance));
        }

        internal IEnumerable<IEnumerable<TransponderSlotsInstance>> GetTransponderSlotsPaged(FilterElement<DomInstance> paramFilter, int pageSize)
        {
            if (paramFilter == null)
            {
                throw new ArgumentNullException(nameof(paramFilter));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), ExceptionMessages.PageSizeNumberException);
            }

            var pages = DomHelper.DomInstances.ReadPaged(paramFilter, pageSize);
            return InstanceFactory.CreateInstances(pages, instance => new TransponderSlotsInstance(instance));
        }
    }
}
