namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private static readonly Assembly CommonAssembly = typeof(Skyline.DataMiner.SDM.SatOps.Common.API.SatOpsApi).Assembly;

        [TestMethod]
        public void ApplyReservationTimes_WithoutExplicitRoll_MirrorsStartOntoPreRollAndEndOntoPostRoll()
        {
            var start = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            var reservationType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", throwOnError: true);
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

            var reservationType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", throwOnError: true);
            var reservation = CreateReservation(reservationType, start, end, preRoll, postRoll);

            var job = CreateEmptyJob();
            InvokeApplyReservationTimes(reservation, job);

            var jobPreRollStart = (DateTimeOffset)job.GetType().GetProperty("PreRollStart").GetValue(job);
            var jobPostRollEnd = (DateTimeOffset)job.GetType().GetProperty("PostRollEnd").GetValue(job);

            Assert.AreEqual(new DateTimeOffset(preRoll), jobPreRollStart);
            Assert.AreEqual(new DateTimeOffset(postRoll), jobPostRollEnd);
        }

        private static object CreateReservation(Type reservationType, DateTime start, DateTime end, DateTime? preRoll, DateTime? postRoll)
        {
            var factory = reservationType.GetMethod("CreateNew", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var reservation = factory.Invoke(null, Array.Empty<object>());

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
            var repoType = CommonAssembly.GetType("Skyline.DataMiner.SDM.SatOps.Common.API.Repositories.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservationRepository", throwOnError: true);
            var method = repoType.GetMethod("ApplyReservationTimes", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.IsNotNull(method, "TransponderRangeReservationRepository.ApplyReservationTimes helper is missing.");
            method.Invoke(null, new[] { reservation, job });
        }
    }
}
