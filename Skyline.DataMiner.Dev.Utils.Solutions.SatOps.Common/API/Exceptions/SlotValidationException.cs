namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Exceptions
{
    using System;

    /// <summary>A slot request failed domain validation, rather than infrastructure processing.</summary>
    public sealed class SlotValidationException : ArgumentException
    {
        /// <summary>Initializes a slot validation failure.</summary>
        /// <param name="message">The user-facing validation reason.</param>
        /// <param name="parameterName">The invalid parameter.</param>
        /// <param name="isConflict">Whether the request conflicts with another slot.</param>
        public SlotValidationException(string message, string parameterName = null, bool isConflict = false)
            : base(message, parameterName)
        {
            Detail = message;
            IsConflict = isConflict;
        }

        /// <summary>Gets the validation reason without parameter or exception diagnostics.</summary>
        public string Detail { get; }

        /// <summary>Gets whether another slot conflicts with the request.</summary>
        public bool IsConflict { get; }
    }
}
