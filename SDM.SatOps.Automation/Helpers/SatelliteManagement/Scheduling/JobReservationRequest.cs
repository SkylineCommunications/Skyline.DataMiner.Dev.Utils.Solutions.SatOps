namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement.Scheduling
{
	using System;

	/// <summary>
	/// Represents the input required to create or update a transponder job reservation.
	/// Mirrors the values that the legacy Job Reservation dialog collected before delegating
	/// to the Job Handler and Configuration Handler.
	/// </summary>
	public class JobReservationRequest
	{
		/// <summary>
		/// Gets or sets the unique identifier of the job to update.
		/// Ignored when creating a new reservation.
		/// </summary>
		public Guid JobId { get; set; }

		/// <summary>
		/// Gets or sets the name of the job reservation.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the reservation start time.
		/// </summary>
		public DateTime StartTime { get; set; }

		/// <summary>
		/// Gets or sets the reservation end time.
		/// </summary>
		public DateTime EndTime { get; set; }

		/// <summary>
		/// Gets or sets the relative start frequency to reserve.
		/// </summary>
		public decimal RelativeStartFrequency { get; set; }

		/// <summary>
		/// Gets or sets the relative end frequency to reserve.
		/// </summary>
		public decimal RelativeEndFrequency { get; set; }

		/// <summary>
		/// Gets or sets the DOM resource identifier of the transponder to book.
		/// </summary>
		public Guid TransponderResourceId { get; set; }

		/// <summary>
		/// Gets or sets the satellite capability discrete value, or <c>null</c>/empty to skip setting it.
		/// </summary>
		public string SatelliteName { get; set; }

		/// <summary>
		/// Gets or sets the slot name property value to store for the reservation.
		/// </summary>
		public string SlotName { get; set; }
	}
}
