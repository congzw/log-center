using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace LogCenter.Client
{
    public class RemoteHubReporter
    {
        public RemoteHubReporterConfig Config { get; set; }
        public HubConnection Connection { get; set; }

        public async Task Init(RemoteHubReporterConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Connection = await TryInitHubConnection();
        }

        public async Task ReportLog(ReportLogArgs args)
        {
            if (!Config.Enabled)
            {
                return;
            }

            if (Connection == null)
            {
                return;
            }
            
            try
            {
                await Connection.InvokeAsync("ReportLog", args);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
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
                    Trace.WriteLine(ex.Message);
                }
            }
            Trace.WriteLine("TryInitHubConnection failed: " + Config.MaxTryCount);
            return null;
        }

        private async Task<HubConnection> InitHubConnection()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(Config.HubUri)
                .Build();

            await connection.StartAsync();
            connection.Closed += async (ex) =>
            {
                Trace.WriteLine(ex);
                await Task.Delay(TimeSpan.FromSeconds(1));
                await Connection.StartAsync();
            };

            return connection;
        }

        public static RemoteHubReporter Instance = new RemoteHubReporter();
    }

    public class RemoteHubReporterConfig
    {
        public int MaxTryCount { get; set; } = 3;
        public bool Enabled { get; set; }
        public string HubUri { get; set; }
    }
}