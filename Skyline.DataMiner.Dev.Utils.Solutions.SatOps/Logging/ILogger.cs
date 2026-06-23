namespace Skyline.DataMiner.SDM.SatOps.Common.Logging
{
    using Skyline.DataMiner.Net.IManager.Helper;
    using System;

    public interface ILogger
    {
        void Log(string message, LogType type = LogType.Information);

        void Debug(string message);

        void Information(string message);

        void Warning(string message);

        void Error(string message, Exception exception = null);
    }
}
