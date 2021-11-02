using Microsoft.Extensions.Logging;

namespace Common.Logs
{
    public class NetCoreLog : ISimpleLog
    {
        private readonly ILoggerFactory _loggerFactory;

        public NetCoreLog(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Log(string category, object msg, int level)
        {
            if (msg == null)
            {
                return;
            }
            var logger = _loggerFactory.CreateLogger(category);
            logger.Log((LogLevel)level, msg.ToString());
        }
    }

    public static class NetCoreLogExtensions
    {
        public static LogLevel AsLogLevel(this int level)
        {
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
            if (level <= 5)
            {
                return LogLevel.Critical;
            }
            return LogLevel.None;
        }
    }
}
