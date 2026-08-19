namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement
{
    /// <summary>
    /// Represents the lifecycle status of a satellite.
    /// </summary>
    public enum InstanceStatus
    {
        /// <summary>The satellite record is a draft and not yet active.</summary>
        Draft,

        /// <summary>The satellite is active and in use.</summary>
        Active,

        /// <summary>The satellite has been deprecated.</summary>
        Deprecated,

        /// <summary>The satellite is in an error state.</summary>
        Error,
    }
}
