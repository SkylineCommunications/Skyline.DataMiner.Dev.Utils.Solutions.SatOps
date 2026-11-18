namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Solutions.SatOps.Common.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation;
    using System;
    using System.Reflection;

    /// <summary>
    /// Regression tests for the pre/post-roll mapping in
    /// <c>TransponderRangeReservationRepository.ApplyReservationTimes</c>.
    /// See release note for 0.0.9: freshly-built reservation jobs must have
    /// <c>PreRollStart == Start</c> and <c>PostRollEnd == End</c> (zero-length roll),
    /// otherwise MediaOps.Plan throws JobInvalidPreRollError / JobInvalidPostRollError.
    /// </summary>
    [TestClass]
    public class TransponderRangeReservationTimeMappingTests
    {
        private static readonly Assembly CommonAssembly = typeof(SatOpsApi).Assembly;

        [TestMethod]
        public void ApplyReservationTimes_WithoutExplicitRoll_MirrorsStartOntoPreRollAndEndOntoPostRoll()
        {
            var start = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var reservationType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", throwOnError: true);
            var reservation = CreateReservation(reservationType, start, end, preRoll: null, postRoll: null);

            var job = CreateEmptyJob();
            InvokeApplyReservationTimes(reservation, job);

            var jobStart = (DateTimeOffset)job.GetType().GetProperty("Start").GetValue(job);
            var jobEnd = (DateTimeOffset)job.GetType().GetProperty("End").GetValue(job);
            var jobPreRollStart = (DateTimeOffset)job.GetType().GetProperty("PreRollStart").GetValue(job);
            var jobPostRollEnd = (DateTimeOffset)job.GetType().GetProperty("PostRollEnd").GetValue(job);

            Assert.AreEqual(new DateTimeOffset(start), jobStart);
            Assert.AreEqual(new DateTimeOffset(end), jobEnd);
            Assert.AreEqual(jobStart, jobPreRollStart, "PreRollStart must default to Start when no explicit pre-roll is set.");
            Assert.AreEqual(jobEnd, jobPostRollEnd, "PostRollEnd must default to End when no explicit post-roll is set.");
        }

        [TestMethod]
        public void ApplyReservationTimes_WithExplicitRoll_UsesRollValues()
        {
            var start = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var preRoll = start.AddMinutes(-5);
            var postRoll = end.AddMinutes(5);

            var reservationType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", throwOnError: true);
            var reservation = CreateReservation(reservationType, start, end, preRoll, postRoll);

            var job = CreateEmptyJob();
            InvokeApplyReservationTimes(reservation, job);

            var jobPreRollStart = (DateTimeOffset)job.GetType().GetProperty("PreRollStart").GetValue(job);
            var jobPostRollEnd = (DateTimeOffset)job.GetType().GetProperty("PostRollEnd").GetValue(job);

            Assert.AreEqual(new DateTimeOffset(preRoll), jobPreRollStart);
            Assert.AreEqual(new DateTimeOffset(postRoll), jobPostRollEnd);
        }

        [DataTestMethod]
        [DataRow(0, 30)]
        [DataRow(0, -15)]
        [DataRow(5, 30)]
        [DataRow(5, -15)]
        public void SetTimeWindow_ResizingEnd_AlignsPostRollWithNewEnd(int rollMinutes, int endShiftMinutes)
        {
            var start = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var end = start.AddHours(1);
            var reservation = new TransponderRangeReservation
            {
                StartTime = start,
                EndTime = end,
                PreRollStart = start.AddMinutes(-rollMinutes),
                PostRollEnd = end.AddMinutes(rollMinutes),
            };

            reservation.SetTimeWindow(start, end.AddMinutes(endShiftMinutes));
            var job = new Skyline.DataMiner.Solutions.MediaOps.Plan.API.Job();
            TransponderRangeReservationRepository.ApplyReservationTimes(reservation, job);

            Assert.AreEqual(new DateTimeOffset(start), job.Start);
            Assert.AreEqual(new DateTimeOffset(end.AddMinutes(endShiftMinutes)), job.End);
            Assert.AreEqual(job.Start.AddMinutes(-rollMinutes), job.PreRollStart);
            Assert.AreEqual(job.End, job.PostRollEnd);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(30)]
        public void SetTimeWindow_UnchangedEnd_PreservesExistingPostRoll(int startShiftMinutes)
        {
            var start = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var end = start.AddHours(1);
            var reservation = new TransponderRangeReservation
            {
                StartTime = start,
                EndTime = end,
                PreRollStart = start.AddMinutes(-5),
                PostRollEnd = end.AddMinutes(10),
            };

            reservation.SetTimeWindow(start.AddMinutes(startShiftMinutes), end);

            Assert.AreEqual(start.AddMinutes(startShiftMinutes - 5), reservation.PreRollStart);
            Assert.AreEqual(end.AddMinutes(10), reservation.PostRollEnd);
        }

        [TestMethod]
        public void SetTimeWindow_ShortenThenExpand_DoesNotCreateUnintendedRolls()
        {
            var start = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var end = start.AddHours(1);
            var reservation = new TransponderRangeReservation
            {
                StartTime = start,
                EndTime = end,
                PreRollStart = start,
                PostRollEnd = end,
            };

            reservation.SetTimeWindow(start.AddMinutes(10), end.AddMinutes(-10));
            Assert.AreEqual(reservation.StartTime, reservation.PreRollStart);
            Assert.AreEqual(reservation.EndTime, reservation.PostRollEnd);

            reservation.SetTimeWindow(start.AddMinutes(-10), end.AddMinutes(10));
            Assert.AreEqual(reservation.StartTime, reservation.PreRollStart);
            Assert.AreEqual(reservation.EndTime, reservation.PostRollEnd);
        }

        [TestMethod]
        public void SetTimeWindow_UnspecifiedRolls_StillDefaultToZeroOnPersistence()
        {
            var start = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var reservation = new TransponderRangeReservation();

            reservation.SetTimeWindow(start, start.AddHours(1));
            var job = new Skyline.DataMiner.Solutions.MediaOps.Plan.API.Job();
            TransponderRangeReservationRepository.ApplyReservationTimes(reservation, job);

            Assert.IsNull(reservation.PreRollStart);
            Assert.AreEqual(reservation.EndTime, reservation.PostRollEnd);
            Assert.AreEqual(job.Start, job.PreRollStart);
            Assert.AreEqual(job.End, job.PostRollEnd);
        }

        [TestMethod]
        public void SetTimeWindow_ExplicitRollOverridesAfterResizing_ArePreservedOnPersistence()
        {
            var start = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var reservation = new TransponderRangeReservation
            {
                StartTime = start,
                EndTime = start.AddHours(1),
                PreRollStart = start.AddMinutes(-5),
                PostRollEnd = start.AddHours(1).AddMinutes(5),
            };

            reservation.SetTimeWindow(start.AddHours(1), start.AddHours(2));
            reservation.PreRollStart = reservation.StartTime.Value.AddMinutes(-10);
            reservation.PostRollEnd = reservation.EndTime.Value.AddMinutes(20);
            var job = new Skyline.DataMiner.Solutions.MediaOps.Plan.API.Job();
            TransponderRangeReservationRepository.ApplyReservationTimes(reservation, job);

            Assert.AreEqual(job.Start.AddMinutes(-10), job.PreRollStart);
            Assert.AreEqual(job.End.AddMinutes(20), job.PostRollEnd);
        }

        private static object CreateReservation(Type reservationType, DateTime start, DateTime end, DateTime? preRoll, DateTime? postRoll)
        {
            var reservation = Activator.CreateInstance(reservationType);

            reservationType.GetProperty("StartTime").SetValue(reservation, start);
            reservationType.GetProperty("EndTime").SetValue(reservation, end);
            if (preRoll.HasValue)
            {
                reservationType.GetProperty("PreRollStart").SetValue(reservation, preRoll.Value);
            }

            if (postRoll.HasValue)
            {
                reservationType.GetProperty("PostRollEnd").SetValue(reservation, postRoll.Value);
            }

            return reservation;
        }

        private static object CreateEmptyJob()
        {
            var jobType = Type.GetType("Skyline.DataMiner.Solutions.MediaOps.Plan.API.Job, Skyline.DataMiner.Solutions.MediaOps.Plan", throwOnError: false)
                ?? typeof(Skyline.DataMiner.Solutions.MediaOps.Plan.API.Job);
            return Activator.CreateInstance(jobType);
        }

        private static void InvokeApplyReservationTimes(object reservation, object job)
        {
            var repoType = CommonAssembly.GetType("Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservationRepository", throwOnError: true);
            var method = repoType.GetMethod("ApplyReservationTimes", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.IsNotNull(method, "TransponderRangeReservationRepository.ApplyReservationTimes helper is missing.");
            method.Invoke(null, new[] { reservation, job });
        }
    }
}
