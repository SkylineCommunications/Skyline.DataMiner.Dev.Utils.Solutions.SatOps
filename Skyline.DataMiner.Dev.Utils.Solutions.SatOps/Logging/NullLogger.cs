namespace Skyline.DataMiner.Solutions.SatOps.Common.Logging
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An <see cref="ILogger"/> implementation that discards every message.
    /// </summary>
    /// <remarks>
    /// Used as the default logger so that the API never has to null check its logger.
    /// </remarks>

    internal class NullLogger : ILogger
    {
        public void Debug(object callerInstance, string message, object[] args = null, [CallerMemberName] string methodName = "")
        {
            // nothing to do
        }

        public void Debug(string message)
        {
            // nothing to do
        }

        public void Error(object callerInstance, string message, object[] args = null, [CallerMemberName] string methodName = "")
        {
            // nothing to do
        }

        public void Error(string message)
        {
            // nothing to do
        }

        public void Information(object callerInstance, string message, object[] args = null, [CallerMemberName] string methodName = "")
        {
            // nothing to do
        }

        public void Information(string message)
        {
            // nothing to do
        }

        public void Warning(object callerInstance, string message, object[] args = null, [CallerMemberName] string methodName = "")
        {
            // nothing to do
        }

        public void Warning(string message)
        {
            // nothing to do
        }
    }
}
