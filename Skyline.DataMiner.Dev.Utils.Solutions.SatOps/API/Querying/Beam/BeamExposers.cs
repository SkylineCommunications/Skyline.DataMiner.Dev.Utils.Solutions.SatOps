namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying.Beam
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Beam;
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
        /// Exposes the <see cref="ApiObject.Id"/> property for querying as "Guid".
        /// </summary>
        public static readonly Exposer<Beam, Guid> Guid = new Exposer<Beam, Guid>((obj) => obj.Id, "Guid");

        /// <summary>
        /// Exposes the <see cref="Beam.Name"/> property for querying as "Name".
        /// </summary>
        public static readonly Exposer<Beam, string> Name = new Exposer<Beam, string>((obj) => obj.Name, "Name");

        /// <summary>
        /// Exposes the <see cref="Beam.Status"/> property for querying as "Status".
        /// </summary>
        public static readonly Exposer<Beam, InstanceStatus> Status = new Exposer<Beam, InstanceStatus>((obj) => obj.Status, "Status");

        /// <summary>
        /// Exposes the <see cref="Beam.BeamSatellite"/> property for querying.
        /// </summary>
        public static readonly Exposer<Beam, Guid> Satellite = new Exposer<Beam, Guid>((obj) => obj.BeamSatellite.Value, "Satellite");

        /// <summary>
        /// Exposes the <see cref="Beam.LinkType"/> property for querying as "LinkType".
        /// </summary>
        public static readonly Exposer<Beam, BeamLinkType> LinkType = new Exposer<Beam, BeamLinkType>((obj) => obj.LinkType.Value, "LinkType");

        /// <summary>
        /// Exposes the <see cref="Beam.TransmissionType"/> property for querying as "TransmissionType".
        /// </summary>
        public static readonly Exposer<Beam, BeamTransmissionType> TransmissionType = new Exposer<Beam, BeamTransmissionType>((obj) => obj.TransmissionType.Value, "TransmissionType");

        /// <summary>
        /// Exposes the <see cref="Beam.FootprintFile"/> property for querying.
        /// </summary>
        public static readonly Exposer<Beam, string> FootprintFile = new Exposer<Beam, string>((obj) => obj.FootprintFile, "FootprintFile");
    }
}
