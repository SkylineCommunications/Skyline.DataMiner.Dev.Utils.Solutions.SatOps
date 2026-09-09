namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TransponderRangeReservationRepository : Repository, ITransponderRangeReservationRepository
    {
        private readonly TransponderRangeReservationFilterTranslator filterTranslator = new TransponderRangeReservationFilterTranslator();

        public TransponderRangeReservationRepository(SatOpsApi satOpsApi)
            : base(satOpsApi)
        {
        }

        private IJobsRepository Jobs => SatOpsApi.MediaOpsPlan.Jobs;

        public long Count()
        {
            return ReadAllReservationJobs().LongCount();
        }

        public long Count(FilterElement<TransponderRangeReservation> filter)
        {
            if (filter == null || filter.isEmpty())
            {
                return 0;
            }

            return Read(filter).LongCount();
        }

        public long Count(IQuery<TransponderRangeReservation> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return Count(query.Filter);
        }

        public IReadOnlyCollection<TransponderRangeReservation> Create(IEnumerable<TransponderRangeReservation> oToCreate)
        {
            if (oToCreate == null)
            {
                throw new ArgumentNullException(nameof(oToCreate));
            }

            return oToCreate.Select(Create).ToList();
        }

        public TransponderRangeReservation Create(TransponderRangeReservation oToCreate)
        {
            if (oToCreate == null)
            {
                throw new ArgumentNullException(nameof(oToCreate));
            }

            if (oToCreate.Id != Guid.Empty && Read(oToCreate.Id) != null)
            {
                throw new InvalidOperationException(ExceptionMessages.CannotCreateExistingTransponderRangeReservation);
            }

            var job = BuildJob(oToCreate, existingJob: null);
            var createdJob = Jobs.Create(job);

            // Jobs.Create persists the job in the Draft state, which does not reserve any
            // resources. Drive it to Tentative so the MediaOps.Plan scheduling engine reserves
            // the transponder slot (mirrors the old CreateJobAction setting DesiredJobStatus = Tentative).
            var reservedJob = Jobs.SaveAsTentative(createdJob.Id);
            UpsertSlotNamePropertyIfPresent(oToCreate, reservedJob);
            return ReadJobAsReservation(reservedJob);
        }

        public IReadOnlyCollection<TransponderRangeReservation> CreateOrUpdate(IEnumerable<TransponderRangeReservation> oToCreateOrUpdate)
        {
            if (oToCreateOrUpdate == null)
            {
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));
            }

            var results = new List<TransponderRangeReservation>();
            foreach (var reservation in oToCreateOrUpdate)
            {
                var existing = reservation != null && reservation.Id != Guid.Empty ? Read(reservation.Id) : null;
                results.Add(existing == null ? Create(reservation) : Update(reservation));
            }

            return results;
        }

        public void Delete(Guid apiObjectId)
        {
            if (apiObjectId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(apiObjectId));
            }

            var existing = Jobs.Read(apiObjectId);
            if (existing == null || !IsReservationJob(existing, out _))
            {
                return;
            }

            EnsureJobIsMutable(existing);
            Jobs.Delete(apiObjectId);
        }

        public void Delete(IEnumerable<Guid> apiObjectIds)
        {
            if (apiObjectIds == null)
            {
                throw new ArgumentNullException(nameof(apiObjectIds));
            }

            foreach (var id in apiObjectIds)
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(apiObjectIds));
                }

                Delete(id);
            }
        }

        public void Delete(IEnumerable<TransponderRangeReservation> oToDelete)
        {
            if (oToDelete == null)
            {
                throw new ArgumentNullException(nameof(oToDelete));
            }

            foreach (var reservation in oToDelete)
            {
                if (reservation == null)
                {
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainNullItems, nameof(oToDelete));
                }

                Delete(reservation);
            }
        }

        public void Delete(TransponderRangeReservation oToDelete)
        {
            if (oToDelete == null)
            {
                throw new ArgumentNullException(nameof(oToDelete));
            }

            Delete(oToDelete.Id);
        }

        public IEnumerable<TransponderRangeReservation> Read()
        {
            return ReadAllReservationJobs().Select(pair => ToReservation(pair.Job, pair.Node));
        }

        public IEnumerable<TransponderRangeReservation> ReadByTransponder(Guid transponderId)
        {
            if (transponderId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderId));
            }

            var resourceId = TryGetResourceId(transponderId);
            if (!resourceId.HasValue)
            {
                return Enumerable.Empty<TransponderRangeReservation>();
            }

            return ReadAllReservationJobs()
                .Where(pair => pair.Node.ResourceId == resourceId.Value)
                .Select(pair => ToReservation(pair.Job, pair.Node));
        }

        public IEnumerable<TransponderRangeReservation> ReadByTimeWindow(DateTime startTimeUtc, DateTime endTimeUtc)
        {
            ValidateTimeWindow(startTimeUtc, endTimeUtc);

            return ReadAllReservationJobs()
                .Where(pair => OverlapsWindow(pair.Job, startTimeUtc, endTimeUtc))
                .Select(pair => ToReservation(pair.Job, pair.Node));
        }

        public IEnumerable<TransponderRangeReservation> ReadByTransponderAndTimeWindow(Guid transponderId, DateTime startTimeUtc, DateTime endTimeUtc)
        {
            if (transponderId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderId));
            }

            ValidateTimeWindow(startTimeUtc, endTimeUtc);

            var resourceId = TryGetResourceId(transponderId);
            if (!resourceId.HasValue)
            {
                return Enumerable.Empty<TransponderRangeReservation>();
            }

            return ReadAllReservationJobs()
                .Where(pair => pair.Node.ResourceId == resourceId.Value && OverlapsWindow(pair.Job, startTimeUtc, endTimeUtc))
                .Select(pair => ToReservation(pair.Job, pair.Node));
        }

        public TransponderRangeReservation Update(TransponderRangeReservation oToUpdate)
        {
            if (oToUpdate == null)
            {
                throw new ArgumentNullException(nameof(oToUpdate));
            }

            Job existing;
            try
            {
                existing = Jobs.Read(oToUpdate.Id);
            }
            catch
            {
                existing = null;
            }

            if (existing == null || !IsReservationJob(existing, out _))
            {
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderRangeReservation);
            }

            EnsureJobIsMutable(existing);

            var updatedJob = BuildJob(oToUpdate, existing);
            var persistedJob = Jobs.Update(updatedJob);

            // Editing a reservation can change the transponder, which swaps the JobResourceNode for a new
            // one with a new id. Re-anchor the slot-name property to the current node so it is not orphaned
            // on the removed node (no-op when the node did not change or when no slot name is stored).
            UpsertSlotNamePropertyIfPresent(oToUpdate, persistedJob);
            RefreshSlotNameNodeLink(oToUpdate.Id);

            return ReadJobAsReservation(persistedJob);
        }

        public IReadOnlyCollection<TransponderRangeReservation> Update(IEnumerable<TransponderRangeReservation> oToUpdate)
        {
            if (oToUpdate == null)
            {
                throw new ArgumentNullException(nameof(oToUpdate));
            }

            return oToUpdate.Select(Update).ToList();
        }

        public IEnumerable<TransponderRangeReservation> Read(FilterElement<TransponderRangeReservation> filter)
        {
            if (filter == null || filter.isEmpty())
            {
                return Enumerable.Empty<TransponderRangeReservation>();
            }

            var jobFilter = filterTranslator.Translate(filter);
            var matches = Jobs.Read(jobFilter).ToList();
            var reservations = new List<TransponderRangeReservation>();
            foreach (var job in matches)
            {
                if (IsReservationJob(job, out var node))
                {
                    reservations.Add(ToReservation(job, node));
                }
            }

            return filterTranslator.ApplyClientSide(filter, reservations);
        }

        public IEnumerable<TransponderRangeReservation> Read(IQuery<TransponderRangeReservation> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return Read(query.Filter);
        }

        public TransponderRangeReservation Read(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(id));
            }

            Job job;
            try
            {
                job = Jobs.Read(id);
            }
            catch
            {
                return null;
            }

            if (job == null || !IsReservationJob(job, out var node))
            {
                return null;
            }

            return ToReservation(job, node);
        }

        public IEnumerable<TransponderRangeReservation> Read(IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            foreach (var id in ids)
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException(ExceptionMessages.CollectionCannotContainEmptyGuidValues, nameof(ids));
                }
            }

            var idSet = new HashSet<Guid>(ids);
            return ReadAllReservationJobs()
                .Where(pair => idSet.Contains(pair.Job.Id))
                .Select(pair => ToReservation(pair.Job, pair.Node));
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged()
        {
            return ReadPaged(new TRUEFilterElement<TransponderRangeReservation>());
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(int pageSize)
        {
            return ReadPaged(new TRUEFilterElement<TransponderRangeReservation>(), pageSize);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(FilterElement<TransponderRangeReservation> filter)
        {
            return ReadPaged(filter, 100);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(IQuery<TransponderRangeReservation> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return ReadPaged(query.Filter);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(FilterElement<TransponderRangeReservation> filter, int pageSize)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "The page size must be greater than zero.");
            }

            return ReadPagedIterator(filter, pageSize);
        }

        public IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPaged(IQuery<TransponderRangeReservation> query, int pageSize)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return ReadPaged(query.Filter, pageSize);
        }

        private IEnumerable<IPagedResult<TransponderRangeReservation>> ReadPagedIterator(FilterElement<TransponderRangeReservation> filter, int pageSize)
        {
            var results = filter is TRUEFilterElement<TransponderRangeReservation> ? Read().ToList() : Read(filter).ToList();
            var pageNumber = 0;
            for (var offset = 0; offset < results.Count; offset += pageSize)
            {
                var slice = results.Skip(offset).Take(pageSize).ToList();
                var hasNext = offset + pageSize < results.Count;
                yield return new SDM.PagedResult<TransponderRangeReservation>(slice, pageNumber++, pageSize, hasNext);
            }
        }

        private static void ValidateTimeWindow(DateTime startTimeUtc, DateTime endTimeUtc)
        {
            if (startTimeUtc >= endTimeUtc)
            {
                throw new ArgumentException("The start time must be earlier than the end time.");
            }
        }

        private static bool OverlapsWindow(Job job, DateTime startTimeUtc, DateTime endTimeUtc)
        {
            var jobStart = job.Start.UtcDateTime;
            var jobEnd = job.End.UtcDateTime;
            return jobStart < endTimeUtc && jobEnd > startTimeUtc;
        }

        private static bool IsReservationJob(Job job, out JobResourceNode resourceNode)
        {
            resourceNode = null;
            if (job?.NodeGraph == null)
            {
                return false;
            }

            JobResourceNode transponderNode = null;
            foreach (var graphNode in job.NodeGraph.Nodes)
            {
                if (!graphNode.IsResourceNode(out var candidate))
                {
                    continue;
                }

                if (candidate.ResourcePoolId != PredefinedGuids.TransponderResourcePoolGuid)
                {
                    continue;
                }

                if (transponderNode != null)
                {
                    // More than one transponder node on the same job is not a valid range reservation shape.
                    return false;
                }

                transponderNode = candidate;
            }

            if (transponderNode == null)
            {
                return false;
            }

            resourceNode = transponderNode;
            return true;
        }

        private IEnumerable<(Job Job, JobResourceNode Node)> ReadAllReservationJobs()
        {
            foreach (var job in Jobs.Read())
            {
                if (IsReservationJob(job, out var node))
                {
                    yield return (job, node);
                }
            }
        }

        private TransponderRangeReservation ToReservation(Job job, JobResourceNode node)
        {
            var reservation = TransponderRangeReservation.CreateWithId(job.Id);
            reservation.Name = job.Name;
            reservation.StartTime = job.Start.UtcDateTime;
            reservation.EndTime = job.End.UtcDateTime;
            reservation.NodeId = node.Id;
            reservation.SlotName = GetSlotName(job.Id);

            reservation.Transponder = ResolveTransponderIdFromResource(node.ResourceId);

            var rangeCapacity = node.OrchestrationSettings.Capacities
                .OfType<RangeCapacitySetting>()
                .FirstOrDefault(c => c.Id == PredefinedGuids.TransponderBandwidthGuid);
            if (rangeCapacity != null)
            {
                reservation.RelativeStartFrequency = rangeCapacity.MinValue.HasValue ? (double?)(double)rangeCapacity.MinValue.Value : null;
                reservation.RelativeEndFrequency = rangeCapacity.MaxValue.HasValue ? (double?)(double)rangeCapacity.MaxValue.Value : null;
            }

            var satelliteCapability = node.OrchestrationSettings.Capabilities
                .FirstOrDefault(c => c.Id == PredefinedGuids.SatelliteCapabilityGuid);
            if (satelliteCapability != null)
            {
                reservation.SatelliteName = satelliteCapability.Value;
            }

            return reservation;
        }

        private TransponderRangeReservation ReadJobAsReservation(Job job)
        {
            if (job == null || !IsReservationJob(job, out var node))
            {
                return null;
            }

            return ToReservation(job, node);
        }

        private Guid? ResolveTransponderIdFromResource(Guid resourceId)
        {
            if (resourceId == Guid.Empty)
            {
                return null;
            }

            var match = SatOpsApi.Transponders
                .Read(TransponderExposers.TransponderDOMResource.Equal(resourceId))
                .FirstOrDefault();

            return match?.Id;
        }

        private Guid? TryGetResourceId(Guid transponderId)
        {
            var transponder = SatOpsApi.Transponders.Read(transponderId);
            return transponder?.DOMResource;
        }

        /// <summary>
        /// Copies <see cref="TransponderRangeReservation.StartTime"/>/<see cref="TransponderRangeReservation.EndTime"/>
        /// onto the given job. If <see cref="TransponderRangeReservation.PreRollStart"/> or
        /// <see cref="TransponderRangeReservation.PostRollEnd"/> are set, those values override; otherwise a
        /// zero-length pre-/post-roll is written to satisfy MediaOps.Plan validation
        /// (<see cref="MediaOps.Plan.Exceptions.JobInvalidPreRollError"/> /
        /// <see cref="MediaOps.Plan.Exceptions.JobInvalidPostRollError"/>).
        /// </summary>
        internal static void ApplyReservationTimes(TransponderRangeReservation reservation, Job job)
        {
            if (reservation == null) throw new ArgumentNullException(nameof(reservation));
            if (job == null) throw new ArgumentNullException(nameof(job));

            if (reservation.StartTime.HasValue)
            {
                var startUtc = new DateTimeOffset(DateTime.SpecifyKind(reservation.StartTime.Value, DateTimeKind.Utc));
                job.Start = startUtc;
                job.PreRollStart = reservation.PreRollStart.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(reservation.PreRollStart.Value, DateTimeKind.Utc))
                    : startUtc;
            }

            if (reservation.EndTime.HasValue)
            {
                var endUtc = new DateTimeOffset(DateTime.SpecifyKind(reservation.EndTime.Value, DateTimeKind.Utc));
                job.End = endUtc;
                job.PostRollEnd = reservation.PostRollEnd.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(reservation.PostRollEnd.Value, DateTimeKind.Utc))
                    : endUtc;
            }
        }

        private Job BuildJob(TransponderRangeReservation reservation, Job existingJob)
        {
            if (!reservation.Transponder.HasValue || reservation.Transponder.Value == Guid.Empty)
            {
                throw new ArgumentException("Transponder is required to build a reservation job.", nameof(reservation));
            }

            var transponderId = reservation.Transponder.Value;
            var transponder = SatOpsApi.Transponders.Read(transponderId)
                ?? throw new ArgumentException($"Transponder '{transponderId}' could not be found.", nameof(reservation));

            if (transponder.DOMResource == null || transponder.DOMResource == Guid.Empty)
            {
                throw new InvalidOperationException($"Transponder '{transponderId}' has no associated MediaOps.Plan resource.");
            }

            var resourceId = transponder.DOMResource.Value;
            var satelliteName = ResolveSatelliteNameFromTransponder(transponder, id => SatOpsApi.Satellites.Read(id));

            reservation.SatelliteName = satelliteName;

            var job = existingJob ?? (reservation.Id != Guid.Empty ? new Job(reservation.Id) : new Job());

            job.Name = reservation.Name;
            ApplyReservationTimes(reservation, job);

            var node = job.NodeGraph.Nodes.OfType<JobResourceNode>()
                .FirstOrDefault(n => n.ResourcePoolId == PredefinedGuids.TransponderResourcePoolGuid);

            if (node == null)
            {
                node = new JobResourceNode(PredefinedGuids.TransponderResourcePoolGuid, resourceId);
                job.NodeGraph.Add(node);
            }
            else if (node.ResourceId != resourceId)
            {
                var replacement = new JobResourceNode(PredefinedGuids.TransponderResourcePoolGuid, resourceId);
                job.NodeGraph.Swap(node, replacement);
                node = replacement;
            }

            ApplyBandwidthRange(node, reservation.RelativeStartFrequency, reservation.RelativeEndFrequency);
            ApplyBandwidthSize(node, reservation.BandwidthSize);
            ApplySatelliteCapability(node, satelliteName);

            return job;
        }

        /// <summary>
        /// Resolves the satellite name for a reservation from the transponder's
        /// <see cref="Transponder.TransponderSatellite"/> relationship.
        /// The transponder is the single source of truth for the satellite, so callers cannot supply
        /// a mismatched or stale <see cref="TransponderRangeReservation.SatelliteName"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when the transponder has no linked satellite, when the linked satellite cannot be
        /// resolved, or when the satellite has no name.
        /// </exception>
        internal static string ResolveSatelliteNameFromTransponder(
            Transponder transponder,
            Func<Guid, Satellite> lookupSatellite)
        {
            if (transponder == null) throw new ArgumentNullException(nameof(transponder));
            if (lookupSatellite == null) throw new ArgumentNullException(nameof(lookupSatellite));

            var satelliteId = transponder.TransponderSatellite;
            if (!satelliteId.HasValue || satelliteId.Value == Guid.Empty)
            {
                throw new ArgumentException(
                    $"Transponder '{transponder.Id}' has no linked satellite (TransponderSatellite is missing). " +
                    "A reservation cannot be created without a satellite relationship.",
                    nameof(transponder));
            }

            var satellite = lookupSatellite(satelliteId.Value)
                ?? throw new ArgumentException(
                    $"Satellite '{satelliteId.Value}' referenced by transponder '{transponder.Id}' could not be resolved.",
                    nameof(transponder));

            if (string.IsNullOrWhiteSpace(satellite.Name))
            {
                throw new ArgumentException(
                    $"Satellite '{satelliteId.Value}' referenced by transponder '{transponder.Id}' has no name.",
                    nameof(transponder));
            }

            return satellite.Name;
        }

        private static void ApplyBandwidthRange(JobResourceNode node, double? minValue, double? maxValue)
        {
            var existing = node.OrchestrationSettings.Capacities
                .OfType<RangeCapacitySetting>()
                .FirstOrDefault(c => c.Id == PredefinedGuids.TransponderBandwidthGuid);

            if (existing != null)
            {
                node.OrchestrationSettings.RemoveCapacity(existing);
            }

            var setting = new RangeCapacitySetting(PredefinedGuids.TransponderBandwidthGuid)
            {
                MinValue = minValue.HasValue ? (decimal?)(decimal)minValue.Value : null,
                MaxValue = maxValue.HasValue ? (decimal?)(decimal)maxValue.Value : null,
            };
            node.OrchestrationSettings.AddCapacity(setting);
        }

        private static void ApplyBandwidthSize(JobResourceNode node, double? size)
        {
            var existing = node.OrchestrationSettings.Configurations
                .OfType<NumberConfigurationSetting>()
                .FirstOrDefault(c => c.Id == PredefinedGuids.BandwidthSizeGuid);

            if (existing != null)
            {
                node.OrchestrationSettings.RemoveConfiguration(existing);
            }

            if (!size.HasValue)
            {
                return;
            }

            node.OrchestrationSettings.AddConfiguration(new NumberConfigurationSetting(PredefinedGuids.BandwidthSizeGuid)
            {
                Value = (decimal)size.Value,
            });
        }

        private static void ApplySatelliteCapability(JobResourceNode node, string satelliteName)
        {
            var existing = node.OrchestrationSettings.Capabilities
                .FirstOrDefault(c => c.Id == PredefinedGuids.SatelliteCapabilityGuid);

            if (existing != null)
            {
                node.OrchestrationSettings.RemoveCapability(existing);
            }

            if (string.IsNullOrWhiteSpace(satelliteName))
            {
                return;
            }

            node.OrchestrationSettings.AddCapability(new CapabilitySetting(PredefinedGuids.SatelliteCapabilityGuid)
            {
                Value = satelliteName,
            });
        }

        private static void EnsureJobIsMutable(Job job)
        {
            if (job.State == JobState.Running || job.State == JobState.Completed)
            {
                throw new InvalidOperationException(
                    $"Reservation '{job.Id}' cannot be modified because its underlying job is in state '{job.State}'.");
            }
        }

        private void UpsertSlotNamePropertyIfPresent(TransponderRangeReservation reservation, Job job)
        {
            if (reservation == null) throw new ArgumentNullException(nameof(reservation));
            if (job == null) throw new ArgumentNullException(nameof(job));

            if (String.IsNullOrWhiteSpace(reservation.SlotName) || !IsReservationJob(job, out var node))
            {
                return;
            }

            AddSlotNameProperty(job.Id, node.Id, reservation.SlotName);
        }

        public void ReserveRange(Guid reservationId, double startFrequency, double endFrequency)
        {
            ReserveRangeInternal(reservationId, startFrequency, endFrequency, satelliteName: null, applySatellite: false);
        }

        public void ReserveRange(Guid reservationId, double startFrequency, double endFrequency, string satelliteName)
        {
            ReserveRangeInternal(reservationId, startFrequency, endFrequency, satelliteName, applySatellite: true);
        }

        private void ReserveRangeInternal(Guid reservationId, double startFrequency, double endFrequency, string satelliteName, bool applySatellite)
        {
            if (reservationId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(reservationId));
            }

            var job = Jobs.Read(reservationId)
                ?? throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderRangeReservation);

            if (!IsReservationJob(job, out var node))
            {
                throw new InvalidOperationException(ExceptionMessages.ReservationIsNotValidTransponderRangeReservation);
            }

            EnsureJobIsMutable(job);

            ApplyBandwidthRange(node, startFrequency, endFrequency);
            ApplyBandwidthSize(node, endFrequency - startFrequency);

            if (applySatellite)
            {
                ApplySatelliteCapability(node, satelliteName);
            }

            Jobs.Update(job);

            // A Draft job does not reserve resources. If the reservation job has not been driven
            // to Tentative yet (e.g. it was created outside the fixed Create path), transition it
            // now so the applied range actually reserves the transponder slot.
            if (job.State == JobState.Draft)
            {
                Jobs.SaveAsTentative(job.Id);
            }
        }

        public string GetFirstNodeId(Guid reservationId)
        {
            if (reservationId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(reservationId));
            }

            var job = Jobs.Read(reservationId)
                ?? throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderRangeReservation);

            if (!IsReservationJob(job, out var node))
            {
                throw new InvalidOperationException(ExceptionMessages.CannotUpdateNonExistingTransponderRangeReservation);
            }

            return node.Id;
        }

        private void AddSlotNameProperty(Guid reservationId, string nodeId, string slotName)
        {
            if (reservationId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(reservationId));
            }

            if (string.IsNullOrWhiteSpace(nodeId))
            {
                throw new ArgumentException("Node id cannot be null or white space.", nameof(nodeId));
            }

            var propertiesHelper = new DomHelper(
                SatOpsApi.Connection.HandleMessages,
                SlcPropertiesIds.ModuleId);

            var reservationIdString = reservationId.ToString();

            var propertyInfoInstance = propertiesHelper.DomInstances
                .Read(DomInstanceExposers.DomDefinitionId
                    .Equal(SlcPropertiesIds.Definitions.Property.Id))
                .Select(di => new PropertyInstance(di))
                .FirstOrDefault(pi => pi.PropertyInfo.Name == NamingConstants.PropertyInfoName);

            var propertyValueInstance = FindSlotNameProperty(propertiesHelper, reservationIdString);

            if (propertyValueInstance != null)
            {
                var existingEntry = propertyValueInstance.PropertyValue
                    .FirstOrDefault(pv => pv.PropertyName == NamingConstants.PropertyInfoName);

                if (existingEntry != null)
                {
                    existingEntry.Value = slotName;

                    // Keep the property anchored to the reservation's current transponder node. A resource
                    // swap replaces the JobResourceNode with a new node id, so a pre-existing slot-name
                    // property still references the old, now-removed node via SubID. MediaOps scheduling
                    // reads this property per node (LinkedObjectID + SubID), so a stale SubID detaches the
                    // slot name from the swapped-in node. Refresh it so the name follows the node.
                    propertyValueInstance.PropertyValueInfo.SubID = nodeId;
                    propertyValueInstance.Save(propertiesHelper);
                    return;
                }
            }

            var newInstance = new PropertyValuesInstance
            {
                PropertyValueInfo =
                {
                    LinkedObjectID = reservationIdString,
                    Scope = NamingConstants.PropertyInfoScope,
                    SubID = nodeId,
                },
            };

            newInstance.PropertyValue.Add(new PropertyValueSection
            {
                PropertyName = NamingConstants.PropertyInfoName,
                Value = slotName,
                PropertyID = propertyInfoInstance?.ID.Id ?? Guid.Empty,
            });

            newInstance.Save(propertiesHelper);
        }

        public string GetSlotName(Guid reservationId)
        {
            if (reservationId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(reservationId));
            }

            var propertiesHelper = new DomHelper(
                SatOpsApi.Connection.HandleMessages,
                SlcPropertiesIds.ModuleId);

            var propertyValueInstance = FindSlotNameProperty(propertiesHelper, reservationId.ToString());

            return propertyValueInstance?.PropertyValue
                .FirstOrDefault(pv => pv.PropertyName == NamingConstants.PropertyInfoName)?.Value;
        }

        public void RefreshSlotNameNodeLink(Guid reservationId)
        {
            if (reservationId == Guid.Empty)
            {
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(reservationId));
            }

            var propertiesHelper = new DomHelper(
                SatOpsApi.Connection.HandleMessages,
                SlcPropertiesIds.ModuleId);

            var propertyValueInstance = FindSlotNameProperty(propertiesHelper, reservationId.ToString());
            if (propertyValueInstance == null)
            {
                // No slot name stored yet: nothing to re-anchor. A later AddSlotNameProperty will create
                // the property against the current node.
                return;
            }

            var nodeId = GetFirstNodeId(reservationId);
            if (string.Equals(propertyValueInstance.PropertyValueInfo.SubID, nodeId, StringComparison.Ordinal))
            {
                return;
            }

            propertyValueInstance.PropertyValueInfo.SubID = nodeId;
            propertyValueInstance.Save(propertiesHelper);
        }

        private static PropertyValuesInstance FindSlotNameProperty(DomHelper propertiesHelper, string reservationIdString)
        {
            var matches = propertiesHelper.DomInstances
                .Read(DomInstanceExposers.DomDefinitionId
                    .Equal(SlcPropertiesIds.Definitions.PropertyValues.Id))
                .Select(di => new PropertyValuesInstance(di))
                .Where(pvi => pvi.PropertyValueInfo.LinkedObjectID == reservationIdString
                    && pvi.PropertyValue.Any(pv => pv.PropertyName == NamingConstants.PropertyInfoName))
                .ToList();

            if (matches.Count <= 1)
            {
                return matches.FirstOrDefault();
            }

            var canonical = matches.FirstOrDefault(pvi => pvi.PropertyValue
                    .Any(pv => pv.PropertyName == NamingConstants.PropertyInfoName
                        && !string.IsNullOrWhiteSpace(pv.Value)))
                ?? matches[0];

            foreach (var duplicate in matches.Where(m => !ReferenceEquals(m, canonical)))
            {
                duplicate.Delete(propertiesHelper);
            }

            return canonical;
        }
    }
}
