using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace LogCenter.Common.RemoteLogs.Server
{
    public class LogHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            //如果指定了group，则加入组
            var isLogMonitor = this.Context.GetHttpContext().TryGetQueryParameterValue(MyConst.LogMonitor, false);
            if (isLogMonitor)
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, MyConst.LogMonitor);
            }
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //如果指定了group
            var isLogMonitor = this.Context.GetHttpContext().TryGetQueryParameterValue(MyConst.LogMonitor, false);
            if (isLogMonitor)
            {
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, MyConst.LogMonitor);
            }
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        public virtual async Task ReportLog(ReportLogArgs args)
        {
            var validateResult = args.Validate();
            validateResult.Data = args;

            if (!validateResult.Success)
            {
                validateResult.Message += "=> Failed From Caller";
                await this.Clients.Caller.SendAsync(MyConst.ReportLogCallback, validateResult);
            }

            try
            {
                validateResult.Message += "=> Success From All";
                
                CallServerLog(args);
                //await this.Clients.All.SendAsync(HubConst.ReportLogCallback, validateResult);
                await this.Clients.Group(MyConst.LogMonitor).SendAsync(MyConst.ReportLogCallback, validateResult);
            }
            catch (Exception ex)
            {
                validateResult.Message += "=> Ex From Caller";
                validateResult.Message = ex.Message;
                validateResult.Success = false;
                await this.Clients.Caller.SendAsync(MyConst.ReportLogCallback, validateResult);
            }
        }

        private void CallServerLog(ReportLogArgs args)
        {
            var logHelper = LogHelper.Instance;
            if (!string.IsNullOrWhiteSpace(args.Category))
            {
                logHelper.Log(args.Category + " " + args.Message, args.Level);
            }
            else
            {
                logHelper.Log(args.Message?.ToString(), args.Level);
            }
        }
    }
}
