using System;
using Common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Logs.LogCenter.Server
{
    /// <summary>
    /// 用于记录来自远端的日志
    /// </summary>
    public class RemoteLogRepository
    {
        #region for di extensions

        [LazySingleton]
        public static RemoteLogRepository Instance => LazySingleton.Instance.Resolve(() => new RemoteLogRepository());

        #endregion

        public ILoggerFactory LoggerFactory { get; set; }
        
        public string GetRemoteCategory(string category, string clientId)
        {
            var remotePrefix = ReportLogConst.RemoteLogsPrefix;
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                remotePrefix = $"{remotePrefix}.{clientId}";
            }
            
            if (string.IsNullOrWhiteSpace(category))
            {
                category = string.Empty;
            }
            if (!category.StartsWith("RemoteLogs", StringComparison.OrdinalIgnoreCase))
            {
                category = string.IsNullOrWhiteSpace(category) ? remotePrefix : $"{remotePrefix}.{category}";
            }
            return category;
        }
        
        public void Log(ReportLogArgs args)
        {
            //所有的日志，都被加上了Remote.的前缀，可以防止循环调用
            LoggerFactory ??= RootServiceProvider.Instance.Root.GetService<ILoggerFactory>();
            var remoteCategory = GetRemoteCategory(args.Category, args.ClientId);
            var logger = LoggerFactory.CreateLogger(remoteCategory);
            logger.Log(args.Level.AsLogLevel(), args.Message.ToString());
        }
    }
}