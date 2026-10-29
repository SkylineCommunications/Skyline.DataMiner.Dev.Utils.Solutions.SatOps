namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;

    [TestClass]
    public class TransponderRangeReservationReadTests
    {
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
