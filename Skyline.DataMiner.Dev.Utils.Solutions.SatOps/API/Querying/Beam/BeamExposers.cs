namespace Skyline.DataMiner.SDM.SatOps.Common.API.Querying.Beam
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement.Beam;
    using System;

    /// <summary>
    /// Provides exposers for querying <see cref="Beam"/> objects.
    /// </summary>
    /// <remarks>
    /// Exposers define how properties of <see cref="Beam"/> objects are exposed for use in queries.
    /// </remarks>
    public static class BeamExposers
    {
        /// <summary>
        /// Exposes the <see cref="Beam.Name"/> property for querying as "BeamName".
        /// </summary>
        public static readonly Exposer<Beam, string> BeamName = new Exposer<Beam, string>((obj) => obj.Name, "BeamName");

        /// <summary>
        /// Exposes the <see cref="Beam.BeamSatellite"/> property for querying.
        /// </summary>
        public static readonly Exposer<Beam, Guid> BeamSatellite = new Exposer<Beam, Guid>((obj) => obj.BeamSatellite.Value, "BeamSatellite");

        /// <summary>
        /// Exposes the <see cref="Beam.LinkType"/> property for querying as "LinkType".
        /// </summary>
        public static readonly Exposer<Beam, BeamLinkType> BeamLinkType = new Exposer<Beam, BeamLinkType>((obj) => obj.LinkType.Value, "LinkType");

        /// <summary>
        /// Exposes the <see cref="Beam.TransmissionType"/> property for querying as "TransmissionType".
        /// </summary>
        public static readonly Exposer<Beam, BeamTransmissionType> BeamTransmissionType = new Exposer<Beam, BeamTransmissionType>((obj) => obj.TransmissionType.Value, "TransmissionType");

        /// <summary>
        /// Exposes the <see cref="Beam.FootprintFile"/> property for querying.
        /// </summary>
        public static readonly Exposer<Beam, string> FootprintFile = new Exposer<Beam, string>((obj) => obj.FootprintFile, "FootprintFile");
    }
}
