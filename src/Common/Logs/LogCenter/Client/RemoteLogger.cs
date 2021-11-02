using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Common.Logs.LogCenter.Server;
using Microsoft.Extensions.Logging;

namespace Common.Logs.LogCenter.Client
{
    /// <summary>
    /// 内部使用LogReporter发送给远端
    /// </summary>
    public class RemoteLogger : ILogger
    {
        /// <summary>
        /// 用于支持多客户端的标识
        /// </summary>
        public string ClientId { get; set; }

        public string Category { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //fix none!
            if (logLevel == LogLevel.None)
            {
                return;
            }
            
            //防止循环调用：相比在Reporter控制，这里更提升效率
            if (RemoteIgnoreLoggers.Instance.ShouldIgnore(Category))
            {
                //防止循环调用
                return;
            }

            var reporter = LogReporter.Instance;
            if (!reporter.ShouldReport())
            {
                return;
            }

            var category = Category ?? this.GetType().FullName;
            var msg = $"[{logLevel}] {formatter(state, exception)}";
            var reportLogArgs = ReportLogArgs.Create(category, msg, (int)logLevel);
            reportLogArgs.ClientId = ClientId;

            //立即返回，不等待
#pragma warning disable 4014
            reporter.ReportLog(reportLogArgs);
#pragma warning restore 4014
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public void Dispose()
        {
        }
    }

    [ProviderAlias("RemoteLogCenter")]
    public class RemoteLoggerProvider : ILoggerProvider
    {
        public RemoteLoggerProvider(string clientId)
        {
            ClientId = clientId;
        }

        public string ClientId { get; set; }

        public IDictionary<string, RemoteLogger> RemoteLoggers { get; set; } = new ConcurrentDictionary<string, RemoteLogger>(StringComparer.OrdinalIgnoreCase);

        public ILogger CreateLogger(string categoryName)
        {
            if (!RemoteLoggers.ContainsKey(categoryName))
            {
                RemoteLoggers.Add(categoryName, new RemoteLogger() { Category = categoryName, ClientId = ClientId });
            }
            return RemoteLoggers[categoryName];
        }

        public void Dispose()
        {
        }
    }

    public class RemoteIgnoreLoggers
    {
        public RemoteIgnoreLoggers()
        {
            Ignores = new List<string>(new[]
            {
                typeof(LogReporter).FullName
                , typeof(LogHub).FullName
                , ReportLogConst.RemoteLogsPrefix
            });
        }

        /// <summary>
        /// 忽略的Logger类型，用于防止循环调用
        /// </summary>

        public List<string> Ignores { get; set; }

        public void AddIgnores(params string[] addIgnores)
        {
            if (addIgnores == null)
            {
                return;
            }

            foreach (var addIgnore in addIgnores)
            {
                var exist = Ignores.Any(x => x.StartsWith(addIgnore, StringComparison.OrdinalIgnoreCase));
                if (!exist)
                {
                    Ignores.Add(addIgnore);
                }
            }
        }


        public IDictionary<string, bool> GetIgnoreCaches()
        {
            return _cacheResult;
        }

        private readonly IDictionary<string, bool> _cacheResult = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        public bool ShouldIgnore(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return true;
            }
            
            if (_cacheResult.ContainsKey(category))
            {
                return _cacheResult[category];
            }

            var shouldIgnore = Ignores.Any(x => category.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            _cacheResult[category] = shouldIgnore;
            return shouldIgnore;
        }

        public static RemoteIgnoreLoggers Instance = new RemoteIgnoreLoggers();
    }
}
