namespace Skyline.DataMiner.Solutions.SatOps.Common.Logging
{
    using System;
    using Skyline.DataMiner.Automation;

    /// <summary>
    /// Defines a logger interface for logging messages with different severity levels.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message with the specified log type.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="type">The type of log entry. Defaults to <see cref="LogType.Information"/>.</param>
        void Log(string message, LogType type = LogType.Information);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        void Debug(string message);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        void Information(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        void Warning(string message);

        /// <summary>
        /// Logs an error message with an optional exception.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="exception">The optional exception associated with the error.</param>
        void Error(string message, Exception exception = null);
    }
}
