namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.TransponderSlot
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot;

    /// <summary>
    /// Covers the slot layout that a single transponder plan row produces, and verifies that the produced
    /// slots are accepted by the overlap validation that runs when they are created.
    /// </summary>
    [TestClass]
    public class TransponderSlotCalculationTests
    {
        private static readonly Guid PlanId = Guid.NewGuid();

        [TestMethod]
        public void CalculateSlots_RowTilingTheFullTransponder_ProducesAdjacentSlots()
        {
            // A 72 MHz transponder with a single 36 MHz row: bw = 36, step = 36, offset = 0.
            var slots = CalculateSlots(transponderBandwidth: 72d, offset: 0d, step: 36d, bandwidth: 36d, limit: 0d);

            Assert.AreEqual(2, slots.Count);

            Assert.AreEqual("A36", slots[0].Name);
            Assert.AreEqual(0d, slots[0].Start);
            Assert.AreEqual(36d, slots[0].Stop);

            Assert.AreEqual("B36", slots[1].Name);
            Assert.AreEqual(36d, slots[1].Start);
            Assert.AreEqual(72d, slots[1].Stop);
        }

        [TestMethod]
        public void CalculateSlots_RowTilingTheFullTransponder_SlotsPassOverlapValidation()
        {
            var calculated = CalculateSlots(transponderBandwidth: 72d, offset: 0d, step: 36d, bandwidth: 36d, limit: 0d);
            var slots = calculated.Select(ToSlot).ToList();

            var repository = new Mock<ITransponderSlotRepository>();
            repository.Setup(r => r.ReadByTransponderPlan(It.IsAny<Guid>())).Returns(Enumerable.Empty<TransponderSlot>());
            var sut = new SlotOverlapValidationMiddleware(repository.Object);

            var result = sut.OnCreate(slots, s => s.ToList());

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void CalculateSlots_RowStoppingAtLimit_DoesNotExceedTheLimit()
        {
            var slots = CalculateSlots(transponderBandwidth: 72d, offset: 0d, step: 36d, bandwidth: 36d, limit: 36d);

            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual("A36", slots[0].Name);
            Assert.AreEqual(36d, slots[0].Stop);
        }

        [TestMethod]
        public void CalculateSlots_RowWithOffset_StartsAtTheOffset()
        {
            var slots = CalculateSlots(transponderBandwidth: 72d, offset: 36d, step: 36d, bandwidth: 36d, limit: 0d);

            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual(36d, slots[0].Start);
            Assert.AreEqual(72d, slots[0].Stop);
        }

        [TestMethod]
        public void CalculateSlots_PlanWithMultipleBandwidthRows_TilesTheTransponderPerBandwidth()
        {
            // A 36 MHz transponder with three rows: 4, 6 and 12 MHz. Every row tiles the full transponder,
            // so the rows deliberately cover the same frequency range.
            var slots = CalculateSlotsForRows(
                transponderBandwidth: 36d,
                rows: new[]
                {
                    new PlanRow(bandwidth: 4d, step: 4d, offset: 0d, limit: 36d),
                    new PlanRow(bandwidth: 6d, step: 6d, offset: 0d, limit: 36d),
                    new PlanRow(bandwidth: 12d, step: 12d, offset: 0d, limit: 36d),
                });

            Assert.AreEqual(9 + 6 + 3, slots.Count);
            CollectionAssert.AreEqual(
                new[] { "A4", "B4", "C4", "D4", "E4", "F4", "G4", "H4", "I4" },
                slots.Where(s => s.Bandwidth == 4d).Select(s => s.Name).ToArray());
            CollectionAssert.AreEqual(
                new[] { "A6", "B6", "C6", "D6", "E6", "F6" },
                slots.Where(s => s.Bandwidth == 6d).Select(s => s.Name).ToArray());
            CollectionAssert.AreEqual(
                new[] { "A12", "B12", "C12" },
                slots.Where(s => s.Bandwidth == 12d).Select(s => s.Name).ToArray());
        }

        [TestMethod]
        public void CalculateSlots_PlanWithMultipleBandwidthRows_SlotsPassOverlapValidation()
        {
            var calculated = CalculateSlotsForRows(
                transponderBandwidth: 36d,
                rows: new[]
                {
                    new PlanRow(bandwidth: 4d, step: 4d, offset: 0d, limit: 36d),
                    new PlanRow(bandwidth: 6d, step: 6d, offset: 0d, limit: 36d),
                    new PlanRow(bandwidth: 12d, step: 12d, offset: 0d, limit: 36d),
                });

            var slots = calculated.Select(ToSlot).ToList();

            var repository = new Mock<ITransponderSlotRepository>();
            repository.Setup(r => r.ReadByTransponderPlan(It.IsAny<Guid>())).Returns(Enumerable.Empty<TransponderSlot>());
            var sut = new SlotOverlapValidationMiddleware(repository.Object);

            var result = sut.OnCreate(slots, s => s.ToList());

            Assert.AreEqual(18, result.Count);
        }

        private static TransponderSlot ToSlot(CalculatedSlot calculated)
        {
            var slot = new TransponderSlot();
            slot.TransponderPlan = PlanId;
            slot.Name = calculated.Name;
            slot.SlotStartFrequency = calculated.Start;
            slot.SlotEndFrequency = calculated.Stop;
            slot.Bandwidth = calculated.Bandwidth;

            return slot;
        }

        private static IReadOnlyList<CalculatedSlot> CalculateSlotsForRows(double transponderBandwidth, IEnumerable<PlanRow> rows)
        {
            return rows
                .SelectMany(row => CalculateSlots(transponderBandwidth, row.Offset, row.Step, row.Bandwidth, row.Limit))
                .ToList();
        }

        private static IReadOnlyList<CalculatedSlot> CalculateSlots(double transponderBandwidth, double offset, double step, double bandwidth, double limit)
        {
            var method = typeof(TransponderSlotGenerator).GetMethod("CalculateSlots", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method, "TransponderSlotGenerator.CalculateSlots was not found.");

            var calculations = (IEnumerable)method.Invoke(
                null,
                new object[] { transponderBandwidth, 0d, 0d, offset, step, bandwidth, limit });

            return calculations.Cast<object>().Select(c => CalculatedSlot.FromCalculation(c, bandwidth)).ToList();
        }

        private sealed class PlanRow
        {
            public PlanRow(double bandwidth, double step, double offset, double limit)
            {
                Bandwidth = bandwidth;
                Step = step;
                Offset = offset;
                Limit = limit;
            }

            public double Bandwidth { get; }

            public double Step { get; }

            public double Offset { get; }

            public double Limit { get; }
        }

        private sealed class CalculatedSlot
        {
            public string Name { get; private set; }

            public double Start { get; private set; }

            public double Stop { get; private set; }

            public double Bandwidth { get; private set; }

            public static CalculatedSlot FromCalculation(object calculation, double bandwidth)
            {
                var type = calculation.GetType();

                return new CalculatedSlot
                {
                    Name = (string)type.GetProperty("SlotName").GetValue(calculation),
                    Start = (double)type.GetProperty("StartFrequency").GetValue(calculation),
                    Stop = (double)type.GetProperty("StopFrequency").GetValue(calculation),
                    Bandwidth = bandwidth,
                };
            }
        }
    }
}
