namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement.Transponder
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;

    [TestClass]
    public class TransponderResourceLifecycleTests
    {
        [DataTestMethod]
        [DataRow(ResourceState.Complete, ResourceState.Deprecated)]
        [DataRow(ResourceState.Deprecated, ResourceState.Complete)]
        public void Transition_ChangesLinkedResourceBeforeTransponder(ResourceState original, ResourceState target)
        {
            var resource = CreateResource(original);
            var transitioned = CreateResource(target);
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            var changed = false;
            if (target == ResourceState.Deprecated)
                resources.Setup(api => api.Deprecate(resource)).Callback(() => changed = true).Returns(transitioned);
            else
                resources.Setup(api => api.Restore(resource)).Callback(() => changed = true).Returns(transitioned);
            var expected = new Transponder();

            var result = new TransponderResourceLifecycle(resources.Object).Transition(resource.Id, target, () =>
            {
                Assert.IsTrue(changed);
                return expected;
            });

            Assert.AreSame(expected, result);
        }

        [DataTestMethod]
        [DataRow(ResourceState.Deprecated)]
        [DataRow(ResourceState.Complete)]
        public void Transition_ResourceAlreadyInTargetState_DoesNotTransitionResource(ResourceState target)
        {
            var resource = CreateResource(target);
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            var expected = new Transponder();

            var result = new TransponderResourceLifecycle(resources.Object).Transition(resource.Id, target, () => expected);

            Assert.AreSame(expected, result);
            resources.Verify(api => api.Read(resource.Id), Times.Once);
            resources.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Transition_ResourceDeprecationRejected_DoesNotTransitionTransponder()
        {
            var resource = CreateResource(ResourceState.Complete);
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            var failure = new InvalidOperationException("Resource is used in a booking.");
            resources.Setup(api => api.Deprecate(resource)).Throws(failure);
            var transitioned = false;

            var actual = Assert.ThrowsException<InvalidOperationException>(() =>
                new TransponderResourceLifecycle(resources.Object).Transition(resource.Id, ResourceState.Deprecated,
                    () => { transitioned = true; return new Transponder(); }));

            Assert.AreSame(failure, actual);
            Assert.IsFalse(transitioned);
        }

        [DataTestMethod]
        [DataRow(ResourceState.Complete, ResourceState.Deprecated)]
        [DataRow(ResourceState.Deprecated, ResourceState.Complete)]
        public void Transition_DomTransitionFails_RollsBackResource(ResourceState original, ResourceState target)
        {
            var resource = CreateResource(original);
            var transitioned = CreateResource(target);
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            if (target == ResourceState.Deprecated)
            {
                resources.Setup(api => api.Deprecate(resource)).Returns(transitioned);
                resources.Setup(api => api.Restore(transitioned)).Returns(resource);
            }
            else
            {
                resources.Setup(api => api.Restore(resource)).Returns(transitioned);
                resources.Setup(api => api.Deprecate(transitioned)).Returns(resource);
            }
            var failure = new InvalidOperationException("DOM transition failed.");

            var actual = Assert.ThrowsException<InvalidOperationException>(() =>
                new TransponderResourceLifecycle(resources.Object).Transition(resource.Id, target, () => throw failure));

            Assert.AreSame(failure, actual);
            if (target == ResourceState.Deprecated)
                resources.Verify(api => api.Restore(transitioned), Times.Once);
            else
                resources.Verify(api => api.Deprecate(transitioned), Times.Once);
        }

        [TestMethod]
        public void Transition_RollbackFails_ReportsBothFailures()
        {
            var resource = CreateResource(ResourceState.Complete);
            var deprecated = CreateResource(ResourceState.Deprecated);
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            resources.Setup(api => api.Deprecate(resource)).Returns(deprecated);
            var rollbackFailure = new InvalidOperationException("Restore failed.");
            resources.Setup(api => api.Restore(deprecated)).Throws(rollbackFailure);
            var failure = new InvalidOperationException("DOM transition failed.");

            var actual = Assert.ThrowsException<AggregateException>(() =>
                new TransponderResourceLifecycle(resources.Object).Transition(resource.Id, ResourceState.Deprecated, () => throw failure));

            CollectionAssert.AreEqual(new Exception[] { failure, rollbackFailure }, actual.InnerExceptions);
            StringAssert.Contains(actual.Message, resource.Id.ToString());
        }

        [TestMethod]
        public void Transition_LinkedResourceMissing_DoesNotTransitionTransponder()
        {
            var resources = new Mock<IResourcesRepository>();
            var transitioned = false;

            Assert.ThrowsException<InvalidOperationException>(() =>
                new TransponderResourceLifecycle(resources.Object).Transition(Guid.NewGuid(), ResourceState.Deprecated,
                    () => { transitioned = true; return new Transponder(); }));

            Assert.IsFalse(transitioned);
        }

        [TestMethod]
        public void Transition_LinkedResourceDraft_DoesNotTransitionTransponder()
        {
            var resource = CreateResource(ResourceState.Draft);
            var resources = new Mock<IResourcesRepository>(MockBehavior.Strict);
            resources.Setup(api => api.Read(resource.Id)).Returns(resource);
            var transitioned = false;

            Assert.ThrowsException<InvalidOperationException>(() =>
                new TransponderResourceLifecycle(resources.Object).Transition(resource.Id, ResourceState.Deprecated,
                    () => { transitioned = true; return new Transponder(); }));

            Assert.IsFalse(transitioned);
        }

        private static Resource CreateResource(ResourceState state)
        {
            var resource = new UnmanagedResource { Name = "Transponder" };
            // MediaOps exposes state as read-only; tests model persisted resources without a DataMiner connection.
            typeof(Resource).GetProperty(nameof(Resource.State)).SetValue(resource, state);
            return resource;
        }
    }
}
