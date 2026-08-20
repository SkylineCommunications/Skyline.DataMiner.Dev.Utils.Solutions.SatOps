namespace Skyline.DataMiner.SDM.SatOps.CommonTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.MediaOps.Plan.API;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.TransponderRangeReservation;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.TransponderRangeReservation;

    [TestClass]
    public class TransponderRangeReservationFilterTranslatorTests
    {
        [TestMethod]
        public void Translate_NullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => translator.Translate(null));
        }

        [TestMethod]
        public void Translate_AnyFilter_ReturnsTrueFilterElement()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();

            // Act
            var result = translator.Translate(TransponderRangeReservationExposers.ReservationName.Equal("abc"));

            // Assert
            Assert.IsInstanceOfType(result, typeof(TRUEFilterElement<Job>));
        }

        [TestMethod]
        public void ApplyClientSide_NullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(
                () => translator.ApplyClientSide(null, new List<TransponderRangeReservation>()));
        }

        [TestMethod]
        public void ApplyClientSide_NullReservations_ReturnsEmpty()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();

            // Act
            var result = translator.ApplyClientSide(new TRUEFilterElement<TransponderRangeReservation>(), null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void ApplyClientSide_TrueFilter_ReturnsAllReservations()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();
            var reservations = new[] { CreateReservation("a"), CreateReservation("b") };

            // Act
            var result = translator.ApplyClientSide(new TRUEFilterElement<TransponderRangeReservation>(), reservations).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void ApplyClientSide_FalseFilter_ReturnsNoReservations()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();
            var reservations = new[] { CreateReservation("a") };

            // Act
            var result = translator.ApplyClientSide(new FALSEFilterElement<TransponderRangeReservation>(), reservations).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ApplyClientSide_NotFalseFilter_ReturnsAllReservations()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();
            var reservations = new[] { CreateReservation("a"), CreateReservation("b") };

            // Act
            var result = translator.ApplyClientSide(
                new NOTFilterElement<TransponderRangeReservation>(new FALSEFilterElement<TransponderRangeReservation>()),
                reservations).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void ApplyClientSide_EmptyReservations_ReturnsEmpty()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();

            // Act
            var result = translator.ApplyClientSide(
                new TRUEFilterElement<TransponderRangeReservation>(),
                Enumerable.Empty<TransponderRangeReservation>()).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ApplyClientSide_ReservationIdFilter_ReturnsMatchingReservation()
        {
            // Arrange
            var translator = new TransponderRangeReservationFilterTranslator();
            var reservations = new[] { CreateReservation("a"), CreateReservation("b") };
            var expected = reservations[1];

            // Act
            var result = translator.ApplyClientSide(
                TransponderRangeReservationExposers.ReservationId.Equal(expected.Id),
                reservations).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expected.Id, result[0].Id);
        }

        private static TransponderRangeReservation CreateReservation(string name)
        {
            var reservation = new TransponderRangeReservation();
            reservation.Name = name;
            return reservation;
        }
    }
}
