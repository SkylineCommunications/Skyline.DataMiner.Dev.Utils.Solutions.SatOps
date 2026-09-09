namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;

    /// <summary>
    /// Regression tests for SatOps Common 0.0.9:
    /// TransponderRangeReservationRepository.ApplyBandwidthSize used to add "Bandwidth Size"
    /// as a NumberCapacitySetting keyed on <see cref="PredefinedGuids.BandwidthSizeGuid"/>.
    /// That GUID belongs to a MediaOps <c>NumberConfiguration</c>, not a Capacity, so
    /// MediaOps <c>CapacitySettingValidator</c> threw
    /// <c>ArgumentNullException("capacity")</c> when the reservation was created.
    /// It must be stored as a <see cref="NumberConfigurationSetting"/> instead, while
    /// Transponder Bandwidth stays a <see cref="RangeCapacitySetting"/> and Satellite stays a
    /// <see cref="CapabilitySetting"/>.
    /// </summary>
    [TestClass]
    public class TransponderRangeReservationBandwidthSizeTests
    {
        private static readonly Assembly CommonAssembly =
            typeof(SatOpsApi).Assembly;

        private static readonly Type RepositoryType = CommonAssembly.GetType(
            "Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservationRepository",
            throwOnError: true);

        [TestMethod]
        public void ApplyBandwidthSize_DoesNotThrowCapacityNull_AndStoresAsNumberConfigurationSetting()
        {
            var node = CreateNode();

            // Previously threw ArgumentNullException("capacity") from CapacitySettingValidator via AddCapacity.
            InvokeApply("ApplyBandwidthSize", node, 36.0);

            var capacity = node.OrchestrationSettings.Capacities
                .FirstOrDefault(c => c.Id == PublicBandwidthSizeGuid);
            Assert.IsNull(capacity, "Bandwidth Size must NOT be stored as a Capacity - its GUID identifies a MediaOps NumberConfiguration.");

            var configurations = node.OrchestrationSettings.Configurations
                .OfType<NumberConfigurationSetting>()
                .Where(c => c.Id == PublicBandwidthSizeGuid)
                .ToList();

            Assert.AreEqual(1, configurations.Count, "Exactly one Bandwidth Size NumberConfigurationSetting must be stored.");
            Assert.AreEqual(36m, configurations[0].Value, "Bandwidth Size value must be persisted on the NumberConfigurationSetting.");
        }

        [TestMethod]
        public void ApplyBandwidthRange_StoresTransponderBandwidthAsRangeCapacitySetting()
        {
            var node = CreateNode();

            InvokeApply("ApplyBandwidthRange", node, 100.0, 136.0);

            var ranges = node.OrchestrationSettings.Capacities
                .OfType<RangeCapacitySetting>()
                .Where(c => c.Id == PublicTransponderBandwidthGuid)
                .ToList();

            Assert.AreEqual(1, ranges.Count, "Transponder Bandwidth must be stored as a RangeCapacitySetting.");
            Assert.AreEqual(100m, ranges[0].MinValue);
            Assert.AreEqual(136m, ranges[0].MaxValue);
        }

        [TestMethod]
        public void ApplySatelliteCapability_StoresSatelliteAsCapabilitySetting()
        {
            var node = CreateNode();

            InvokeApply("ApplySatelliteCapability", node, "Astra-19E");

            var capability = node.OrchestrationSettings.Capabilities
                .FirstOrDefault(c => c.Id == PublicSatelliteCapabilityGuid);

            Assert.IsNotNull(capability, "Satellite must be stored as a CapabilitySetting.");
            Assert.AreEqual("Astra-19E", capability.Value);
        }

        [TestMethod]
        public void ApplyBandwidthSize_CalledTwice_DoesNotCreateDuplicateConfiguration()
        {
            var node = CreateNode();

            InvokeApply("ApplyBandwidthSize", node, 36.0);
            InvokeApply("ApplyBandwidthSize", node, 72.0);

            var configurations = node.OrchestrationSettings.Configurations
                .OfType<NumberConfigurationSetting>()
                .Where(c => c.Id == PublicBandwidthSizeGuid)
                .ToList();

            Assert.AreEqual(1, configurations.Count, "Updating a reservation must not create duplicate Bandwidth Size configurations.");
            Assert.AreEqual(72m, configurations[0].Value, "The most recent Bandwidth Size value must be kept.");
        }

        [TestMethod]
        public void ApplyBandwidthRange_CalledTwice_DoesNotCreateDuplicateCapacity()
        {
            var node = CreateNode();

            InvokeApply("ApplyBandwidthRange", node, 100.0, 136.0);
            InvokeApply("ApplyBandwidthRange", node, 200.0, 236.0);

            var ranges = node.OrchestrationSettings.Capacities
                .OfType<RangeCapacitySetting>()
                .Where(c => c.Id == PublicTransponderBandwidthGuid)
                .ToList();

            Assert.AreEqual(1, ranges.Count, "Updating a reservation must not create duplicate Transponder Bandwidth capacities.");
            Assert.AreEqual(200m, ranges[0].MinValue);
            Assert.AreEqual(236m, ranges[0].MaxValue);
        }

        [TestMethod]
        public void ApplyBandwidthSize_WithNullSize_RemovesExistingConfiguration()
        {
            var node = CreateNode();

            InvokeApply("ApplyBandwidthSize", node, 36.0);
            InvokeApply("ApplyBandwidthSize", node, (double?)null);

            var configurations = node.OrchestrationSettings.Configurations
                .OfType<NumberConfigurationSetting>()
                .Where(c => c.Id == PublicBandwidthSizeGuid)
                .ToList();

            Assert.AreEqual(0, configurations.Count, "Clearing Bandwidth Size must remove the existing NumberConfigurationSetting.");
        }

        private static JobResourceNode CreateNode()
        {
            // (ResourcePoolId, ResourceId) - the specific resource GUID is irrelevant for these tests;
            // we only exercise the OrchestrationSettings on the node.
            return new JobResourceNode(SatelliteManagementConstant.TransponderResourcePoolGuid, Guid.NewGuid());
        }

        private static Guid PublicBandwidthSizeGuid => SatelliteManagementConstant.BandwidthSizeGuid;

        private static Guid PublicTransponderBandwidthGuid => SatelliteManagementConstant.TransponderBandwidthGuid;

        private static Guid PublicSatelliteCapabilityGuid => SatelliteManagementConstant.SatelliteCapabilityGuid;

        private static void InvokeApply(string methodName, JobResourceNode node, params object[] extraArgs)
        {
            var method = RepositoryType.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.IsNotNull(method, $"TransponderRangeReservationRepository.{methodName} helper is missing.");

            var args = new ArrayList { node };
            args.AddRange(extraArgs);
            method.Invoke(null, args.ToArray());
        }
    }
}
