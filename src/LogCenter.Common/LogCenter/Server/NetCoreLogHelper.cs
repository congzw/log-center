using System;
using Common.Logs;
using Microsoft.Extensions.Logging;

namespace LogCenter.Server
{
    public class NetCoreLogHelper : ILogHelper
    {
        public NetCoreLogHelper(ILogger<NetCoreLogHelper> loggerWrapper)
        {
            Logger = loggerWrapper ?? throw new ArgumentNullException(nameof(loggerWrapper));
        }

        public void Log(string message, int level)
        {
            Logger.Log(level.AsLogLevel(), message);
        }

        public ILogger Logger { get; set; }
    }

    public static class NetCoreLogExtensions
    {
        public static LogLevel AsLogLevel(this int level)
        {
            //Trace = 0,
            //Debug = 1,
            //Information = 2,
            //Warning = 3,
            //Error = 4,
            //Critical = 5,
            //None = 6

            if (level <= 0)
            {
                return LogLevel.Trace;
            }
            if (level <= 1)
            {
                return LogLevel.Debug;
            }
            if (level <= 2)
            {
                return LogLevel.Information;
            }
            if (level <= 3)
            {
                return LogLevel.Warning;
            }
            if (level <= 4)
            {
                return LogLevel.Error;
            }
            //if (level <= 5)
            //{
            //    return LogLevel.Critical;
            //}
            //return LogLevel.None;
            return LogLevel.Critical;
        }
    }
}