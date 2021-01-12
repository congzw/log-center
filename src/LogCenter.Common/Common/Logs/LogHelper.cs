using System;
using System.Diagnostics;

namespace Common.Logs
{
    public interface ILogHelper
    {
        void Log(string message, int level, string category = "");
    }

    public class LogHelper : ILogHelper
    {
        public void Log(string message, int level = 1, string category = "")
        {
            var logMessage = string.Format("{0} [{1}]{2} {3}", "LogCenter.Common.LogHelper", "Default", level.ToString(), message);
            Trace.WriteLine(logMessage);
        }

        #region for extensions and simple use

        internal static Lazy<LogHelper> Lazy = new Lazy<LogHelper>(() => new LogHelper());
        public static ILogHelper Instance => Resolve == null ? Lazy.Value : Resolve();
        public static Func<ILogHelper> Resolve = null;

        #endregion
    }

    public static class LogHelperExtensions
    {
        public static void Trace(this ILogHelper logHelper, string message)
        {
            logHelper.Log(message, 0);
        }
        public static void Debug(this ILogHelper logHelper, string message)
        {
            logHelper.Log(message, 1);
        }
        public static void Info(this ILogHelper logHelper, string message)
        {
            logHelper.Log(message, 2);
        }
        public static void Warn(this ILogHelper logHelper, string message)
        {
            logHelper.Log(message, 3);
        }
        public static void Error(this ILogHelper logHelper, Exception ex, string message)
        {
            var logMessage = string.Format("{0}=>{1}", message, ex?.StackTrace);
            logHelper.Log(logMessage, 4);
        }
        public static void Fatal(this ILogHelper logHelper, Exception ex, string message)
        {
            var logMessage = string.Format("{0}=>{1}", message, ex?.StackTrace);
            logHelper.Log(logMessage, 5);
        }
    }
}
