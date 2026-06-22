using System;

namespace Skyline.DataMiner.SDM.SatOps.Common.Models
{
    /// <summary>
    /// Represents a beam entity associated with a satellite, including its unique identifier, link type, transmission type, and footprint information.
    /// </summary>
    // [GenerateExposers]
    // [SdmDomStorage("(slc)satellite_management")]
    public class Beam : SdmObject<Beam>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the beam.
        /// </summary>
        public Guid BeamId { get; set; }

        /// <summary>
        /// Gets or sets the reference to the satellite associated with this beam.
        /// </summary>
        public SdmObjectReference<Satellite> SatelliteId { get; set; }

        /// <summary>
        /// Gets or sets the link type of the beam.
        /// </summary>
        public SharedMappers.Satellite_Management.SlcSatellite_ManagementIds.Enums.LinktypeEnum LinkType { get; set; }

        /// <summary>
        /// Gets or sets the transmission type of the beam.
        /// </summary>
        public SharedMappers.Satellite_Management.SlcSatellite_ManagementIds.Enums.TransmissiontypeEnum TransmissionType { get; set; }

        /// <summary>
        /// Gets or sets the footprint file path or reference for this beam.
        /// </summary>
        public string FootprintFile { get; set; }
    }
}
