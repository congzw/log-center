using System.Diagnostics;
using Common.Utilities;

namespace Common.Logs
{
    public interface ISimpleLog
    {
        void Log(string category, object msg, int level = 0);
    }

    public class SimpleLog : ISimpleLog
    {
        #region for di extensions

        [LazySingleton]
        public static ISimpleLog Instance => LazySingleton.Instance.Resolve<ISimpleLog>(() => new SimpleLog());

        #endregion
        
        private static readonly string[] KnownLevels = { "Trace", "Debug", "Information", "Warning", "Error", "Critical", "None" };
        public void Log(string category, object msg, int level = 0)
        {
            if (level < 0)
            {
                level = 0;
            }

            if (level > 6)
            {
                level = 6;
            }
            var theLevel = KnownLevels[level];
            Trace.WriteLine($"{theLevel}|{category} => {msg}");
        }
    }
}