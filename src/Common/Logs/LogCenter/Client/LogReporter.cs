using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace Common.Logs.LogCenter.Client
{
    /// <summary>
    /// 远程日志报告类
    /// 内部不使用任何日志系统，使用的是内存日志，防止循环调用！
    /// 根据配置的Enabled，在启动时自动启用，后期通过API手动启用（前提是Enabled==true）
    /// </summary>
    public class LogReporter
    {
        public async Task<LogReporterStatus> GetStatus()
        {
            await Task.CompletedTask;
            var status = new LogReporterStatus();
            status.Config = Config;
            status.Connected = Connection != null && Connection.State == HubConnectionState.Connected;
            status.Enabled = Enabled;
            
            var enabled = status.Enabled ?  "启用中": "禁用中";
            var connected = status.Connected ?  "已连接": "未连接";
            status.Message = $"{enabled},{connected}";
            return status;
        }

        public async Task<LogReporterStatus> TryStart(LogReporterConfig config)
        {
            await Init(config);
            var status = await GetStatus();
            return status;
        }

        public async Task<LogReporterStatus> TryStop()
        {
            _closedAutoStart = false;
            if (Connection != null && Connection.State != HubConnectionState.Disconnected)
            {
                await Connection.StopAsync();
                _initInvoked = false;
            }
            var status = await GetStatus();
            return status;
        }
        
        public LogReporterConfig Config { get; set; }
        public HubConnection Connection { get; set; }
        /// <summary>
        /// 是否启用远程
        /// </summary>
        public bool Enabled => Config != null && Config.Enabled;
        /// <summary>
        /// 是否初始化过
        /// </summary>
        public bool InitInvoked => _initInvoked;

        private bool _initInvoked = false;
        public async Task Init(LogReporterConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));

            _closedAutoStart = false;
            if (Connection != null && Connection.State != HubConnectionState.Disconnected)
            {
                await Connection.StopAsync();
            }

            Connection = await TryInitHubConnection();
            _closedAutoStart = true;
            _initInvoked = true;
        }

        /// <summary>
        /// 是否需要报告
        /// </summary>
        /// <returns></returns>
        public bool ShouldReport()
        {
            if (!Enabled)
            {
                InnerLog("配置没有启用：Config.Enabled == false");
                return false;
            }

            if (!_initInvoked)
            {
                InnerLog("连接初始化没有被调用：Init() not invoked");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// 发送的前提：
        /// 1 Config != null && Config.Enabled
        /// 2 Init Invoked Success
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task ReportLog(ReportLogArgs args)
        {
            try
            {
                #region InvokeAsync vs SendAsync

                //The InvokeAsync method returns a Task which completes when the server method returns.
                //The return value, if any, is provided as the result of the Task.
                //Any exceptions thrown by the method on the server produce a faulted Task.
                //Use await syntax to wait for the server method to complete and try...catch syntax to handle errors.

                //The SendAsync method returns a Task which completes when the message has been sent to the server.
                //No return value is provided since this Task doesn't wait until the server method completes.
                //Any exceptions thrown on the client while sending the message produce a faulted Task.
                //Use await and try...catch syntax to handle send errors.

                //eg:
                //var result = await Connection.InvokeAsync<TResult>("ReportLog", args);
                //await Connection.InvokeAsync("ReportLog", args);

                #endregion

                if (string.IsNullOrEmpty(args.ClientId))
                {
                    args.ClientId = Config.ClientId;
                }
                InnerLog("Connection.SendAsync ReportLog");

                //todo: cache logs and delay flush
                //todo: 改造为批量接口，减少发送次数
                await Connection.SendAsync("ReportLog", args);
            }
            catch (Exception ex)
            {
                InnerLog("Connection.SendAsync ReportLog Ex" + ex.Message);
            }
        }

        private async Task<HubConnection> TryInitHubConnection()
        {
            for (int i = 0; i < Config.MaxTryCount; i++)
            {
                try
                {
                    var connection = await InitHubConnection();
                    return connection;
                }
                catch (Exception ex)
                {
                    InnerLog(ex);
                }
            }
            InnerLog("TryInitHubConnection failed: " + Config.MaxTryCount);
            return null;
        }

        private bool _closedAutoStart = true;
        private async Task<HubConnection> InitHubConnection()
        {
            var hubUri = Config.AutoFixHubUri();
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUri, options => {
                    options.Transports = HttpTransportType.WebSockets;
                    options.SkipNegotiation = true;
                }).Build();

            //connection.HandshakeTimeout = TimeSpan.FromSeconds(3);
            //connection.ServerTimeout = TimeSpan.FromSeconds(3);

            await connection.StartAsync();

            connection.Closed += async (ex) =>
            {
                InnerLog(ex.Message);
                if (_closedAutoStart)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await Connection.StartAsync();
                }
            };

            return connection;
        }

        public void InnerLog(object msg)
        {
            if (msg is Exception ex)
            {
                Logger.Log("ex: " + ex.Message);
                return;
            }
            Logger.Log(msg);
        }
        public MemoryLogger Logger { get; set; } = MemoryLogFactory.Instance.GetLogger(typeof(LogReporter).FullName).WithMaxLength(100);

        public static LogReporter Instance = new LogReporter();
    }

    public class LogReporterConfig
    {
        public bool Enabled { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string HubUri { get; set; }
        public int MaxTryCount { get; set; } = 3;
        public List<string> IgnoreCategories { get; set; } = new List<string>();

        public string AutoFixHubUri()
        {
            return $"{HubUri}?LogMonitor=true&ClientId={ClientId}";
        }
    }

    public class LogReporterStatus
    {
        public LogReporterConfig Config { get; set; }
        public bool Connected { get; set; }
        public bool Enabled { get; set; }
        public string Message { get; set; }
    }
}
