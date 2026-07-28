namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TransponderRangeReservationDomModelTests
    {
        private static readonly Type ReservationType = typeof(Common.API.SatOpsApi).Assembly.GetType(
            "Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation.TransponderRangeReservation", true);

        [TestMethod]
        public void ReservationModel_DoesNotExposeMediaOpsPlanTypes()
        {
            foreach (var property in ReservationType.GetProperties())
            {
                Assert.IsFalse(property.PropertyType.FullName.StartsWith("Skyline.DataMiner.Solutions.MediaOps.Plan", StringComparison.Ordinal));
            }
        }

        [TestMethod]
        public void NewReservation_PreservesAssignedValuesAndNormalizesTimesToUtc()
        {
            var factory = ReservationType.GetMethod("CreateNewTransponderRangeReservation", BindingFlags.Static | BindingFlags.NonPublic);
            var reservation = factory.Invoke(null, Array.Empty<object>());
            var transponderId = Guid.NewGuid();
            var localStart = DateTime.SpecifyKind(new DateTime(2026, 7, 28, 8, 0, 0), DateTimeKind.Local);

            ReservationType.GetProperty("Name").SetValue(reservation, "Slot A");
            ReservationType.GetProperty("Transponder").SetValue(reservation, (Guid?)transponderId);
            ReservationType.GetProperty("StartTime").SetValue(reservation, (DateTime?)localStart);

            Assert.AreEqual("Slot A", ReservationType.GetProperty("Name").GetValue(reservation));
            Assert.AreEqual(transponderId, ReservationType.GetProperty("Transponder").GetValue(reservation));
            Assert.AreEqual(DateTimeKind.Utc, ((DateTime?)ReservationType.GetProperty("StartTime").GetValue(reservation)).Value.Kind);
        }
    }
}