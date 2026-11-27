namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.Satellite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;

    [TestClass]
    public class SatelliteResourceRenameTests
    {
        [TestMethod]
        public void Execute_UpdatesLinkedResourcesBeforeSavingAndPreservesOtherSettings()
        {
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            var capabilities = new Mock<ICapabilitiesRepository>(MockBehavior.Strict);
            var capability = CreateCapability("Old");
            capabilities.Setup(api => api.Read(capability.Id)).Returns(capability);
            capabilities.Setup(api => api.Update(capability)).Returns(capability);
            var resource = CreateResource("Old");
            var otherCapabilityId = Guid.NewGuid();
            resource.AddCapability(new CapabilitySettings(otherCapabilityId).SetDiscretes(new[] { "Other" }));
            resource.AddCapacity(new RangeCapacitySetting(Guid.NewGuid()) { MinValue = 0, MaxValue = 36 });
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            var updated = false;
            resources.Setup(api => api.Update(resource)).Callback(() => updated = true).Returns(resource);

            var result = CreateSut(resources, capabilities).Execute(
                new[] { new Transponder { DOMResource = resource.Id } }, "New",
                () =>
                {
                    Assert.IsTrue(updated);
                    return new Satellite { Name = "New" };
                });

            Assert.AreEqual("New", result.Name);
            CollectionAssert.AreEquivalent(new[] { "Old", "New" }, capability.Discretes.ToArray());
            CollectionAssert.AreEqual(new[] { "New" }, resource.Capabilities.Single(item => item.Id == capability.Id).Discretes.ToArray());
            CollectionAssert.AreEqual(new[] { "Other" }, resource.Capabilities.Single(item => item.Id == otherCapabilityId).Discretes.ToArray());
            Assert.AreEqual(36m, ((RangeCapacitySetting)resource.Capacities.Single()).MaxValue);
            Assert.AreEqual("Transponder", resource.Name);
            resources.Verify(api => api.Update(resource), Times.Once);
        }

        [TestMethod]
        public void Execute_DeduplicatesLinkedResourcesAndSkipsAlreadyCorrectValues()
        {
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            var capabilities = new Mock<ICapabilitiesRepository>(MockBehavior.Strict);
            var capability = CreateCapability("New");
            var resource = CreateResource("New");
            capabilities.Setup(api => api.Read(capability.Id)).Returns(capability);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);

            CreateSut(resources, capabilities).Execute(
                new[]
                {
                    new Transponder { DOMResource = resource.Id },
                    new Transponder { DOMResource = resource.Id },
                    new Transponder(),
                }, "New", () => new Satellite());

            resources.Verify(api => api.Read(resource.Id), Times.Once);
            resources.VerifyNoOtherCalls();
            capabilities.Verify(api => api.Read(capability.Id), Times.Once);
            capabilities.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Execute_MissingLinkedResource_DoesNotSaveSatellite()
        {
            var resources = new Mock<IResourcesRepository>();
            var capabilities = new Mock<ICapabilitiesRepository>(MockBehavior.Strict);
            var saved = false;

            Assert.ThrowsException<InvalidOperationException>(() => CreateSut(resources, capabilities).Execute(
                new[] { new Transponder { DOMResource = Guid.NewGuid() } }, "New",
                () => { saved = true; return new Satellite(); }));

            Assert.IsFalse(saved);
            capabilities.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Execute_CapabilityUpdateFails_DoesNotUpdateResourcesOrSaveSatellite()
        {
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            var capabilities = new Mock<ICapabilitiesRepository>(MockBehavior.Strict);
            var capability = CreateCapability("Old");
            var resource = CreateResource("Old");
            capabilities.Setup(api => api.Read(capability.Id)).Returns(capability);
            capabilities.Setup(api => api.Update(capability)).Throws(new InvalidOperationException("Locked"));
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            var saved = false;

            Assert.ThrowsException<InvalidOperationException>(() => CreateSut(resources, capabilities).Execute(
                new[] { new Transponder { DOMResource = resource.Id } }, "New",
                () => { saved = true; return new Satellite(); }));

            Assert.IsFalse(saved);
            CollectionAssert.AreEqual(new[] { "Old" }, resource.Capabilities.Single().Discretes.ToArray());
            resources.Verify(api => api.Read(resource.Id), Times.Once);
            resources.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Execute_SatelliteSaveFails_RestoresResourceValues()
        {
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            var capabilities = new Mock<ICapabilitiesRepository>(MockBehavior.Strict);
            var capability = CreateCapability("Old", "New");
            var resource = CreateResource("Old");
            capabilities.Setup(api => api.Read(capability.Id)).Returns(capability);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            var writes = new List<string>();
            resources.Setup(api => api.Update(resource))
                .Callback(() => writes.Add(resource.Capabilities.Single().Discretes.Single())).Returns(resource);
            var failure = new InvalidOperationException("DOM save failed");

            var actual = Assert.ThrowsException<InvalidOperationException>(() => CreateSut(resources, capabilities).Execute(
                new[] { new Transponder { DOMResource = resource.Id } }, "New", () => throw failure));

            Assert.AreSame(failure, actual);
            CollectionAssert.AreEqual(new[] { "New", "Old" }, writes);
        }

        [TestMethod]
        public void Execute_SecondResourceUpdateFails_RestoresResourcesWithoutSavingSatellite()
        {
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            var capabilities = new Mock<ICapabilitiesRepository>(MockBehavior.Strict);
            var capability = CreateCapability("Old", "New");
            var first = CreateResource("Old");
            var second = CreateResource("Old");
            capabilities.Setup(api => api.Read(capability.Id)).Returns(capability);
            resources.Setup(api => api.Read(first.Id)).Returns(first);
            resources.Setup(api => api.Read(second.Id)).Returns(second);
            resources.Setup(api => api.Update(first)).Returns(first);
            resources.SetupSequence(api => api.Update(second))
                .Throws(new InvalidOperationException("Resource locked")).Returns(second);
            var saved = false;

            Assert.ThrowsException<InvalidOperationException>(() => CreateSut(resources, capabilities).Execute(
                new[] { new Transponder { DOMResource = first.Id }, new Transponder { DOMResource = second.Id } }, "New",
                () => { saved = true; return new Satellite(); }));

            Assert.IsFalse(saved);
            CollectionAssert.AreEqual(new[] { "Old" }, first.Capabilities.Single().Discretes.ToArray());
            CollectionAssert.AreEqual(new[] { "Old" }, second.Capabilities.Single().Discretes.ToArray());
            resources.Verify(api => api.Update(first), Times.Exactly(2));
        }

        [TestMethod]
        public void Execute_RollbackFails_ReportsOriginalAndRollbackFailures()
        {
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            var capabilities = new Mock<ICapabilitiesRepository>(MockBehavior.Strict);
            var capability = CreateCapability("Old", "New");
            var resource = CreateResource("Old");
            capabilities.Setup(api => api.Read(capability.Id)).Returns(capability);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            resources.SetupSequence(api => api.Update(resource))
                .Returns(resource).Throws(new InvalidOperationException("Rollback failed"));
            var failure = new InvalidOperationException("DOM save failed");

            var error = Assert.ThrowsException<AggregateException>(() => CreateSut(resources, capabilities).Execute(
                new[] { new Transponder { DOMResource = resource.Id } }, "New", () => throw failure));

            Assert.AreEqual(2, error.InnerExceptions.Count);
            Assert.AreSame(failure, error.InnerExceptions[0]);
            StringAssert.Contains(error.InnerExceptions[1].Message, resource.Id.ToString());
        }

        [TestMethod]
        public void Execute_ResourceHasNoSatelliteCapability_FailsBeforeSaving()
        {
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            var capabilities = new Mock<ICapabilitiesRepository>(MockBehavior.Strict);
            var capability = CreateCapability("Old");
            var resource = new UnmanagedResource();
            capabilities.Setup(api => api.Read(capability.Id)).Returns(capability);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            var saved = false;

            Assert.ThrowsException<InvalidOperationException>(() => CreateSut(resources, capabilities).Execute(
                new[] { new Transponder { DOMResource = resource.Id } }, "New",
                () => { saved = true; return new Satellite(); }));

            Assert.IsFalse(saved);
            resources.Verify(api => api.Read(resource.Id), Times.Once);
            resources.VerifyNoOtherCalls();
            capabilities.Verify(api => api.Read(capability.Id), Times.Once);
            capabilities.VerifyNoOtherCalls();
        }

        private static SatelliteResourceRename CreateSut(Mock<IResourcesRepository> resources, Mock<ICapabilitiesRepository> capabilities)
        {
            var api = new Mock<IMediaOpsPlanApi>(MockBehavior.Strict);
            api.SetupGet(item => item.Resources).Returns(resources.Object);
            api.SetupGet(item => item.Capabilities).Returns(capabilities.Object);
            return new SatelliteResourceRename(api.Object);
        }

        private static Capability CreateCapability(params string[] names)
        {
            return new Capability(PredefinedGuids.SatelliteCapabilityGuid).SetDiscretes(names);
        }

        private static Resource CreateResource(string satelliteName)
        {
            return new UnmanagedResource { Name = "Transponder" }
                .AddCapability(new CapabilitySettings(PredefinedGuids.SatelliteCapabilityGuid).SetDiscretes(new[] { satelliteName }));
        }
    }
}
