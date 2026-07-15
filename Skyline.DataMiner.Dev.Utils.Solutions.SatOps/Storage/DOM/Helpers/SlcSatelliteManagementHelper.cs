namespace Skyline.DataMiner.SDM.SatOps.Common.Storage.DOM.Helpers
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
    using Skyline.DataMiner.SDM.SatOps.Common.API.Querying;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.SDM.SatOps.Common.Storage.DOM.Tools;
    using Skyline.DataMiner.Utils.DOM.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        /// <summary>
        /// Gets transponder reservations matching the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>An enumerable of <see cref="TransponderReservationsInstance"/>.</returns>
        public IEnumerable<TransponderReservationsInstance> GetTransponderReservations(FilterElement<DomInstance> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return InstanceFactory.ReadAndCreateInstances(DomHelper, filter, instance => new TransponderReservationsInstance(instance));
        }

        /// <summary>
        /// Gets all transponders that belong to the specified satellite.
        /// </summary>
        /// <param name="satellite">The satellite whose transponders should be retrieved.</param>
        /// <returns>An enumerable of <see cref="DomInstance"/>.</returns>
        public IEnumerable<DomInstance> GetTranspondersFromSatelliteName(SatellitesInstance satellite)
        {
            if (satellite == null)
            {
                throw new ArgumentNullException(nameof(satellite));
            }

            var filter = DomInstanceExposers.FieldValues.DomInstanceField(
                SlcSatellite_ManagementIds.Sections.Transponder.TransponderSatellite)
                .Equal(satellite.ID.Id);

            return DomHelper.DomInstances.Read(filter);
        }

        /// <summary>
        /// Gets transponder plans for the specified transponder.
        /// </summary>
        /// <param name="transponderId">The transponder identifier.</param>
        /// <returns>An enumerable of <see cref="TransponderPlansInstance"/>.</returns>
        public IEnumerable<TransponderPlansInstance> GetTransponderPlansByTransponderId(Guid transponderId)
        {
            if (transponderId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderId));
            }

            var filter = DomInstanceExposers.FieldValues.DomInstanceField(
                SlcSatellite_ManagementIds.Sections.TransponderPlan.Transponder)
                .Contains(transponderId);

            return GetTransponderPlans(filter);
        }

        /// <summary>
        /// Gets a transponder plan by its identifier.
        /// </summary>
        /// <param name="planId">The transponder plan identifier.</param>
        /// <returns>An enumerable of matching <see cref="TransponderPlansInstance"/> instances.</returns>
        public IEnumerable<TransponderPlansInstance> GetTransponderPlanById(Guid planId)
        {
            if (planId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(planId));
            }

            var filter = DomInstanceExposers.Id.Equal(new DomInstanceId(planId) { ModuleId = SlcSatellite_ManagementIds.ModuleId });
            return GetTransponderPlans(filter);
        }

        /// <summary>
        /// Gets transponder slots for the specified transponder plan.
        /// </summary>
        /// <param name="planId">The transponder plan identifier.</param>
        /// <returns>An enumerable of <see cref="TransponderSlotsInstance"/>.</returns>
        public IEnumerable<TransponderSlotsInstance> GetSlotsByTransponderPlanId(Guid planId)
        {
            if (planId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(planId));
            }

            var filter = DomInstanceExposers.FieldValues.DomInstanceField(
                SlcSatellite_ManagementIds.Sections.TransponderSlot.TransponderPlan)
                .Contains(planId);

            return GetTransponderSlots(filter);
        }

        /// <summary>
        /// Gets transponder plans for a satellite by following Satellite -> Transponder -> Plan.
        /// </summary>
        /// <param name="satellite">The satellite to resolve.</param>
        /// <returns>An enumerable of <see cref="TransponderPlansInstance"/>.</returns>
        public IEnumerable<TransponderPlansInstance> GetTransponderPlansByTransponderFilter(SatellitesInstance satellite)
        {
            if (satellite == null)
            {
                throw new ArgumentNullException(nameof(satellite));
            }

            var transponders = GetTranspondersFromSatelliteName(satellite).ToList();
            return GetTransponderPlansByTransponderFilter(transponders);
        }

        /// <summary>
        /// Gets transponder plans for the specified transponders.
        /// </summary>
        /// <param name="transponders">The transponders to resolve.</param>
        /// <returns>An enumerable of <see cref="TransponderPlansInstance"/>.</returns>
        public IEnumerable<TransponderPlansInstance> GetTransponderPlansByTransponderFilter(IEnumerable<DomInstance> transponders)
        {
            if (transponders == null)
            {
                throw new ArgumentNullException(nameof(transponders));
            }

            var transponderList = transponders.ToList();
            if (!transponderList.Any())
            {
                return Enumerable.Empty<TransponderPlansInstance>();
            }

            var filter = DomInstanceFilterHelper.BuildOrFilter(
                SlcSatellite_ManagementIds.Sections.TransponderPlan.Transponder,
                transponderList.Select(transponder => transponder.ID.Id));

            return GetTransponderPlans(filter);
        }

        /// <summary>
        /// Gets transponder slots for a satellite by following Satellite -> Transponder -> Plan -> Slot.
        /// </summary>
        /// <param name="satellite">The satellite to resolve.</param>
        /// <returns>An enumerable of <see cref="TransponderSlotsInstance"/>.</returns>
        public IEnumerable<TransponderSlotsInstance> GetSlotsByTransponderFilter(SatellitesInstance satellite)
        {
            if (satellite == null)
            {
                throw new ArgumentNullException(nameof(satellite));
            }

            var transponders = GetTranspondersFromSatelliteName(satellite).ToList();
            return GetSlotsByTransponderFilter(transponders);
        }

        /// <summary>
        /// Gets transponder slots for the specified transponders.
        /// </summary>
        /// <param name="transponders">The transponders to resolve.</param>
        /// <returns>An enumerable of <see cref="TransponderSlotsInstance"/>.</returns>
        public IEnumerable<TransponderSlotsInstance> GetSlotsByTransponderFilter(IEnumerable<DomInstance> transponders)
        {
            if (transponders == null)
            {
                throw new ArgumentNullException(nameof(transponders));
            }

            var transponderList = transponders.ToList();
            if (!transponderList.Any())
            {
                return Enumerable.Empty<TransponderSlotsInstance>();
            }

            var plansFilter = DomInstanceFilterHelper.BuildOrFilter(
                SlcSatellite_ManagementIds.Sections.TransponderPlan.Transponder,
                transponderList.Select(transponder => transponder.ID.Id));

            var plans = GetTransponderPlans(plansFilter).ToList();
            if (!plans.Any())
            {
                return Enumerable.Empty<TransponderSlotsInstance>();
            }

            var slotsFilter = DomInstanceFilterHelper.BuildOrFilter(
                SlcSatellite_ManagementIds.Sections.TransponderSlot.TransponderPlan,
                plans.Select(plan => plan.ID.Id));

            return GetTransponderSlots(slotsFilter);
        }

        /// <summary>
        /// Builds a flattened and sorted list of plan intervals.
        /// </summary>
        /// <param name="permanentPlans">The permanent plans.</param>
        /// <param name="nonPermanentPlans">The non-permanent plans.</param>
        /// <returns>The ordered interval list.</returns>
        public List<(DateTime? Start, DateTime? End, TransponderPlansInstance Plan, bool IsPermanent)> GetTransponderPlanIntervals(
            List<TransponderPlansInstance> permanentPlans,
            List<TransponderPlansInstance> nonPermanentPlans)
        {
            if (permanentPlans == null)
            {
                throw new ArgumentNullException(nameof(permanentPlans));
            }

            if (nonPermanentPlans == null)
            {
                throw new ArgumentNullException(nameof(nonPermanentPlans));
            }

            var intervals = new List<(DateTime? Start, DateTime? End, TransponderPlansInstance Plan, bool IsPermanent)>();

            foreach (var plan in permanentPlans)
            {
                intervals.Add((plan.TransponderPlan.StartTime, plan.TransponderPlan.EndTime, plan, true));
            }

            foreach (var plan in nonPermanentPlans)
            {
                intervals.Add((plan.TransponderPlan.StartTime, plan.TransponderPlan.EndTime, plan, false));
            }

            return intervals.OrderBy(interval => interval.Start).ToList();
        }

        /// <summary>
        /// Splits overlapping plan intervals into non-overlapping chopped segments.
        /// </summary>
        /// <param name="choppedPlans">The output list.</param>
        /// <param name="intervals">The input intervals.</param>
        /// <param name="timePoints">The partition points.</param>
        public void GetChoppedTransponderPlans(
            List<(TransponderPlansInstance Plan, DateTime Start, DateTime End)> choppedPlans,
            List<(DateTime? Start, DateTime? End, TransponderPlansInstance Plan, bool IsPermanent)> intervals,
            List<DateTime?> timePoints)
        {
            if (choppedPlans == null)
            {
                throw new ArgumentNullException(nameof(choppedPlans));
            }

            if (intervals == null)
            {
                throw new ArgumentNullException(nameof(intervals));
            }

            if (timePoints == null)
            {
                throw new ArgumentNullException(nameof(timePoints));
            }

            for (var index = 0; index < timePoints.Count - 1; index++)
            {
                var intervalStart = timePoints[index];
                var intervalEnd = timePoints[index + 1];

                var active = intervals.Where(interval => interval.Start < intervalEnd && interval.End > intervalStart).ToList();
                if (!active.Any())
                {
                    continue;
                }

                var chosen = active.FirstOrDefault(interval => !interval.IsPermanent);
                if (chosen.Equals(default((DateTime? Start, DateTime? End, TransponderPlansInstance Plan, bool IsPermanent))))
                {
                    chosen = active[0];
                }

                if (intervalStart < intervalEnd)
                {
                    choppedPlans.Add((chosen.Plan, intervalStart.GetValueOrDefault(), intervalEnd.GetValueOrDefault()));
                }
            }
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

        internal IEnumerable<IEnumerable<TransponderReservationsInstance>> GetTransponderReservationsPaged(FilterElement<DomInstance> paramFilter, int pageSize)
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
            return InstanceFactory.CreateInstances(pages, instance => new TransponderReservationsInstance(instance));
        }
    }
}
