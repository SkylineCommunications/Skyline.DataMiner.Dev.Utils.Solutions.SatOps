namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.Exceptions;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Satellite;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation;
    using Reservation = Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation;

    [TestClass]
    public class TransponderRangeReservationLifecycleTests
    {
        [DataTestMethod]
        [DataRow(JobState.Confirmed, TransponderReservationState.Confirmed)]
        [DataRow(JobState.Running, TransponderReservationState.Active)]
        [DataRow(JobState.Completed, TransponderReservationState.Completed)]
        public void Create_ConfirmsAfterTentativeAndReturnsReachedState(JobState reachedState, TransponderReservationState expectedState)
        {
            var fixture = new Fixture();
            var reservation = fixture.CreateReservation();
            Job persisted = null;
            var sequence = new MockSequence();
            fixture.Jobs.InSequence(sequence).Setup(value => value.Read(reservation.Id)).Returns((Job)null);
            fixture.Jobs.InSequence(sequence).Setup(value => value.Create(It.IsAny<Job>()))
                .Returns<Job>(job => persisted = job);
            fixture.Jobs.InSequence(sequence).Setup(value => value.SaveAsTentative(reservation.Id))
                .Returns(() => WithState(persisted, JobState.Tentative));
            fixture.Jobs.InSequence(sequence).Setup(value => value.Confirm(reservation.Id))
                .Returns(() => WithState(persisted, reachedState));

            var result = fixture.Repository.Create(reservation);

            Assert.AreEqual(reservation.Id, result.Id);
            Assert.AreEqual(expectedState, result.State);
            Assert.AreEqual(reservation.PreRollStart, result.PreRollStart);
            Assert.AreEqual(reservation.PostRollEnd, result.PostRollEnd);
            fixture.Jobs.Verify(value => value.SaveAsTentative(reservation.Id), Times.Once);
            fixture.Jobs.Verify(value => value.Confirm(reservation.Id), Times.Once);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Create_TransitionRejected_ReportsPersistedIdStageAndLastKnownState(bool confirm)
        {
            var fixture = new Fixture();
            var reservation = fixture.CreateReservation();
            Job persisted = null;
            var failure = new MediaOpsException("Transition is not allowed.");
            fixture.Jobs.Setup(value => value.Read(reservation.Id)).Returns((Job)null);
            fixture.Jobs.Setup(value => value.Create(It.IsAny<Job>())).Returns<Job>(job => persisted = job);
            if (confirm)
            {
                fixture.Jobs.Setup(value => value.SaveAsTentative(reservation.Id))
                    .Returns(() => WithState(persisted, JobState.Tentative));
                fixture.Jobs.Setup(value => value.Confirm(reservation.Id)).Throws(failure);
            }
            else
            {
                fixture.Jobs.Setup(value => value.SaveAsTentative(reservation.Id)).Throws(failure);
            }

            var exception = Assert.ThrowsException<ReservationTransitionException>(() => fixture.Repository.Create(reservation));

            Assert.AreEqual(reservation.Id, exception.ReservationId);
            Assert.AreEqual(confirm ? TransponderReservationState.Tentative : TransponderReservationState.Draft, exception.LastKnownState);
            Assert.AreEqual(confirm ? "Confirm" : "SaveAsTentative", exception.Operation);
            Assert.AreSame(failure, exception.InnerException);
            StringAssert.Contains(exception.Message, failure.Message);
            StringAssert.Contains(exception.Message, reservation.Id.ToString());
            fixture.Jobs.Verify(value => value.Delete(It.IsAny<Guid>()), Times.Never);
            fixture.Jobs.Verify(value => value.Cancel(It.IsAny<Guid>()), Times.Never);
            fixture.Jobs.Verify(value => value.Confirm(reservation.Id), confirm ? Times.Once() : Times.Never());
        }

        [DataTestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public void Create_TransitionReturnsMissingOrUnexpectedState_DoesNotReportSuccess(bool confirm, bool missing)
        {
            var fixture = new Fixture();
            var reservation = fixture.CreateReservation();
            Job persisted = null;
            fixture.Jobs.Setup(value => value.Read(reservation.Id)).Returns((Job)null);
            fixture.Jobs.Setup(value => value.Create(It.IsAny<Job>())).Returns<Job>(job => persisted = job);
            fixture.Jobs.Setup(value => value.SaveAsTentative(reservation.Id))
                .Returns(() => confirm
                    ? WithState(persisted, JobState.Tentative)
                    : missing ? null : persisted);
            if (confirm)
            {
                fixture.Jobs.Setup(value => value.Confirm(reservation.Id)).Returns(() => missing ? null : persisted);
            }

            var exception = Assert.ThrowsException<ReservationTransitionException>(() => fixture.Repository.Create(reservation));

            Assert.AreEqual(reservation.Id, exception.ReservationId);
            Assert.AreEqual(confirm ? "Confirm" : "SaveAsTentative", exception.Operation);
            StringAssert.Contains(exception.Message, "expected state");
        }

        [DataTestMethod]
        [DataRow(JobState.Draft)]
        [DataRow(JobState.Tentative)]
        [DataRow(JobState.Confirmed)]
        public void ReserveRange_ConfirmsOnlyRecoveredDraftJobs(JobState initialState)
        {
            var fixture = new Fixture();
            Job persisted = null;
            fixture.Jobs.Setup(value => value.Read(It.IsAny<Guid>())).Returns((Job)null);
            fixture.Jobs.Setup(value => value.Create(It.IsAny<Job>())).Returns<Job>(job => persisted = job);
            fixture.Jobs.Setup(value => value.SaveAsTentative(It.IsAny<Guid>()))
                .Returns(() => WithState(persisted, JobState.Tentative));
            fixture.Jobs.Setup(value => value.Confirm(It.IsAny<Guid>()))
                .Returns(() => WithState(persisted, JobState.Confirmed));
            var reservation = fixture.Repository.Create(fixture.CreateReservation());
            WithState(persisted, initialState);
            fixture.Jobs.Invocations.Clear();
            fixture.Jobs.Setup(value => value.Read(reservation.Id)).Returns(() => persisted);
            fixture.Jobs.Setup(value => value.Update(It.IsAny<Job>())).Returns<Job>(job => job);

            fixture.Repository.ReserveRange(reservation.Id, 10d, 15d);

            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Once);
            fixture.Jobs.Verify(value => value.SaveAsTentative(reservation.Id), initialState == JobState.Draft ? Times.Once() : Times.Never());
            fixture.Jobs.Verify(value => value.Confirm(reservation.Id), initialState == JobState.Draft ? Times.Once() : Times.Never());
            Assert.AreEqual(initialState == JobState.Draft ? JobState.Confirmed : initialState, persisted.State);
        }

        [DataTestMethod]
        [DataRow("Name")]
        [DataRow("Start")]
        [DataRow("End")]
        [DataRow("PreRollStart")]
        [DataRow("PostRollEnd")]
        public void Update_ActiveNonAllocationChanges_AreDelegatedToMediaOps(string field)
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);
            var node = fixture.PersistedJob.NodeGraph.Nodes.OfType<JobResourceNode>().Single();
            var range = node.OrchestrationSettings.Capacities.Single();
            var bandwidth = node.OrchestrationSettings.Configurations.Single();
            var satellite = node.OrchestrationSettings.Capabilities.Single();
            switch (field)
            {
                case "Name": reservation.Name = "Renamed"; break;
                case "Start": reservation.StartTime = reservation.StartTime.Value.AddMinutes(1); break;
                case "End": reservation.EndTime = reservation.EndTime.Value.AddMinutes(1); break;
                case "PreRollStart": reservation.PreRollStart = reservation.PreRollStart.Value.AddMinutes(1); break;
                case "PostRollEnd": reservation.PostRollEnd = reservation.PostRollEnd.Value.AddMinutes(1); break;
            }

            var result = fixture.Repository.Update(reservation);

            Assert.AreEqual(TransponderReservationState.Active, result.State);
            Assert.AreEqual(reservation.Name, result.Name);
            Assert.AreEqual(reservation.StartTime, result.StartTime);
            Assert.AreEqual(reservation.EndTime, result.EndTime);
            Assert.AreEqual(reservation.PreRollStart, result.PreRollStart);
            Assert.AreEqual(reservation.PostRollEnd, result.PostRollEnd);
            Assert.AreSame(range, node.OrchestrationSettings.Capacities.Single());
            Assert.AreSame(bandwidth, node.OrchestrationSettings.Configurations.Single());
            Assert.AreSame(satellite, node.OrchestrationSettings.Capabilities.Single());
            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Once);
        }

        [TestMethod]
        public void Update_ActiveTransponderSwapWithSameAllocation_IsDelegated()
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);
            var target = fixture.AddTransponder("SES-10");
            reservation.Transponder = target.Id;

            var result = fixture.Repository.Update(reservation);

            Assert.AreEqual(target.Id, result.Transponder);
            Assert.AreEqual(target.DOMResource, fixture.PersistedJob.NodeGraph.Nodes.OfType<JobResourceNode>().Single().ResourceId);
            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Once);
        }

        [DataTestMethod]
        [DataRow("Range", ReservationOperationErrorCode.FrequencyRangeChangeNotAllowed)]
        [DataRow("Bandwidth", ReservationOperationErrorCode.BandwidthChangeNotAllowed)]
        [DataRow("Satellite", ReservationOperationErrorCode.SatelliteCapabilityChangeNotAllowed)]
        public void Update_ActiveAllocationChange_IsRejectedBeforeMutatingJob(string change, ReservationOperationErrorCode expectedCode)
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);
            var job = fixture.PersistedJob;
            var originalName = job.Name;
            reservation.Name = "Must not be applied";
            if (change == "Range")
            {
                reservation.RelativeStartFrequency += 1d;
                reservation.RelativeEndFrequency += 1d;
            }
            else if (change == "Bandwidth")
            {
                job.NodeGraph.Nodes.OfType<JobResourceNode>().Single().OrchestrationSettings.Configurations
                    .OfType<NumberConfigurationSetting>().Single(setting => setting.Id == PredefinedGuids.BandwidthSizeGuid).Value = 3m;
            }
            else
            {
                reservation.Transponder = fixture.AddTransponder("Other satellite").Id;
            }

            var exception = Assert.ThrowsException<ReservationOperationException>(() => fixture.Repository.Update(reservation));

            Assert.AreEqual(reservation.Id, exception.ReservationId);
            Assert.AreEqual(TransponderReservationState.Active, exception.LastKnownState);
            Assert.AreEqual(expectedCode, exception.Errors.Single().Code);
            StringAssert.Contains(exception.Message, "Active");
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(originalName, job.Name);
            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ReserveRange_ActiveUnchangedAllocation_IsDelegated(bool withSatellite)
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);

            if (withSatellite)
                fixture.Repository.ReserveRange(reservation.Id, 5d, 9d, "SES-10");
            else
                fixture.Repository.ReserveRange(reservation.Id, 5d, 9d);

            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Once);
            fixture.Jobs.Verify(value => value.Confirm(It.IsAny<Guid>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ReserveRange_ActiveChangedRangeAndBandwidth_IsRejected(bool withSatellite)
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);

            var exception = Assert.ThrowsException<ReservationOperationException>(() =>
            {
                if (withSatellite)
                    fixture.Repository.ReserveRange(reservation.Id, 5d, 10d, "SES-10");
                else
                    fixture.Repository.ReserveRange(reservation.Id, 5d, 10d);
            });

            CollectionAssert.AreEquivalent(
                new[] { ReservationOperationErrorCode.FrequencyRangeChangeNotAllowed, ReservationOperationErrorCode.BandwidthChangeNotAllowed },
                exception.Errors.Select(error => error.Code).ToArray());
            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow("Other satellite")]
        [DataRow(null)]
        public void ReserveRange_ActiveSatelliteChange_IsRejected(string satelliteName)
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);

            var exception = Assert.ThrowsException<ReservationOperationException>(() =>
                fixture.Repository.ReserveRange(reservation.Id, 5d, 9d, satelliteName));

            Assert.AreEqual(ReservationOperationErrorCode.SatelliteCapabilityChangeNotAllowed, exception.Errors.Single().Code);
            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(ReservationOperationErrorCode.StartTimeChangeNotAllowed)]
        [DataRow(ReservationOperationErrorCode.ResourceReassignmentNotAllowed)]
        [DataRow(ReservationOperationErrorCode.InvalidBookingState)]
        public void Update_MediaOpsLifecycleRejection_ReturnsSatOpsReasonAndOriginalFailure(ReservationOperationErrorCode expectedCode)
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);
            MediaOpsErrorData error;
            if (expectedCode == ReservationOperationErrorCode.StartTimeChangeNotAllowed)
                error = new JobStartChangedNotAllowedError();
            else if (expectedCode == ReservationOperationErrorCode.ResourceReassignmentNotAllowed)
                error = new JobNodeSwappedInPostRollNotAllowedError();
            else
                error = new JobInvalidStateError();
            typeof(MediaOpsErrorData).GetProperty(nameof(MediaOpsErrorData.ErrorMessage))
                .SetValue(error, "The requested operation is not supported in the current lifecycle state.");
            var failure = new MediaOpsException(error);
            fixture.Jobs.Setup(value => value.Update(It.IsAny<Job>())).Throws(failure);

            var exception = Assert.ThrowsException<ReservationOperationException>(() => fixture.Repository.Update(reservation));

            Assert.AreEqual(expectedCode, exception.Errors.Single().Code);
            Assert.AreEqual(TransponderReservationState.Active, exception.LastKnownState);
            Assert.AreSame(failure, exception.InnerException);
            StringAssert.Contains(exception.Message, failure.Message);
            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Once);
        }

        [TestMethod]
        public void Update_MediaOpsFailureWithoutLifecycleErrors_IsNotDisguisedAsStateRejection()
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);
            var failure = new MediaOpsException("Resource is unavailable.");
            fixture.Jobs.Setup(value => value.Update(It.IsAny<Job>())).Throws(failure);

            var actual = Assert.ThrowsException<MediaOpsException>(() => fixture.Repository.Update(reservation));

            Assert.AreSame(failure, actual);
        }

        [DataTestMethod]
        [DataRow(JobState.Completed, false)]
        [DataRow(JobState.Canceled, false)]
        [DataRow(JobState.Completed, true)]
        [DataRow(JobState.Canceled, true)]
        public void Mutation_CompletedOrCanceled_IsRejectedByMediaOpsNotByLocalGuard(JobState state, bool reserveRange)
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(state);
            var error = new JobInvalidStateError();
            typeof(MediaOpsErrorData).GetProperty(nameof(MediaOpsErrorData.ErrorMessage))
                .SetValue(error, "Completed or canceled bookings cannot be updated.");
            var failure = new MediaOpsException(error);
            fixture.Jobs.Setup(value => value.Update(It.IsAny<Job>())).Throws(failure);

            var exception = Assert.ThrowsException<ReservationOperationException>(() =>
            {
                if (reserveRange)
                    fixture.Repository.ReserveRange(reservation.Id, 5d, 10d);
                else
                    fixture.Repository.Update(reservation);
            });

            Assert.AreEqual(Reservation.MapState(state), exception.LastKnownState);
            Assert.AreEqual(reserveRange ? "ReserveRange" : "Update", exception.Operation);
            Assert.AreSame(failure, exception.InnerException);
            fixture.Jobs.Verify(value => value.Update(It.IsAny<Job>()), Times.Once);
        }

        [TestMethod]
        public void Update_MixedLifecycleAndValidationErrors_PreservesEveryReason()
        {
            var fixture = new Fixture();
            var reservation = fixture.SeedReservation(JobState.Running);
            var timing = new JobStartChangedNotAllowedError();
            var name = new JobInvalidNameError();
            typeof(MediaOpsErrorData).GetProperty(nameof(MediaOpsErrorData.ErrorMessage))
                .SetValue(timing, "The start has already occurred.");
            typeof(MediaOpsErrorData).GetProperty(nameof(MediaOpsErrorData.ErrorMessage))
                .SetValue(name, "The booking name is invalid.");
            var trace = new MediaOpsTraceData();
            trace.ErrorData.Add(timing);
            trace.ErrorData.Add(name);
            var failure = new MediaOpsException(trace);
            fixture.Jobs.Setup(value => value.Update(It.IsAny<Job>())).Throws(failure);

            var exception = Assert.ThrowsException<ReservationOperationException>(() => fixture.Repository.Update(reservation));

            CollectionAssert.AreEquivalent(
                new[] { ReservationOperationErrorCode.StartTimeChangeNotAllowed, ReservationOperationErrorCode.MediaOpsValidationFailure },
                exception.Errors.Select(error => error.Code).ToArray());
            StringAssert.Contains(exception.Message, timing.ErrorMessage);
            StringAssert.Contains(exception.Message, name.ErrorMessage);
            Assert.AreSame(failure, exception.InnerException);
        }

        private static Job WithState(Job job, JobState state)
        {
            typeof(Job).GetProperty(nameof(Job.State)).SetValue(job, state);
            return job;
        }

        private sealed class Fixture
        {
            private readonly Transponder transponder;
            private readonly Mock<ITransponderRepository> transponders;
            private readonly Mock<ISatelliteRepository> satellites;

            public Fixture()
            {
                Jobs = new Mock<IJobsRepository>(MockBehavior.Strict);
                var plan = new Mock<IMediaOpsPlanApi>();
                plan.Setup(value => value.Jobs).Returns(Jobs.Object);
                var satellite = new Satellite { Name = "SES-10" };
                transponder = new Transponder { DOMResource = Guid.NewGuid(), TransponderSatellite = satellite.Id };
                transponders = new Mock<ITransponderRepository>(MockBehavior.Strict);
                transponders.Setup(value => value.Read(transponder.Id)).Returns(transponder);
                transponders.Setup(value => value.Read(It.IsAny<FilterElement<Transponder>>())).Returns(new[] { transponder });
                satellites = new Mock<ISatelliteRepository>(MockBehavior.Strict);
                satellites.Setup(value => value.Read(satellite.Id)).Returns(satellite);
                var connection = new Mock<IConnection>(MockBehavior.Strict);
                connection.SetupGet(value => value.IsShuttingDown).Returns(false);
                connection.Setup(value => value.HandleMessages(It.IsAny<DMSMessage[]>()))
                    .Returns(new DMSMessage[] { new ManagerStoreCrudResponse<DomInstance>() });
                var api = new SatOpsApi(connection.Object);
                typeof(SatOpsApi).GetField("lazyMediaOpsPlanApi", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(api, new Lazy<IMediaOpsPlanApi>(() => plan.Object));
                typeof(SatOpsApi).GetField("lazyTransponderRepository", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(api, new Lazy<ITransponderRepository>(() => transponders.Object));
                typeof(SatOpsApi).GetField("lazySatelliteRepository", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(api, new Lazy<ISatelliteRepository>(() => satellites.Object));
                Repository = new TransponderRangeReservationRepository(api);
            }

            public Mock<IJobsRepository> Jobs { get; }

            public TransponderRangeReservationRepository Repository { get; }

            public Job PersistedJob { get; private set; }

            public Reservation SeedReservation(JobState state)
            {
                Jobs.Setup(value => value.Read(It.IsAny<Guid>())).Returns((Job)null);
                Jobs.Setup(value => value.Create(It.IsAny<Job>())).Returns<Job>(job => PersistedJob = job);
                Jobs.Setup(value => value.SaveAsTentative(It.IsAny<Guid>()))
                    .Returns(() => WithState(PersistedJob, JobState.Tentative));
                Jobs.Setup(value => value.Confirm(It.IsAny<Guid>()))
                    .Returns(() => WithState(PersistedJob, JobState.Confirmed));
                var reservation = Repository.Create(CreateReservation());
                WithState(PersistedJob, state);
                Jobs.Invocations.Clear();
                Jobs.Setup(value => value.Read(reservation.Id)).Returns(() => PersistedJob);
                Jobs.Setup(value => value.Update(It.IsAny<Job>())).Returns<Job>(job => PersistedJob = job);
                return reservation;
            }

            public Transponder AddTransponder(string satelliteName)
            {
                var satellite = new Satellite { Name = satelliteName };
                var target = new Transponder { DOMResource = Guid.NewGuid(), TransponderSatellite = satellite.Id };
                satellites.Setup(value => value.Read(satellite.Id)).Returns(satellite);
                transponders.Setup(value => value.Read(target.Id)).Returns(target);
                transponders.Setup(value => value.Read(It.IsAny<FilterElement<Transponder>>())).Returns(new[] { target });
                return target;
            }

            public Reservation CreateReservation()
            {
                var start = new DateTime(2030, 1, 1, 10, 0, 0, DateTimeKind.Utc);
                return new Reservation
                {
                    Name = "Booking",
                    Transponder = transponder.Id,
                    StartTime = start,
                    EndTime = start.AddHours(1),
                    PreRollStart = start.AddMinutes(-5),
                    PostRollEnd = start.AddHours(1).AddMinutes(5),
                    RelativeStartFrequency = 5d,
                    RelativeEndFrequency = 9d,
                };
            }
        }
    }
}
