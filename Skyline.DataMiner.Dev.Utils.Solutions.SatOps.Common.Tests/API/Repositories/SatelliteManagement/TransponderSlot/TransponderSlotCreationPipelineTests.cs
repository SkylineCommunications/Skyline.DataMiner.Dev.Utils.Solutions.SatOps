namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.TransponderSlot
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderPlan;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;

    /// <summary>
    /// Drives the complete slot creation pipeline that <c>SatOpsApi</c> wires up
    /// (<c>TransponderSlotValidationMiddleware</c> followed by <c>SlotOverlapValidationMiddleware</c>)
    /// for a transponder plan whose rows deliberately produce overlapping slots of different bandwidths.
    /// </summary>
    [TestClass]
    public class TransponderSlotCreationPipelineTests
    {
        private const double TransponderBandwidth = 36d;
        private const double TransponderStartFrequency = 14000d;
        private const double TransponderDownlinkStartFrequency = 11000d;

        private static readonly TransponderPlan Plan = new TransponderPlan();
        private static readonly Guid PlanId = Plan.Id;

        /// <summary>
        /// The reported scenario: a 36 MHz transponder with 4, 6 and 12 MHz plan rows.
        /// Every row tiles the full transponder, so 'A4', 'A6' and 'A12' all start at 0 MHz.
        /// </summary>
        [TestMethod]
        public void Create_PlanWithFourSixAndTwelveMhzRows_CreatesAllSlots()
        {
            var store = new List<TransponderSlot>();
            var repository = CreateRepository(store);
            var pipeline = BuildPipeline(repository, BuildReportedScenarioSlots());

            var created = pipeline.Create(PlanId);

            Assert.AreEqual(18, created.Count);
            Assert.AreEqual(18, store.Count);
            CollectionAssert.AreEquivalent(
                new[] { "A4", "B4", "C4", "D4", "E4", "F4", "G4", "H4", "I4", "A6", "B6", "C6", "D6", "E6", "F6", "A12", "B12", "C12" },
                created.Select(s => s.Name).ToArray());
        }

        [TestMethod]
        public void Create_PlanWithFourSixAndTwelveMhzRows_KeepsOverlappingBandwidthLayers()
        {
            var store = new List<TransponderSlot>();
            var repository = CreateRepository(store);
            var pipeline = BuildPipeline(repository, BuildReportedScenarioSlots());

            var created = pipeline.Create(PlanId);

            var a4 = created.Single(s => s.Name == "A4");
            var a6 = created.Single(s => s.Name == "A6");
            var a12 = created.Single(s => s.Name == "A12");

            // All three deliberately share the start of the transponder.
            Assert.AreEqual(0d, a4.SlotStartFrequency.Value);
            Assert.AreEqual(0d, a6.SlotStartFrequency.Value);
            Assert.AreEqual(0d, a12.SlotStartFrequency.Value);
            Assert.AreEqual(4d, a4.SlotEndFrequency.Value);
            Assert.AreEqual(6d, a6.SlotEndFrequency.Value);
            Assert.AreEqual(12d, a12.SlotEndFrequency.Value);
        }

        /// <summary>
        /// Regenerating a plan must not report the slots that are being replaced as overlapping.
        /// </summary>
        [TestMethod]
        public void Create_PlanThatAlreadyHasSlots_ReplacesThemWithoutOverlapFailure()
        {
            var store = new List<TransponderSlot>(BuildReportedScenarioSlots());
            var repository = CreateRepository(store);
            var pipeline = BuildPipeline(repository, BuildReportedScenarioSlots());

            var created = pipeline.Create(PlanId);

            Assert.AreEqual(18, created.Count);
            Assert.AreEqual(18, store.Count);
        }

        /// <summary>
        /// Two slots of the same bandwidth that share frequency space must still be rejected.
        /// </summary>
        [TestMethod]
        public void Create_GeneratorProducingOverlapWithinOneBandwidth_ThrowsArgumentException()
        {
            var store = new List<TransponderSlot>();
            var repository = CreateRepository(store);
            var overlappingWithinLayer = new[]
            {
                CreateSlot("A4", 0d, 4d, 4d),
                CreateSlot("B4", 2d, 6d, 4d),
            };
            var pipeline = BuildPipeline(repository, overlappingWithinLayer);

            Assert.ThrowsException<SlotValidationException>(() => pipeline.Create(PlanId));
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Create_InvalidReplacement_DoesNotDeleteExistingSlots(bool duplicateName)
        {
            var original = CreateSlot("Original", 0, 4, 4);
            var store = new List<TransponderSlot> { original };
            var replacements = new[] { CreateSlot("A4", 0, 4, 4), CreateSlot(duplicateName ? "A4" : "B4", 2, 6, 4) };
            var pipeline = BuildPipeline(CreateRepository(store), replacements);

            var exception = Assert.ThrowsException<SlotValidationException>(() => pipeline.Create(PlanId));

            Assert.IsTrue(exception.IsConflict);
            Assert.AreEqual(1, store.Count);
            Assert.AreSame(original, store[0]);
        }

        [TestMethod]
        public void Create_InvalidSlotFields_DoesNotDeleteExistingSlots()
        {
            var original = CreateSlot("Original", 0, 4, 4);
            var store = new List<TransponderSlot> { original };
            var invalid = CreateSlot("A4", 0, 4, 4);
            invalid.Name = null;
            var pipeline = BuildPipeline(CreateRepository(store), new[] { invalid });

            Assert.ThrowsException<SlotValidationException>(() => pipeline.Create(PlanId));

            Assert.AreSame(original, store.Single());
        }

        [TestMethod]
        public void Create_RepeatedRegeneration_ProducesOneReplacementSet()
        {
            var store = new List<TransponderSlot>();
            var pipeline = BuildPipeline(CreateRepository(store), BuildReportedScenarioSlots());

            pipeline.Create(PlanId);
            pipeline.Create(PlanId);

            Assert.AreEqual(18, store.Count);
            Assert.AreEqual(18, store.Select(slot => slot.Name).Distinct().Count());
        }

        [DataTestMethod]
        [DataRow("Delete")]
        [DataRow("Create")]
        public void Create_PersistenceFailure_ReportsPartialWriteStage(string stage)
        {
            var failure = new InvalidOperationException("DOM backend failed.");
            var repository = new Mock<ITransponderSlotRepository>();
            if (stage == "Delete")
                repository.Setup(value => value.DeleteSlotsByTransponderPlan(PlanId)).Throws(failure);
            else
                repository.Setup(value => value.Create(It.IsAny<IEnumerable<TransponderSlot>>())).Throws(failure);
            var pipeline = BuildPipeline(repository.Object, BuildReportedScenarioSlots());

            var exception = Assert.ThrowsException<SlotRegenerationException>(() => pipeline.Create(PlanId));

            Assert.AreEqual(stage, exception.Stage);
            Assert.AreSame(failure, exception.InnerException);
            CollectionAssert.Contains(exception.PlanIds.ToList(), PlanId);
            StringAssert.Contains(exception.Message, "Some slots may have changed");
            if (stage == "Delete")
                repository.Verify(value => value.Create(It.IsAny<IEnumerable<TransponderSlot>>()), Times.Never);
        }

        [TestMethod]
        public void Create_ConcurrentRegeneration_IsRejectedBeforeMutationAndLockIsReleased()
        {
            var store = new List<TransponderSlot>();
            var repository = CreateRepository(store);
            var generator = new Mock<ITransponderSlotGenerator>();
            using (var entered = new ManualResetEventSlim())
            using (var release = new ManualResetEventSlim())
            {
                generator.Setup(value => value.BuildSlots(PlanId)).Returns(() =>
                {
                    entered.Set();
                    if (!release.Wait(TimeSpan.FromSeconds(10)))
                        throw new TimeoutException("Test did not release slot generation.");
                    return BuildReportedScenarioSlots();
                });
                var pipeline = repository.WithMiddleware(null, generator.Object);
                var first = Task.Run(() => pipeline.Create(PlanId));
                try
                {
                    Assert.IsTrue(entered.Wait(TimeSpan.FromSeconds(10)));
                    var exception = Assert.ThrowsException<SlotValidationException>(() => pipeline.Create(PlanId));
                    Assert.IsTrue(exception.IsConflict);
                    Assert.AreEqual(0, store.Count);
                }
                finally
                {
                    release.Set();
                    first.GetAwaiter().GetResult();
                }

                Assert.AreEqual(18, pipeline.Create(PlanId).Count);
            }
        }

        [TestMethod]
        public void Update_ExistingSlot_UsesUpdateAndPreservesId()
        {
            var existing = CreateSlot("A4", 0, 4, 4);
            var store = new List<TransponderSlot> { existing };
            var repository = new Mock<ITransponderSlotRepository>();
            repository.Setup(value => value.ReadByTransponderPlan(PlanId)).Returns(() => store.ToList());
            repository.Setup(value => value.Update(existing)).Returns(existing);
            var pipeline = BuildPipeline(repository.Object, Array.Empty<TransponderSlot>());
            var id = existing.Id;
            existing.Name = "Edited";

            var updated = pipeline.Update(existing);

            Assert.AreEqual(id, updated.Id);
            Assert.AreEqual("Edited", updated.Name);
            repository.Verify(value => value.Update(existing), Times.Once);
            repository.Verify(value => value.Create(It.IsAny<TransponderSlot>()), Times.Never);
            repository.Verify(value => value.Create(It.IsAny<IEnumerable<TransponderSlot>>()), Times.Never);
        }

        [TestMethod]
        public void Update_DuplicateName_DoesNotPersist()
        {
            var existing = CreateSlot("A4", 0, 4, 4);
            var other = CreateSlot("B4", 4, 8, 4);
            var repository = new Mock<ITransponderSlotRepository>();
            repository.Setup(value => value.ReadByTransponderPlan(PlanId)).Returns(new[] { existing, other });
            var pipeline = BuildPipeline(repository.Object, Array.Empty<TransponderSlot>());
            existing.Name = other.Name;

            Assert.ThrowsException<SlotValidationException>(() => pipeline.Update(existing));

            repository.Verify(value => value.Update(It.IsAny<TransponderSlot>()), Times.Never);
        }

        private static ITransponderSlotRepository BuildPipeline(ITransponderSlotRepository repository, IReadOnlyCollection<TransponderSlot> generatedSlots)
        {
            var generator = new Mock<ITransponderSlotGenerator>();
            generator.Setup(g => g.BuildSlots(It.IsAny<Guid>())).Returns(generatedSlots);

            var planRepository = new Mock<ITransponderPlanRepository>();
            planRepository.Setup(r => r.Read(It.IsAny<Guid>())).Returns(Plan);
            planRepository.Setup(r => r.Read(It.IsAny<IEnumerable<Guid>>())).Returns(new[] { Plan });

            // Mirrors the middleware chain configured in SatOpsApi.TransponderSlots.
            return repository
                .WithMiddleware(new TransponderSlotValidationMiddleware(planRepository.Object), generator.Object)
                .WithMiddleware(new SlotOverlapValidationMiddleware(repository), generator.Object);
        }

        private static ITransponderSlotRepository CreateRepository(List<TransponderSlot> store)
        {
            var repository = new Mock<ITransponderSlotRepository>();

            repository.Setup(r => r.ReadByTransponderPlan(It.IsAny<Guid>()))
                .Returns(() => store.ToList());

            repository.Setup(r => r.DeleteSlotsByTransponderPlan(It.IsAny<Guid>()))
                .Callback(() => store.Clear());

            repository.Setup(r => r.Create(It.IsAny<IEnumerable<TransponderSlot>>()))
                .Returns((IEnumerable<TransponderSlot> slots) =>
                {
                    var created = slots.ToList();
                    store.AddRange(created);
                    return created;
                });

            return repository.Object;
        }

        /// <summary>
        /// Produces exactly what <c>TransponderSlotGenerator</c> calculates for the reported plan:
        /// a 36 MHz transponder with 4, 6 and 12 MHz rows.
        /// </summary>
        private static IReadOnlyCollection<TransponderSlot> BuildReportedScenarioSlots()
        {
            var rows = new[]
            {
                new { Bandwidth = 4d, Step = 4d, Offset = 0d, Limit = 36d },
                new { Bandwidth = 6d, Step = 6d, Offset = 0d, Limit = 36d },
                new { Bandwidth = 12d, Step = 12d, Offset = 0d, Limit = 36d },
            };

            return rows
                .SelectMany(row => CalculateSlots(row.Offset, row.Step, row.Bandwidth, row.Limit))
                .ToList();
        }

        private static IEnumerable<TransponderSlot> CalculateSlots(double offset, double step, double bandwidth, double limit)
        {
            var method = typeof(TransponderSlotGenerator).GetMethod("CalculateSlots", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method, "TransponderSlotGenerator.CalculateSlots was not found.");

            var calculations = (IEnumerable)method.Invoke(
                null,
                new object[] { TransponderBandwidth, TransponderStartFrequency, TransponderDownlinkStartFrequency, offset, step, bandwidth, limit });

            foreach (var calculation in calculations)
            {
                var type = calculation.GetType();
                var slot = CreateSlot(
                    (string)type.GetProperty("SlotName").GetValue(calculation),
                    (double)type.GetProperty("StartFrequency").GetValue(calculation),
                    (double)type.GetProperty("StopFrequency").GetValue(calculation),
                    bandwidth);
                slot.UplinkFreq = (double)type.GetProperty("UplinkFrequency").GetValue(calculation);
                slot.DownlinkFreq = (double)type.GetProperty("DownlinkFrequency").GetValue(calculation);

                yield return slot;
            }
        }

        private static TransponderSlot CreateSlot(string name, double start, double stop, double bandwidth)
        {
            var slot = new TransponderSlot();
            slot.TransponderPlan = PlanId;
            slot.Name = name;
            slot.SlotStartFrequency = start;
            slot.SlotEndFrequency = stop;
            slot.Bandwidth = bandwidth;
            slot.UplinkFreq = ((start + stop) / 2) + TransponderStartFrequency;
            slot.DownlinkFreq = ((start + stop) / 2) + TransponderDownlinkStartFrequency;

            return slot;
        }
    }
}
