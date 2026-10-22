namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation;

    [TestClass]
    public class TransponderRangeReservationDeletionTests
    {
        [DataTestMethod]
        [DataRow(JobState.Tentative)]
        [DataRow(JobState.Confirmed)]
        public void Delete_ScheduledReservation_CancelsBeforeDeleting(JobState state)
        {
            var job = CreateJob(state);
            var jobs = new Mock<IJobsRepository>(MockBehavior.Strict);
            var sequence = new MockSequence();
            jobs.InSequence(sequence).Setup(repository => repository.Read(job.Id)).Returns(job);
            jobs.InSequence(sequence).Setup(repository => repository.Cancel(job.Id)).Returns(job);
            jobs.InSequence(sequence).Setup(repository => repository.Delete(job.Id));

            CreateRepository(jobs.Object).Delete(job.Id);

            jobs.Verify(repository => repository.Cancel(job.Id), Times.Once);
            jobs.Verify(repository => repository.Delete(job.Id), Times.Once);
        }

        [DataTestMethod]
        [DataRow(JobState.Draft)]
        [DataRow(JobState.Canceled)]
        public void Delete_UnscheduledReservation_DeletesWithoutCancel(JobState state)
        {
            var job = CreateJob(state);
            var jobs = new Mock<IJobsRepository>();
            jobs.Setup(repository => repository.Read(job.Id)).Returns(job);

            CreateRepository(jobs.Object).Delete(job.Id);

            jobs.Verify(repository => repository.Cancel(It.IsAny<Guid>()), Times.Never);
            jobs.Verify(repository => repository.Delete(job.Id), Times.Once);
        }

        [DataTestMethod]
        [DataRow(JobState.Running)]
        [DataRow(JobState.Completed)]
        public void Delete_ImmutableReservation_DoesNotCancelOrDelete(JobState state)
        {
            var job = CreateJob(state);
            var jobs = new Mock<IJobsRepository>();
            jobs.Setup(repository => repository.Read(job.Id)).Returns(job);

            Assert.ThrowsException<InvalidOperationException>(() => CreateRepository(jobs.Object).Delete(job.Id));

            jobs.Verify(repository => repository.Cancel(It.IsAny<Guid>()), Times.Never);
            jobs.Verify(repository => repository.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void Delete_CancelFailure_DoesNotAttemptDelete()
        {
            var job = CreateJob(JobState.Tentative);
            var jobs = new Mock<IJobsRepository>();
            jobs.Setup(repository => repository.Read(job.Id)).Returns(job);
            var failure = new InvalidOperationException("Cancellation failed");
            jobs.Setup(repository => repository.Cancel(job.Id)).Throws(failure);

            Assert.AreSame(failure, Assert.ThrowsException<InvalidOperationException>(() => CreateRepository(jobs.Object).Delete(job.Id)));

            jobs.Verify(repository => repository.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void Delete_MissingOrUnrelatedJob_DoesNotCancelOrDelete()
        {
            var jobs = new Mock<IJobsRepository>();
            var repository = CreateRepository(jobs.Object);
            repository.Delete(Guid.NewGuid());
            var unrelated = new Job();
            jobs.Setup(value => value.Read(unrelated.Id)).Returns(unrelated);

            repository.Delete(unrelated.Id);

            jobs.Verify(value => value.Cancel(It.IsAny<Guid>()), Times.Never);
            jobs.Verify(value => value.Delete(It.IsAny<Guid>()), Times.Never);
        }

        private static Job CreateJob(JobState state)
        {
            var job = new Job();
            typeof(Job).GetProperty(nameof(Job.State)).SetValue(job, state);
            job.NodeGraph.Add(new JobResourceNode(PredefinedGuids.TransponderResourcePoolGuid, Guid.NewGuid()));
            return job;
        }

        private static TransponderRangeReservationRepository CreateRepository(IJobsRepository jobs)
        {
            var plan = new Mock<IMediaOpsPlanApi>();
            plan.Setup(value => value.Jobs).Returns(jobs);
            var api = new SatOpsApi(new Mock<IConnection>().Object);
            typeof(SatOpsApi).GetField("lazyMediaOpsPlanApi", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(api, new Lazy<IMediaOpsPlanApi>(() => plan.Object));
            return new TransponderRangeReservationRepository(api);
        }
    }
}
