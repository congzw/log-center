using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace LogCenter.Client
{
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
        
        private int _maxErrorCount = 5;
        private int _errorCount = 0;

        public async Task InitAsync()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(HubUri)
                .Build();
            
            try
            {
                await Connection.StartAsync();
                Connection.Closed += async (ex) =>
                {
                    Debug.WriteLine(ex);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await Connection.StartAsync();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _errorCount++;
                if (_errorCount <= _maxErrorCount)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await InitAsync();
                }
            }
        }

        public static RemoteHubReporter Instance = new RemoteHubReporter();
    }
}