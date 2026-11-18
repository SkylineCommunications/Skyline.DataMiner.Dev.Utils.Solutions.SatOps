namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions
{
    /// <summary>A SatOps-owned reason for rejecting a reservation operation.</summary>
    public sealed class ReservationOperationError
    {
        internal ReservationOperationError(ReservationOperationErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>Gets the reason code.</summary>
        public ReservationOperationErrorCode Code { get; }

        /// <summary>Gets the explanation of the rejection.</summary>
        public string Message { get; }
    }
}
