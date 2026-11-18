namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderSlot
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderSlot;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderPlanRow;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions;

    /// <summary>
    /// Calculates the slots of a transponder plan. This is an implementation detail of the slot creation and
    /// update pipeline and is deliberately not part of the public repository API.
    /// </summary>
    internal interface ITransponderSlotGenerator
    {
        /// <summary>
        /// Calculates the slots for the specified transponder plan without persisting or deleting anything.
        /// </summary>
        /// <param name="transponderPlanId">The unique identifier of the transponder plan.</param>
        /// <returns>The collection of calculated, not yet persisted, <see cref="TransponderSlot"/> instances.</returns>
        IReadOnlyCollection<TransponderSlot> BuildSlots(Guid transponderPlanId);
    }

    /// <summary>
    /// Calculates the slots of a transponder plan from its plan rows and the transponder it belongs to.
    /// </summary>
    internal sealed class TransponderSlotGenerator : ITransponderSlotGenerator
    {
        private const int FrequencyPrecision = 12;
        private const double FrequencyComparisonTolerance = 1e-12;

        private readonly ISatOpsApi satOpsApi;

        public TransponderSlotGenerator(ISatOpsApi satOpsApi)
        {
            this.satOpsApi = satOpsApi ?? throw new ArgumentNullException(nameof(satOpsApi));
        }

        public IReadOnlyCollection<TransponderSlot> BuildSlots(Guid transponderPlanId)
        {
            if (transponderPlanId == Guid.Empty)
                throw new ArgumentException(ExceptionMessages.ValueCannotBeEmptyGuid, nameof(transponderPlanId));

            var plan = satOpsApi.TransponderPlans.Read(transponderPlanId) ??
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderPlanWithIdWasNotFound, transponderPlanId), nameof(transponderPlanId));

            if (!plan.Transponder.HasValue || plan.Transponder.Value == Guid.Empty)
                throw new SlotValidationException("The selected plan has no transponder configured.");

            var transponder = satOpsApi.Transponders.Read(plan.Transponder.Value)
                ?? throw new SlotValidationException("The transponder configured on this plan no longer exists.");

            var planRows = satOpsApi.TransponderPlanRows
                .Read(TransponderPlanRowExposers.TransponderPlan.Equal(transponderPlanId))
                .ToList();

            double transponderBandwidth = transponder.Bandwidth.GetValueOrDefault();
            double transponderStartFrequency = transponder.StartFrequency.GetValueOrDefault();
            double transponderDownlinkStartFrequency = transponder.DownlinkStartFreq.GetValueOrDefault();

            if (!IsFinite(transponderBandwidth) || transponderBandwidth <= 0
                || !IsFinite(transponderStartFrequency) || !IsFinite(transponderDownlinkStartFrequency))
                throw new SlotValidationException("The transponder must have a positive bandwidth and valid uplink and downlink frequencies.");

            if (planRows.GroupBy(row => row.Bandwidth).Any(group => group.Count() > 1))
                throw new SlotValidationException("The plan contains multiple rows for the same bandwidth. Keep one row per bandwidth before regenerating slots.", isConflict: true);

            var slots = new List<TransponderSlot>();
            foreach (var row in planRows)
            {
                double offset = row.Offset.GetValueOrDefault();
                double step = row.StepSize.GetValueOrDefault();
                double bandwidth = row.Bandwidth.GetValueOrDefault();
                double limit = row.Limit.GetValueOrDefault();

                if (!IsFinite(offset) || offset < 0 || !IsFinite(step) || step <= 0
                    || !IsFinite(bandwidth) || bandwidth <= 0 || !IsFinite(limit) || limit < 0)
                    throw new SlotValidationException("Each plan row must have a positive bandwidth and step size, and a non-negative offset and limit.");

                foreach (var calc in CalculateSlots(transponderBandwidth, transponderStartFrequency, transponderDownlinkStartFrequency, offset, step, bandwidth, limit))
                {
                    var slot = new TransponderSlot();
                    slot.TransponderPlan = transponderPlanId;
                    slot.Name = calc.SlotName;
                    slot.SlotStartFrequency = calc.StartFrequency;
                    slot.SlotEndFrequency = calc.StopFrequency;
                    slot.Bandwidth = bandwidth;
                    slot.UplinkFreq = calc.UplinkFrequency;
                    slot.DownlinkFreq = calc.DownlinkFrequency;
                    slots.Add(slot);
                }
            }

            return slots;
        }

        private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

        private static IEnumerable<SlotCalculation> CalculateSlots(
            double transponderBandwidth,
            double transponderStartFrequency,
            double transponderDownlinkStartFrequency,
            double offset,
            double step,
            double bandwidth,
            double limit)
        {
            if (bandwidth <= 0)
                yield break;

            int numberOfSlots = (int)Math.Round(transponderBandwidth / bandwidth, MidpointRounding.AwayFromZero);
            for (int i = 0; i < numberOfSlots; i++)
            {
                double startFreq = Math.Round(offset + (i * step), FrequencyPrecision, MidpointRounding.AwayFromZero);
                double stopFreq = Math.Round(startFreq + bandwidth, FrequencyPrecision, MidpointRounding.AwayFromZero);

                if ((limit > 0 && stopFreq - limit > FrequencyComparisonTolerance) ||
                    stopFreq - transponderBandwidth > FrequencyComparisonTolerance)
                    yield break;

                double midpoint = (startFreq + stopFreq) / 2;
                yield return new SlotCalculation
                {
                    SlotName = $"{GetAlphabetLetter(i)}{bandwidth}",
                    StartFrequency = startFreq,
                    StopFrequency = stopFreq,
                    UplinkFrequency = midpoint + transponderStartFrequency,
                    DownlinkFrequency = midpoint + transponderDownlinkStartFrequency,
                };
            }
        }

        private static string GetAlphabetLetter(int index)
        {
            const int alphabetLength = 26;
            if (index < alphabetLength)
                return ((char)('A' + index)).ToString();

            return $"{(char)('A' + (index / alphabetLength) - 1)}{(char)('A' + (index % alphabetLength))}";
        }

        private sealed class SlotCalculation
        {
            public string SlotName { get; set; }

            public double StartFrequency { get; set; }

            public double StopFrequency { get; set; }

            public double UplinkFrequency { get; set; }

            public double DownlinkFrequency { get; set; }
        }
    }
}
