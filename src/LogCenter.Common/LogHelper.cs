using System;
using System.Diagnostics;

namespace LogCenter.Common
{
    public interface ILogHelper
    {
        void Log(string message, int level);
    }

    public class LogHelper : ILogHelper
    {
        public void Log(string message, int level = 1)
        {
            var logMessage = string.Format("{0} [{1}]{2} {3}", "LogCenter.Common.LogHelper", "Default", level.ToString(), message);
            Trace.WriteLine(logMessage);
        }

        #region for extensions and simple use
        public static ILogHelper Instance => Resolve == null ? _default : Resolve();
        private static readonly ILogHelper _default = new LogHelper();
        public static Func<ILogHelper> Resolve = null;

        public static void Debug(string message)
        {
            Instance.Debug(message);
        }

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
