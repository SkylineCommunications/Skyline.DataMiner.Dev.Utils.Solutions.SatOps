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

        private static TransponderSlot ToSlot(CalculatedSlot calculated)
        {
            var slot = new TransponderSlot();
            slot.TransponderPlan = PlanId;
            slot.Name = calculated.Name;
            slot.SlotStartFrequency = calculated.Start;
            slot.SlotEndFrequency = calculated.Stop;

            return slot;
        }

        private static IReadOnlyList<CalculatedSlot> CalculateSlots(double transponderBandwidth, double offset, double step, double bandwidth, double limit)
        {
            var method = typeof(TransponderSlotGenerator).GetMethod("CalculateSlots", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method, "TransponderSlotGenerator.CalculateSlots was not found.");

            var calculations = (IEnumerable)method.Invoke(
                null,
                new object[] { transponderBandwidth, 0d, 0d, offset, step, bandwidth, limit });

            return calculations.Cast<object>().Select(CalculatedSlot.FromCalculation).ToList();
        }

        private sealed class CalculatedSlot
        {
            public string Name { get; private set; }

            public double Start { get; private set; }

            public double Stop { get; private set; }

            public static CalculatedSlot FromCalculation(object calculation)
            {
                var type = calculation.GetType();

                return new CalculatedSlot
                {
                    Name = (string)type.GetProperty("SlotName").GetValue(calculation),
                    Start = (double)type.GetProperty("StartFrequency").GetValue(calculation),
                    Stop = (double)type.GetProperty("StopFrequency").GetValue(calculation),
                };
            }
        }
    }
}
