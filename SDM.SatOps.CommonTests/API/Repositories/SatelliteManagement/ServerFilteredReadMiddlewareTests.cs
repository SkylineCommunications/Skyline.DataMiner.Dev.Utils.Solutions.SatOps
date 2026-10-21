namespace Skyline.DataMiner.SDM.SatOps.CommonTests.API.Repositories.SatelliteManagement
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
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
    using System.Linq;
    using BeamObject = Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam.Beam;
    using SatelliteObject = Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite.Satellite;
    using TransponderObject = Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder.Transponder;

    [TestClass]
    public class ServerFilteredReadMiddlewareTests
    {
        [TestMethod]
        public void SatelliteReadFiltered_ForwardsAllCriteria()
        {
            var filter = SatelliteExposers.SatelliteName.Equal("Sat A");
            var satellites = new[] { new SatelliteObject() };
            var inner = new Mock<ISatelliteRepository>();
            inner.Setup(r => r.ReadFiltered(InstanceStatus.Active, HemisphereType.Western, filter)).Returns(satellites);

            var result = new SatelliteRepositoryMiddleware(inner.Object, null).ReadFiltered(InstanceStatus.Active, HemisphereType.Western, filter);

            CollectionAssert.AreEqual(satellites, result.ToArray());
            inner.Verify(r => r.ReadFiltered(InstanceStatus.Active, HemisphereType.Western, filter), Times.Once);
        }

        [TestMethod]
        public void BeamReadFiltered_ForwardsAllCriteria()
        {
            var filter = BeamExposers.BeamName.Equal("Beam A");
            var beams = new[] { new BeamObject() };
            var inner = new Mock<IBeamRepository>();
            inner.Setup(r => r.ReadFiltered(InstanceStatus.Active, BeamTransmissionType.TX, BeamLinkType.Feeder, filter)).Returns(beams);

            var result = new BeamRepositoryMiddleware(inner.Object, null).ReadFiltered(InstanceStatus.Active, BeamTransmissionType.TX, BeamLinkType.Feeder, filter);

            CollectionAssert.AreEqual(beams, result.ToArray());
            inner.Verify(r => r.ReadFiltered(InstanceStatus.Active, BeamTransmissionType.TX, BeamLinkType.Feeder, filter), Times.Once);
        }

        [TestMethod]
        public void TransponderReadFiltered_ForwardsAllCriteria()
        {
            var filter = TransponderExposers.TransponderName.Equal("Tp A");
            var transponders = new[] { new TransponderObject() };
            var inner = new Mock<ITransponderRepository>();
            inner.Setup(r => r.ReadFiltered(InstanceStatus.Deprecated, TransponderPolarizationType.Linear, TransponderBandType.Ku, filter)).Returns(transponders);

            var result = new TransponderRepositoryMiddleware(inner.Object, null).ReadFiltered(InstanceStatus.Deprecated, TransponderPolarizationType.Linear, TransponderBandType.Ku, filter);

            CollectionAssert.AreEqual(transponders, result.ToArray());
            inner.Verify(r => r.ReadFiltered(InstanceStatus.Deprecated, TransponderPolarizationType.Linear, TransponderBandType.Ku, filter), Times.Once);
        }
    }
}
