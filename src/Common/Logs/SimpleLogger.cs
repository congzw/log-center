using System;

namespace Common.Logs
{
    public class SimpleLogger
    {
        public string Category { get; }
        private readonly ISimpleLog _simpleLog;

        public SimpleLogger(string category, ISimpleLog simpleLog)
        {
            Category = category;
            _simpleLog = simpleLog;
        }

        public void Log(object msg, int level)
        {
            _simpleLog.Log(Category, msg, level);
        }
    }
    
    public static class SimpleLoggerExtensions
    {
        public static void Debug(this SimpleLogger logger, object msg)
        {
            //default as 1: Debug
            logger.Log(msg, 1);
        }

        public static void Info(this SimpleLogger logger, object msg)
        {
            //default as 2: Info
            logger.Log(msg, 2);
        }

        public static SimpleLogger GetLogger(this ISimpleLog simpleLog, string category)
        {
            return new SimpleLogger(category, simpleLog);
        }
        public static SimpleLogger GetLogger(this ISimpleLog simpleLog, Type theType)
        {
            return simpleLog.GetLogger(theType.FullName);
        }
        public static SimpleLogger GetLogger<T>(this ISimpleLog simpleLog)
        {
            return simpleLog.GetLogger(typeof(T));
        }
    }
}