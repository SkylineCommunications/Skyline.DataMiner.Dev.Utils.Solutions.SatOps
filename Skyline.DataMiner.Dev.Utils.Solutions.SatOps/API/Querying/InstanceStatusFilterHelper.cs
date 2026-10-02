namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Querying
{
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement;
    using System;

    internal static class InstanceStatusFilterHelper
    {
        public static string ToStatusId(InstanceStatus status, string draft, string active, string deprecated, string error)
        {
            switch (status)
            {
                case InstanceStatus.Draft:
                    return draft;
                case InstanceStatus.Active:
                    return active;
                case InstanceStatus.Deprecated:
                    return deprecated;
                case InstanceStatus.Error:
                    return error;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, "Invalid lifecycle status.");
            }
        }
    }
}
