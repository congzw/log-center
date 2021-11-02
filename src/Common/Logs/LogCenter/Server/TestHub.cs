using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Logs.LogCenter.Server
{
    /// <summary>
    /// 用于测试：Hub路由系统是否好用
    /// </summary>
    [AllowAnonymous]
    public class TestHub : Hub
    {
        private readonly IServiceProvider _serviceProvider;

        public TestHub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static MemoryLogger Logger = MemoryLogFactory.Instance.GetLogger(typeof(TestHub).FullName).WithMaxLength(100);

        public override async Task OnConnectedAsync()
        {
            var clientId = this.Context.GetHttpContext().TryGetQueryParameterValue(ReportLogConst.ArgsClientId, "");
            Logger.Log($"OnConnectedAsync: {clientId}");
            try
            {
                var logger = _serviceProvider.GetService<ILogger<LogHub>>();
                ShowEnabled(logger);
            }
            catch (Exception e)
            {
                Logger.Log($"OnConnectedAsync Ex: {e.Message}");
            }


            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var clientId = this.Context.GetHttpContext().TryGetQueryParameterValue(ReportLogConst.ArgsClientId, "");
            Logger.Log($"OnDisconnectedAsync: {clientId}");
            if (exception != null)
            {
                Logger.Log($"OnDisconnectedAsync Ex: {exception.Message}");
            }
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }


        public static LogLevel? LogHubLogLevel = null;
        private static void ShowEnabled(ILogger<LogHub> logger)
        {
            if (LogHubLogLevel == null)
            {
                var logLevels = Enum.GetValues(typeof(LogLevel));
                foreach (LogLevel logLevel in logLevels)
                {
                    if (logger.IsEnabled(logLevel))
                    {
                        LogHubLogLevel = logLevel;
                        break;
                    }
                }
            }
            Logger.Log($"LogHubLogLevel >= {LogHubLogLevel}");
        }
    }
}
