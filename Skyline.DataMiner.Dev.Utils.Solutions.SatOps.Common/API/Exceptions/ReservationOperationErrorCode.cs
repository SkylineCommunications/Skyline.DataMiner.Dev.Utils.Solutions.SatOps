namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions
{
    /// <summary>Identifies why a reservation operation was rejected.</summary>
    public enum ReservationOperationErrorCode
    {
        /// <summary>The booking state does not permit the operation.</summary>
        InvalidBookingState,

        /// <summary>The service start time cannot be changed.</summary>
        StartTimeChangeNotAllowed,

        /// <summary>The pre-roll start cannot be changed.</summary>
        PreRollStartChangeNotAllowed,

        /// <summary>The service end time cannot be changed.</summary>
        EndTimeChangeNotAllowed,

        /// <summary>The post-roll end cannot be changed.</summary>
        PostRollEndChangeNotAllowed,

        /// <summary>Resource reassignment is not permitted.</summary>
        ResourceReassignmentNotAllowed,

        /// <summary>A change to the booking node graph is not permitted.</summary>
        NodeGraphChangeNotAllowed,

        /// <summary>The frequency range of an Active booking cannot be changed.</summary>
        FrequencyRangeChangeNotAllowed,

        /// <summary>The bandwidth of an Active booking cannot be changed.</summary>
        BandwidthChangeNotAllowed,

        /// <summary>The satellite capability of an Active booking cannot be changed.</summary>
        SatelliteCapabilityChangeNotAllowed,

        /// <summary>Additional MediaOps validation failed alongside a lifecycle rejection.</summary>
        MediaOpsValidationFailure,
    }
}
