namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using Reservation = Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation;

    [TestClass]
    public class TransponderRangeReservationReadTests
    {
        [DataTestMethod]
        [DataRow(false, false, false)]
        [DataRow(true, false, false)]
        [DataRow(true, true, false)]
        [DataRow(false, false, true)]
        public void ReadByTransponder_FiltersBackendResourcesAndPreservesReservationChecks(bool withWindow, bool occupancyOnly, bool allocationContext)
        {
            var resourceId = Guid.NewGuid();
            var transponder = new Transponder { DOMResource = resourceId };
            var spanning = CreateJob(resourceId).Job;
            var start = spanning.Start.UtcDateTime;
            var end = spanning.End.UtcDateTime;
            spanning.Start = spanning.Start.AddHours(-1);
            spanning.End = spanning.End.AddHours(1);
            var later = CreateJob(resourceId).Job;
            later.Start = later.Start.AddHours(2);
            later.End = later.End.AddHours(2);
            var endsAtStart = CreateJob(resourceId).Job;
            endsAtStart.Start = endsAtStart.Start.AddHours(-1);
            endsAtStart.End = new DateTimeOffset(start);
            var startsAtEnd = CreateJob(resourceId).Job;
            startsAtEnd.Start = new DateTimeOffset(end);
            startsAtEnd.End = startsAtEnd.End.AddHours(1);
            var wrongResource = CreateJob(Guid.NewGuid()).Job;
            wrongResource.NodeGraph.Add(new JobResourceNode(Guid.NewGuid(), resourceId));
            var multipleTransponders = CreateJob(resourceId).Job;
            multipleTransponders.NodeGraph.Add(new JobResourceNode(PredefinedGuids.TransponderResourcePoolGuid, Guid.NewGuid()));
            var wrongPool = new Job();
            wrongPool.NodeGraph.Add(new JobResourceNode(Guid.NewGuid(), resourceId));
            var candidates = new[] { spanning, later, endsAtStart, startsAtEnd, wrongResource, multipleTransponders, wrongPool };
            FilterElement<Job> captured = null;
            var jobs = new Mock<IJobsRepository>(MockBehavior.Strict);
            jobs.Setup(value => value.Read(It.IsAny<FilterElement<Job>>()))
                .Callback<FilterElement<Job>>(filter => captured = filter)
                .Returns(candidates);
            var repository = CreateRepository(jobs.Object, transponder, !occupancyOnly);

            var result = (allocationContext
                ? repository.ReadAllocationContextByTransponder(transponder.Id).Select(context => context.Reservation)
                : occupancyOnly
                ? repository.ReadOccupancyByTransponderAndTimeWindow(transponder.Id, start, end)
                : withWindow
                ? repository.ReadByTransponderAndTimeWindow(transponder.Id, start, end)
                : repository.ReadByTransponder(transponder.Id)).ToList();

            var leaves = ReadJobFilterLeaves(captured).ToList();
            var resourceFilter = leaves.Single(leaf => leaf.getFieldName().fieldName == JobExposers.Resources.fieldName);
            Assert.AreEqual(Comparer.Contains, resourceFilter.getComparer());
            Assert.AreEqual(resourceId, resourceFilter.getValue());
            Assert.AreEqual(withWindow ? 3 : 1, leaves.Count);
            if (withWindow)
            {
                var startFilter = leaves.Single(leaf => leaf.getFieldName().fieldName == JobExposers.Start.fieldName);
                var endFilter = leaves.Single(leaf => leaf.getFieldName().fieldName == JobExposers.End.fieldName);
                Assert.AreEqual(Comparer.LT, startFilter.getComparer());
                Assert.AreEqual(new DateTimeOffset(end), startFilter.getValue());
                Assert.AreEqual(Comparer.GT, endFilter.getComparer());
                Assert.AreEqual(new DateTimeOffset(start), endFilter.getValue());
            }

            CollectionAssert.AreEquivalent(
                (withWindow ? new[] { spanning } : new[] { spanning, later, endsAtStart, startsAtEnd }).Select(job => job.Id).ToList(),
                result.Select(reservation => reservation.Id).ToList());
            var spanningReservation = result.Single(reservation => reservation.Id == spanning.Id);
            Assert.AreEqual(spanning.Start.UtcDateTime, spanningReservation.StartTime);
            Assert.AreEqual(spanning.End.UtcDateTime, spanningReservation.EndTime);
            Assert.AreEqual(transponder.Id, spanningReservation.Transponder);
            Assert.AreEqual(spanning.Name, spanningReservation.Name);
            Assert.AreEqual(5d, spanningReservation.RelativeStartFrequency);
            Assert.AreEqual(9d, spanningReservation.RelativeEndFrequency);
            if (occupancyOnly)
            {
                Assert.IsNull(spanningReservation.SlotName);
            }

            jobs.Verify(value => value.Read(It.IsAny<FilterElement<Job>>()), Times.Once);
            jobs.VerifyNoOtherCalls();
        }

        [DataTestMethod]
        [DataRow(false, false, false)]
        [DataRow(true, false, false)]
        [DataRow(true, true, false)]
        [DataRow(false, false, true)]
        public void ReadByTransponder_BackendFailurePropagates(bool withWindow, bool occupancyOnly, bool allocationContext)
        {
            var transponder = new Transponder { DOMResource = Guid.NewGuid() };
            var failure = new InvalidOperationException("Resource query failed");
            var jobs = new Mock<IJobsRepository>(MockBehavior.Strict);
            jobs.Setup(value => value.Read(It.IsAny<FilterElement<Job>>())).Throws(failure);
            var repository = CreateRepository(jobs.Object, transponder);
            var start = DateTime.UtcNow;

            var actual = Assert.ThrowsException<InvalidOperationException>(() => (allocationContext
                ? repository.ReadAllocationContextByTransponder(transponder.Id).Select(context => context.Reservation)
                : occupancyOnly
                ? repository.ReadOccupancyByTransponderAndTimeWindow(transponder.Id, start, start.AddHours(1))
                : withWindow
                ? repository.ReadByTransponderAndTimeWindow(transponder.Id, start, start.AddHours(1))
                : repository.ReadByTransponder(transponder.Id)).ToList());

            Assert.AreSame(failure, actual);
        }

        [DataTestMethod]
        [DataRow(JobState.Tentative)]
        [DataRow(JobState.Confirmed)]
        [DataRow(JobState.Running)]
        [DataRow(JobState.Completed)]
        public void ReadAllocationContext_ReusesJobStateAndCompleteWindowWithoutReadingJobsById(JobState state)
        {
            var transponder = new Transponder { DOMResource = Guid.NewGuid() };
            var first = CreateJob(transponder.DOMResource.Value);
            var second = CreateJob(transponder.DOMResource.Value, "Later booking");
            typeof(Job).GetProperty(nameof(Job.State)).SetValue(first.Job, state);
            typeof(Job).GetProperty(nameof(Job.State)).SetValue(second.Job, state);
            second.Job.Start = second.Job.Start.AddDays(10);
            second.Job.End = second.Job.End.AddDays(10);
            var jobs = new Mock<IJobsRepository>(MockBehavior.Strict);
            jobs.Setup(value => value.Read(It.IsAny<FilterElement<Job>>())).Returns(new[] { first.Job, second.Job });
            var repository = CreateRepository(jobs.Object, transponder);

            var result = repository.ReadAllocationContextByTransponder(transponder.Id).ToList();

            Assert.AreEqual(2, result.Count);
            foreach (var pair in new[] { first, second })
            {
                var context = result.Single(item => item.Reservation.Id == pair.Job.Id);
                Assert.AreEqual(state, context.JobState);
                Assert.AreEqual(pair.Job.Start.UtcDateTime, context.JobStartTimeUtc);
                Assert.AreEqual(pair.Job.End.UtcDateTime, context.JobEndTimeUtc);
                Assert.AreEqual(pair.Node.Id, context.NodeId);
                Assert.AreEqual(pair.Node.ResourceId, context.ResourceId);
                Assert.AreEqual(pair.Node.ResourcePoolId, context.ResourcePoolId);
                Assert.AreEqual(transponder.Id, context.Reservation.Transponder);
                Assert.AreEqual(5d, context.Reservation.RelativeStartFrequency);
                Assert.AreEqual(9d, context.Reservation.RelativeEndFrequency);
            }

            jobs.Verify(value => value.Read(It.IsAny<FilterElement<Job>>()), Times.Once);
            jobs.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void MaterializeAllocationContext_PreservesStoredSlotNamesAlongsideJobMetadata()
        {
            var pair = CreateJob(Guid.NewGuid());
            typeof(Job).GetProperty(nameof(Job.State)).SetValue(pair.Job, JobState.Confirmed);
            var property = CreateProperty(pair.Job.Id, "Stored slot");

            var result = TransponderRangeReservationRepository.MaterializeReservations(
                new[] { pair },
                _ => new[] { property.ToInstance() },
                _ => Guid.NewGuid(),
                _ => Assert.Fail("No duplicate should be deleted."),
                (reservation, job, node) => new TransponderReservationAllocationContext(reservation, job, node)).Single();

            Assert.AreEqual("Stored slot", result.Reservation.SlotName);
            Assert.AreEqual(pair.Job.Id, result.Reservation.Id);
            Assert.AreEqual(JobState.Confirmed, result.JobState);
            Assert.AreEqual(pair.Node.Id, result.NodeId);
        }

        [TestMethod]
        public void MaterializeReservations_BatchesPropertiesAndResolvesSharedResourceOnce()
        {
            var resourceId = Guid.NewGuid();
            var transponderId = Guid.NewGuid();
            var jobs = Enumerable.Range(0, 121).Select(index => CreateJob(resourceId, "Booking " + index)).ToList();
            var properties = jobs.ToDictionary(pair => pair.Job.Id.ToString(), pair => CreateProperty(pair.Job.Id, "B4"));
            var batchSizes = new List<int>();
            var resolutions = 0;

            var reservations = TransponderRangeReservationRepository.MaterializeReservations(
                jobs,
                filter =>
                {
                    var ids = ReadLinkedIds(filter);
                    batchSizes.Add(ids.Count);
                    return ids.Select(id => properties[id].ToInstance()).ToList();
                },
                id =>
                {
                    Assert.AreEqual(resourceId, id);
                    resolutions++;
                    return transponderId;
                },
                _ => Assert.Fail("No duplicate should be deleted.")).ToList();

            CollectionAssert.AreEqual(new[] { 50, 50, 21 }, batchSizes);
            Assert.AreEqual(1, resolutions);
            Assert.AreEqual(jobs.Count, reservations.Count);
            for (var index = 0; index < reservations.Count; index++)
            {
                var reservation = reservations[index];
                Assert.AreEqual(jobs[index].Job.Id, reservation.Id);
                Assert.AreEqual(jobs[index].Job.Name, reservation.Name);
                Assert.AreEqual(jobs[index].Node.Id, reservation.NodeId);
                Assert.AreEqual(jobs[index].Job.Start.UtcDateTime, reservation.StartTime);
                Assert.AreEqual(jobs[index].Job.End.UtcDateTime, reservation.EndTime);
                Assert.AreEqual(transponderId, reservation.Transponder);
                Assert.AreEqual("B4", reservation.SlotName);
                Assert.AreEqual(5d, reservation.RelativeStartFrequency);
                Assert.AreEqual(9d, reservation.RelativeEndFrequency);
                Assert.AreEqual("SES-10", reservation.SatelliteName);
            }
        }

        [TestMethod]
        public void MaterializeReservations_Empty_DoesNotReadOrResolve()
        {
            var result = TransponderRangeReservationRepository.MaterializeReservations(
                Array.Empty<(Job Job, JobResourceNode Node)>(),
                _ => throw new AssertFailedException("Properties must not be read."),
                _ => throw new AssertFailedException("Resources must not be resolved."),
                _ => Assert.Fail("Properties must not be deleted.")).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void MaterializeReservations_MissingPropertiesAndResourceMappingsRemainNull()
        {
            var firstResource = Guid.NewGuid();
            var secondResource = Guid.NewGuid();
            var jobs = new[] { CreateJob(firstResource), CreateJob(firstResource), CreateJob(secondResource) };
            var resolved = new List<Guid>();

            var result = TransponderRangeReservationRepository.MaterializeReservations(
                jobs,
                _ => Array.Empty<DomInstance>(),
                resource =>
                {
                    resolved.Add(resource);
                    return null;
                },
                _ => Assert.Fail("No duplicate should be deleted.")).ToList();

            CollectionAssert.AreEqual(new[] { firstResource, secondResource }, resolved);
            Assert.IsTrue(result.All(reservation => reservation.Transponder == null && reservation.SlotName == null));
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void MaterializeReservations_DuplicatesKeepExistingCanonicalSelectionAndCleanup(bool allEmpty)
        {
            var job = CreateJob(Guid.NewGuid());
            var first = CreateProperty(job.Job.Id, "");
            var second = CreateProperty(job.Job.Id, allEmpty ? " " : "C4");
            var third = CreateProperty(job.Job.Id, allEmpty ? null : "D4");
            var unrelated = CreateProperty(Guid.NewGuid(), "Unrelated");
            var otherProperty = CreateProperty(job.Job.Id, "Other", "Other property");
            var deleted = new List<Guid>();
            var expected = allEmpty ? first : second;

            var result = TransponderRangeReservationRepository.MaterializeReservations(
                new[] { job },
                _ => new[] { first, second, third, unrelated, otherProperty }.Select(value => value.ToInstance()),
                _ => Guid.NewGuid(),
                duplicate => deleted.Add(duplicate.ID.Id)).Single();

            Assert.AreEqual(allEmpty ? "" : "C4", result.SlotName);
            CollectionAssert.AreEquivalent(
                new[] { first, second, third }.Where(value => value != expected).Select(value => value.ID.Id).ToList(),
                deleted);
        }

        [TestMethod]
        public void MaterializeReservations_ReenumerationUsesFreshData()
        {
            var job = CreateJob(Guid.NewGuid());
            var transponderId = Guid.NewGuid();
            var property = CreateProperty(job.Job.Id, "A4");
            var reads = 0;
            var resolutions = 0;
            var query = TransponderRangeReservationRepository.MaterializeReservations(
                new[] { job },
                _ =>
                {
                    reads++;
                    return new[] { property.ToInstance() };
                },
                _ =>
                {
                    resolutions++;
                    return transponderId;
                },
                _ => Assert.Fail("No duplicate should be deleted."));

            Assert.AreEqual("A4", query.Single().SlotName);
            property.PropertyValue[0].Value = "B4";
            transponderId = Guid.NewGuid();
            var updated = query.Single();

            Assert.AreEqual("B4", updated.SlotName);
            Assert.AreEqual(transponderId, updated.Transponder);
            Assert.AreEqual(2, reads);
            Assert.AreEqual(2, resolutions);
        }

        [DataTestMethod]
        [DataRow("Read")]
        [DataRow("Resolve")]
        [DataRow("Delete")]
        public void MaterializeReservations_DependencyFailuresPropagate(string stage)
        {
            var job = CreateJob(Guid.NewGuid());
            var failure = new InvalidOperationException(stage + " failed");
            var properties = new[] { CreateProperty(job.Job.Id, "A4"), CreateProperty(job.Job.Id, "B4") };

            var actual = Assert.ThrowsException<InvalidOperationException>(() =>
                TransponderRangeReservationRepository.MaterializeReservations(
                    new[] { job },
                    _ => stage == "Read" ? throw failure : properties.Select(value => value.ToInstance()),
                    _ => stage == "Resolve" ? throw failure : (Guid?)Guid.NewGuid(),
                    _ =>
                    {
                        if (stage == "Delete")
                            throw failure;
                    }).ToList());

            Assert.AreSame(failure, actual);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void OccupancyRead_OverlapValidationNeverReadsPropertiesOrReverseMappings(bool overlaps)
        {
            var transponder = new Transponder { DOMResource = Guid.NewGuid() };
            var existing = CreateJob(transponder.DOMResource.Value).Job;
            var jobs = new Mock<IJobsRepository>(MockBehavior.Strict);
            jobs.Setup(value => value.Read(It.IsAny<FilterElement<Job>>())).Returns(new[] { existing });
            var repository = CreateRepository(jobs.Object, transponder, false);
            var middleware = new TransponderRangeReservationOverlapValidationMiddleware(
                repository.ReadOccupancyByTransponderAndTimeWindow);
            var incoming = new Reservation
            {
                Name = "Incoming",
                Transponder = transponder.Id,
                StartTime = existing.Start.UtcDateTime,
                EndTime = existing.End.UtcDateTime,
                RelativeStartFrequency = overlaps ? 6d : 9d,
                RelativeEndFrequency = overlaps ? 8d : 13d,
            };
            var called = false;
            Func<Reservation, Reservation> next = reservation =>
            {
                called = true;
                return reservation;
            };

            if (overlaps)
            {
                var exception = Assert.ThrowsException<ArgumentException>(() => middleware.OnCreate(incoming, next));
                StringAssert.Contains(exception.Message, existing.Name);
                Assert.IsFalse(called);
            }
            else
            {
                Assert.AreSame(incoming, middleware.OnCreate(incoming, next));
                Assert.IsTrue(called);
            }
        }

        private static TransponderRangeReservationRepository CreateRepository(IJobsRepository jobs, Transponder transponder, bool allowEnrichment = true)
        {
            var plan = new Mock<IMediaOpsPlanApi>();
            plan.Setup(value => value.Jobs).Returns(jobs);
            var transponders = new Mock<ITransponderRepository>(MockBehavior.Strict);
            transponders.Setup(value => value.Read(transponder.Id)).Returns(transponder);
            var connection = new Mock<IConnection>(MockBehavior.Strict);
            connection.SetupGet(value => value.IsShuttingDown).Returns(false);
            if (allowEnrichment)
            {
                transponders.Setup(value => value.Read(It.IsAny<FilterElement<Transponder>>())).Returns(new[] { transponder });
                connection.Setup(value => value.HandleMessages(It.IsAny<DMSMessage[]>()))
                    .Returns(new DMSMessage[] { new ManagerStoreCrudResponse<DomInstance>() });
            }
            var api = new SatOpsApi(connection.Object);
            typeof(SatOpsApi).GetField("lazyMediaOpsPlanApi", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(api, new Lazy<IMediaOpsPlanApi>(() => plan.Object));
            typeof(SatOpsApi).GetField("lazyTransponderRepository", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(api, new Lazy<ITransponderRepository>(() => transponders.Object));
            return new TransponderRangeReservationRepository(api);
        }

        private static IEnumerable<ManagedFilterIdentifier> ReadJobFilterLeaves(FilterElement<Job> filter)
        {
            if (filter is ANDFilterElement<Job> combined)
            {
                return combined.subFilters.SelectMany(ReadJobFilterLeaves);
            }

            return new[] { (ManagedFilterIdentifier)filter };
        }

        private static (Job Job, JobResourceNode Node) CreateJob(Guid resourceId, string name = "Booking")
        {
            var start = new DateTimeOffset(2026, 10, 29, 15, 0, 0, TimeSpan.Zero);
            var job = new Job(Guid.NewGuid()) { Name = name, Start = start, End = start.AddHours(1) };
            var node = new JobResourceNode(PredefinedGuids.TransponderResourcePoolGuid, resourceId);
            job.NodeGraph.Add(node);
            typeof(TransponderRangeReservationRepository)
                .GetMethod("ApplyBandwidthRange", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { node, 5d, 9d });
            typeof(TransponderRangeReservationRepository)
                .GetMethod("ApplySatelliteCapability", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { node, "SES-10" });
            return (job, node);
        }

        private static PropertyValuesInstance CreateProperty(Guid jobId, string slot, string name = null)
        {
            var property = new PropertyValuesInstance
            {
                PropertyValueInfo = { LinkedObjectID = jobId.ToString(), Scope = NamingConstants.PropertyInfoScope },
            };
            property.PropertyValue.Add(new PropertyValueSection
            {
                PropertyName = name ?? NamingConstants.PropertyInfoName,
                Value = slot,
                PropertyID = Guid.NewGuid(),
            });
            return property;
        }

        private static List<string> ReadLinkedIds(FilterElement<DomInstance> filter)
        {
            var combined = filter as ANDFilterElement<DomInstance>;
            Assert.IsNotNull(combined);
            Assert.AreEqual(2, combined.subFilters.Length);
            var definition = (ManagedFilterIdentifier)combined.subFilters[0];
            Assert.AreEqual(SlcPropertiesIds.Definitions.PropertyValues.Id, definition.getValue());
            var linkedFilter = combined.subFilters[1];
            var alternatives = linkedFilter as ORFilterElement<DomInstance>;
            var filters = alternatives == null ? new[] { linkedFilter } : alternatives.subFilters;
            return filters.Select(value => (string)((ManagedFilterIdentifier)value).getValue()).ToList();
        }
    }
}
