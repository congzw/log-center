using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCenter.Client.Boots
{
    public static partial class LogCenterExt
    {
        public static ILoggingBuilder AddRemoteLog(this ILoggingBuilder builder, string hubUri, bool enabled)
        {
            var reporter = RemoteHubReporter.Instance;
            reporter.HubUri = hubUri;
            reporter.Enabled = enabled;

            builder.Services.AddSingleton(RemoteHubReporter.Instance);
            builder.AddProvider(new RemoteLoggerProvider());
            return builder;
        }
    }

    public class RemoteLogger : ILogger
    {
        public string Category { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var category = Category ?? "LogCenter.Web.Hubs.RemoteLogger";
            var msg = $"[{logLevel}] {formatter(state, exception)}";
            var reportLogArgs = ReportLogArgs.Create(category, msg, (int)logLevel);

            var reporter = RemoteHubReporter.Instance;
            reporter.ReportLog(reportLogArgs);
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

    public class RemoteLoggerProvider : ILoggerProvider
    {
        public IDictionary<string, RemoteLogger> RemoteLoggers { get; set; } = new ConcurrentDictionary<string, RemoteLogger>(StringComparer.OrdinalIgnoreCase);

        public ILogger CreateLogger(string categoryName)
        {
            if (!RemoteLoggers.ContainsKey(categoryName))
            {
                RemoteLoggers.Add(categoryName, new RemoteLogger() { Category = categoryName });
            }
            return RemoteLoggers[categoryName];
        }

        public void Dispose()
        {
        }
    }

    public class RemoteHubReporter
    {
        public bool Enabled { get; set; }

        public string HubUri { get; set; }

        public HubConnection Connection { get; set; }

        public async Task ReportLog(ReportLogArgs args)
        {
            if (!Enabled)
            {
                return;
            }

            if (Connection == null)
            {
                await InitAsync();
            }

            try
            {
                await Connection.InvokeAsync("ReportLog", args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private int _maxErrorCount = 100;
        private int _errorCount = 100;

        public async Task InitAsync()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(HubUri)
                .Build();

            await Connection.StartAsync();

            Connection.Closed += async (error) =>
            {
                _errorCount++;
                if (_errorCount > _maxErrorCount)
                {
                    Debug.WriteLine("!!!!!!!!!!max error!!!!!!!!");
                    return;
                }

                Debug.WriteLine(error);
                await Task.Delay(TimeSpan.FromSeconds(1));
                await Connection.StartAsync();
            };
        }

        public static RemoteHubReporter Instance = new RemoteHubReporter();
    }
}
