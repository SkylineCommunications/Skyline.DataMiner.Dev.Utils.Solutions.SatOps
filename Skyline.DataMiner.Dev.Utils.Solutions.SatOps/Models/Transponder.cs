namespace Skyline.DataMiner.SDM.SatOps.Common.Models
{
    using System;

    // [GenerateExposers]
    // [SdmDomStorage("(slc)satellite_management")]
    /// <summary>
    /// Represents a satellite transponder configuration.
    /// </summary>
    public class Transponder : SdmObject<Transponder>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the transponder.
        /// </summary>
        public Guid TransponderId { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent satellite.
        /// </summary>
        public SdmObjectReference<Satellite> Satellite { get; set; }

        /// <summary>
        /// Gets or sets the reference to the beam associated with this transponder.
        /// </summary>
        public SdmObjectReference<Beam> Beam { get; set; }

        /// <summary>
        /// Gets or sets the name of the transponder.
        /// </summary>
        public string TransponderName { get; set; }

        /// <summary>
        /// Gets or sets the frequency band used by the transponder.
        /// </summary>
        public SharedMappers.Satellite_Management.SlcSatellite_ManagementIds.Enums.BandEnum Band { get; set; }

        /// <summary>
        /// Gets or sets the bandwidth in MHz.
        /// </summary>
        public double Bandwidth { get; set; }

        /// <summary>
        /// Gets or sets the start frequency in MHz.
        /// </summary>
        public double StartFrequency { get; set; }

        /// <summary>
        /// Gets or sets the stop frequency in MHz.
        /// </summary>
        public double StopFrequency { get; set; }

        /// <summary>
        /// Gets or sets the polarization type for the transponder.
        /// </summary>
        public SharedMappers.Satellite_Management.SlcSatellite_ManagementIds.Enums.PolarizationEnum Polarization { get; set; }

        /// <summary>
        /// Gets or sets the downlink start frequency in MHz.
        /// </summary>
        public double DownnlinkStartFrequency { get; set; }

        /// <summary>
        /// Gets or sets the downlink stop frequency in MHz.
        /// </summary>
        public double DownlinkStopFrequency { get; set; }

        /// <summary>
        /// Gets or sets the rolling window duration for the transponder.
        /// </summary>
        public double RollingWindow { get; set; }

        /// <summary>
        /// Gets or sets the hard end date for the transponder availability.
        /// </summary>
        public DateTime HardEndDate { get; set; }

        /// <summary>
        /// Gets or sets the contact phone number associated with the transponder.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the uplink polarization type.
        /// </summary>
        public SharedMappers.Satellite_Management.SlcSatellite_ManagementIds.Enums.UppolarizationEnum UplinkPolarization { get; set; }

        /// <summary>
        /// Gets or sets the downlink polarization type.
        /// </summary>
        public SharedMappers.Satellite_Management.SlcSatellite_ManagementIds.Enums.DownpolarizationEnum DownlinkPolarization { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier for the transponder.
        /// </summary>
        public Guid ResourceId { get; set; }
    }
}
