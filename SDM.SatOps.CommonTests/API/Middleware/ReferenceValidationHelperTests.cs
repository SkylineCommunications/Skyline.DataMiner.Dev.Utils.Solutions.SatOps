namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;

    using DomModel = Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;

    /// <summary>
    /// Tests for the <c>ReferenceValidationHelper</c> shared by the validation middlewares.
    /// </summary>
    [TestClass]
    public class ReferenceValidationHelperTests
    {
        private sealed class Item
        {
            public Guid? Reference { get; set; }
        }

        [TestMethod]
        public void ReadExistingIds_NullSelector_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => ReferenceValidationHelper.ReadExistingIds<Item, Satellite>(new List<Item>(), null, CreateRepository().Object));
        }

        [TestMethod]
        public void ReadExistingIds_NullRepository_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => ReferenceValidationHelper.ReadExistingIds<Item, Satellite>(new List<Item>(), item => item.Reference, null));
        }

        [TestMethod]
        public void ReadExistingIds_NullItems_ReturnsEmptySetWithoutReading()
        {
            var repository = CreateRepository();

            var result = ReferenceValidationHelper.ReadExistingIds<Item, Satellite>(null, item => item.Reference, repository.Object);

            Assert.AreEqual(0, result.Count);
            repository.Verify(r => r.Read(It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [TestMethod]
        public void ReadExistingIds_OnlyUnusableReferences_ReturnsEmptySetWithoutReading()
        {
            var repository = CreateRepository();
            var items = new List<Item> { null, new Item(), new Item { Reference = Guid.Empty } };

            var result = ReferenceValidationHelper.ReadExistingIds(items, item => item.Reference, repository.Object);

            Assert.AreEqual(0, result.Count);
            repository.Verify(r => r.Read(It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [TestMethod]
        public void ReadExistingIds_RepeatedReference_IsRequestedOnlyOnce()
        {
            var referenceId = Guid.NewGuid();
            var requestedIds = new List<Guid>();

            var repository = CreateRepository();
            repository.Setup(r => r.Read(It.IsAny<IEnumerable<Guid>>()))
                .Returns((IEnumerable<Guid> ids) =>
                {
                    requestedIds.AddRange(ids);
                    return requestedIds.Select(CreateSatelliteWithId).ToList();
                });

            var items = new List<Item>
            {
                new Item { Reference = referenceId },
                new Item { Reference = referenceId },
                new Item { Reference = referenceId },
            };

            var result = ReferenceValidationHelper.ReadExistingIds(items, item => item.Reference, repository.Object);

            repository.Verify(r => r.Read(It.IsAny<IEnumerable<Guid>>()), Times.Once);
            CollectionAssert.AreEqual(new[] { referenceId }, requestedIds);
            CollectionAssert.AreEqual(new[] { referenceId }, result.ToList());
        }

        [TestMethod]
        public void ReadExistingIds_RepositoryReturnsNull_ReturnsEmptySet()
        {
            var repository = CreateRepository();
            repository.Setup(r => r.Read(It.IsAny<IEnumerable<Guid>>())).Returns((IEnumerable<Satellite>)null);

            var items = new List<Item> { new Item { Reference = Guid.NewGuid() } };

            var result = ReferenceValidationHelper.ReadExistingIds(items, item => item.Reference, repository.Object);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ReadExistingIds_MissingReference_IsNotReportedAsExisting()
        {
            var existingId = Guid.NewGuid();
            var missingId = Guid.NewGuid();

            var repository = CreateRepository();
            repository.Setup(r => r.Read(It.IsAny<IEnumerable<Guid>>()))
                .Returns((IEnumerable<Guid> ids) => ids.Where(id => id == existingId).Select(CreateSatelliteWithId).ToList());

            var items = new List<Item> { new Item { Reference = existingId }, new Item { Reference = missingId } };

            var result = ReferenceValidationHelper.ReadExistingIds(items, item => item.Reference, repository.Object);

            Assert.IsTrue(result.Contains(existingId));
            Assert.IsFalse(result.Contains(missingId));
        }

        [TestMethod]
        public void ReadCached_NullRead_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => ReferenceValidationHelper.ReadCached<string>(Guid.NewGuid(), new Dictionary<Guid, IReadOnlyList<string>>(), null));
        }

        [TestMethod]
        public void ReadCached_SameKeyTwice_ReadsOnlyOnce()
        {
            var key = Guid.NewGuid();
            var cache = new Dictionary<Guid, IReadOnlyList<string>>();
            var readCount = 0;

            IEnumerable<string> Read(Guid id)
            {
                readCount++;
                return new[] { "value" };
            }

            var first = ReferenceValidationHelper.ReadCached(key, cache, Read);
            var second = ReferenceValidationHelper.ReadCached(key, cache, Read);

            Assert.AreEqual(1, readCount);
            CollectionAssert.AreEqual(new[] { "value" }, first.ToList());
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void ReadCached_DifferentKeys_ReadsPerKey()
        {
            var cache = new Dictionary<Guid, IReadOnlyList<string>>();
            var readCount = 0;

            IEnumerable<string> Read(Guid id)
            {
                readCount++;
                return new[] { id.ToString() };
            }

            ReferenceValidationHelper.ReadCached(Guid.NewGuid(), cache, Read);
            ReferenceValidationHelper.ReadCached(Guid.NewGuid(), cache, Read);

            Assert.AreEqual(2, readCount);
        }

        [TestMethod]
        public void ReadCached_NullCache_AlwaysReads()
        {
            var key = Guid.NewGuid();
            var readCount = 0;

            IEnumerable<string> Read(Guid id)
            {
                readCount++;
                return new[] { "value" };
            }

            ReferenceValidationHelper.ReadCached(key, null, Read);
            ReferenceValidationHelper.ReadCached(key, null, Read);

            Assert.AreEqual(2, readCount);
        }

        [TestMethod]
        public void ReadCached_ReadReturnsNull_ReturnsEmptyList()
        {
            var result = ReferenceValidationHelper.ReadCached<string>(Guid.NewGuid(), null, _ => null);

            Assert.AreEqual(0, result.Count);
        }

        private static Mock<ISatelliteRepository> CreateRepository()
        {
            return new Mock<ISatelliteRepository>();
        }

        private static Satellite CreateSatelliteWithId(Guid id)
        {
            var domInstance = new DomInstance
            {
                ID = new DomInstanceId(id) { ModuleId = DomModel.SlcSatellite_ManagementIds.ModuleId },
                DomDefinitionId = DomModel.SlcSatellite_ManagementIds.Definitions.Satellites,
            };

            return Satellite.FromInstance(new DomModel.SatellitesInstance(domInstance));
        }
    }
}
