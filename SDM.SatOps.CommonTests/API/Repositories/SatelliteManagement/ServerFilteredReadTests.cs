namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Beam;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BeamObject = Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam;
    using Model = Skyline.DataMiner.Solutions.SatOps.Common.DOM.Model;
    using SatelliteObject = Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite;
    using TransponderObject = Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder;

    [TestClass]
    public class ServerFilteredReadTests
    {
        [DataTestMethod]
        [DataRow(InstanceStatus.Draft, "draft")]
        [DataRow(InstanceStatus.Active, "active")]
        [DataRow(InstanceStatus.Deprecated, "deprecated")]
        [DataRow(InstanceStatus.Error, "error")]
        public void ReadFilters_Status_UsesExactDomStatusId(InstanceStatus status, string expectedId)
        {
            var api = CreateApi();

            AssertContainsStatus(new SatelliteRepository(api).BuildReadFilter(status, null, null), expectedId);
            AssertContainsStatus(new BeamRepository(api).BuildReadFilter(status, null, null, null), expectedId);
            AssertContainsStatus(new TransponderRepository(api).BuildReadFilter(status, null, null, null), expectedId);
        }

        [TestMethod]
        public void SatelliteReadFilter_CombinesDefinitionStatusHemisphereAndName()
        {
            var filter = new SatelliteRepository(CreateApi()).BuildReadFilter(
                InstanceStatus.Active,
                HemisphereType.Eastern,
                SatelliteExposers.SatelliteName.Equal("Sat A"));
            var values = Values(filter);

            CollectionAssert.IsSubsetOf(
                new object[] { Model.SlcSatellite_ManagementIds.Definitions.Satellites.Id, "active", Model.SlcSatellite_ManagementIds.Enums.Hemisphere.Eastern, "Sat A" },
                values);
        }

        [TestMethod]
        public void SatelliteReadFilter_WesternHemisphere_UsesGeneratedValue()
        {
            var filter = new SatelliteRepository(CreateApi()).BuildReadFilter(null, HemisphereType.Western, null);

            CollectionAssert.Contains(Values(filter), Model.SlcSatellite_ManagementIds.Enums.Hemisphere.Western);
        }

        [DataTestMethod]
        [DataRow(BeamLinkType.Feeder, "Feeder")]
        [DataRow(BeamLinkType.User, "User")]
        [DataRow(BeamLinkType.Uplink, "Uplink")]
        [DataRow(BeamLinkType.Downlink, "Downlink")]
        public void BeamReadFilter_MapsAllLinkTypes(BeamLinkType linkType, string expected)
        {
            var filter = new BeamRepository(CreateApi()).BuildReadFilter(null, null, linkType, null);

            CollectionAssert.Contains(Values(filter), expected);
        }

        [DataTestMethod]
        [DataRow(BeamTransmissionType.RX, "RX")]
        [DataRow(BeamTransmissionType.TX, "TX")]
        [DataRow(BeamTransmissionType.CarrierInCarrier, "Carrier in Carrier")]
        public void BeamReadFilter_MapsAllTransmissionTypes(BeamTransmissionType transmissionType, string expected)
        {
            var filter = new BeamRepository(CreateApi()).BuildReadFilter(null, transmissionType, null, null);

            CollectionAssert.Contains(Values(filter), expected);
        }

        [TestMethod]
        public void BeamReadFilter_CombinesStatusEnumsAndSatellite()
        {
            var satelliteId = Guid.NewGuid();
            var filter = new BeamRepository(CreateApi()).BuildReadFilter(
                InstanceStatus.Active,
                BeamTransmissionType.TX,
                BeamLinkType.Feeder,
                BeamExposers.BeamSatellite.Equal(satelliteId));

            CollectionAssert.IsSubsetOf(
                new object[] { Model.SlcSatellite_ManagementIds.Definitions.Beams.Id, "active", "TX", "Feeder", satelliteId },
                Values(filter));
        }

        [DataTestMethod]
        [DataRow(TransponderBandType.C, "C")]
        [DataRow(TransponderBandType.K, "K")]
        [DataRow(TransponderBandType.Ka, "Ka")]
        [DataRow(TransponderBandType.Ku, "Ku")]
        [DataRow(TransponderBandType.L, "L")]
        [DataRow(TransponderBandType.X, "X")]
        public void TransponderReadFilter_MapsAllBands(TransponderBandType band, string expected)
        {
            var filter = new TransponderRepository(CreateApi()).BuildReadFilter(null, null, band, null);

            CollectionAssert.Contains(Values(filter), expected);
        }

        [TestMethod]
        public void TransponderReadFilter_CombinesStatusEnumsAndName()
        {
            var filter = new TransponderRepository(CreateApi()).BuildReadFilter(
                InstanceStatus.Deprecated,
                TransponderPolarizationType.Circular,
                TransponderBandType.Ku,
                TransponderExposers.TransponderName.Equal("Ku 1"));

            CollectionAssert.IsSubsetOf(
                new object[] { Model.SlcSatellite_ManagementIds.Definitions.Transponders.Id, "deprecated", Model.SlcSatellite_ManagementIds.Enums.Polarization.Circular, "Ku", "Ku 1" },
                Values(filter));
        }

        [TestMethod]
        public void TransponderReadFilter_LinearPolarization_UsesGeneratedValue()
        {
            var filter = new TransponderRepository(CreateApi()).BuildReadFilter(null, TransponderPolarizationType.Linear, null, null);

            CollectionAssert.Contains(Values(filter), Model.SlcSatellite_ManagementIds.Enums.Polarization.Linear);
        }

        [TestMethod]
        public void ReadFiltered_EmptyApiFilter_ReturnsEmptyWithoutContactingDom()
        {
            var connection = new Mock<IConnection>();
            var api = new SatOpsApi(connection.Object);

            Assert.AreEqual(0, new SatelliteRepository(api).ReadFiltered(status: InstanceStatus.Active, filter: new FALSEFilterElement<SatelliteObject>()).Count());
            Assert.AreEqual(0, new BeamRepository(api).ReadFiltered(linkType: BeamLinkType.Feeder, filter: new FALSEFilterElement<BeamObject>()).Count());
            Assert.AreEqual(0, new TransponderRepository(api).ReadFiltered(band: TransponderBandType.Ku, filter: new FALSEFilterElement<TransponderObject>()).Count());

            connection.VerifyGet(c => c.IsShuttingDown, Times.Once);
            connection.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ReadFilters_InvalidEnumOrStatus_ThrowBeforeReading()
        {
            var api = CreateApi();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SatelliteRepository(api).ReadFiltered(hemisphere: (HemisphereType)999));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BeamRepository(api).ReadFiltered(linkType: (BeamLinkType)999));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BeamRepository(api).ReadFiltered(transmissionType: (BeamTransmissionType)999));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TransponderRepository(api).ReadFiltered(band: (TransponderBandType)999));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TransponderRepository(api).ReadFiltered(polarization: (TransponderPolarizationType)999));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SatelliteRepository(api).ReadFiltered(status: (InstanceStatus)999));
        }

        private static void AssertContainsStatus(FilterElement<DomInstance> filter, string expectedId)
        {
            var statusFilters = LeafFilters(filter)
                .Where(leaf => leaf.getFieldName().fieldName == DomInstanceExposers.StatusId.fieldName)
                .ToList();
            Assert.AreEqual(1, statusFilters.Count);
            Assert.AreEqual(expectedId, statusFilters[0].getValue());
        }

        private static object[] Values(FilterElement<DomInstance> filter)
        {
            return LeafFilters(filter).Select(leaf => leaf.getValue()).ToArray();
        }

        private static IEnumerable<ManagedFilterIdentifier> LeafFilters(FilterElement<DomInstance> filter)
        {
            if (filter is ANDFilterElement<DomInstance> and)
                return and.subFilters.SelectMany(LeafFilters);

            if (filter is ManagedFilterIdentifier leaf)
                return new[] { leaf };

            return Enumerable.Empty<ManagedFilterIdentifier>();
        }

        private static SatOpsApi CreateApi()
        {
            return new SatOpsApi(new Mock<IConnection>().Object);
        }
    }
}
