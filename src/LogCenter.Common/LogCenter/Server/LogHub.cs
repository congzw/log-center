using System;
using System.Threading.Tasks;
using Common.Logs;
using Microsoft.AspNetCore.SignalR;

namespace LogCenter.Server
{
    public class LogHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            //如果指定了group，则加入组
            var isLogMonitor = this.Context.GetHttpContext().TryGetQueryParameterValue(ReportLogConst.LogMonitor, false);
            if (isLogMonitor)
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, ReportLogConst.LogMonitor);
            }
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //如果指定了group
            var isLogMonitor = this.Context.GetHttpContext().TryGetQueryParameterValue(ReportLogConst.LogMonitor, false);
            if (isLogMonitor)
            {
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, ReportLogConst.LogMonitor);
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
                await this.Clients.Caller.SendAsync(ReportLogConst.ReportLogCallback, validateResult);
            }

            try
            {
                validateResult.Message += "=> Success From All";
                CallServerLog(args);
                //只发给制定的Group: MyConst.LogMonitor
                await this.Clients.Group(ReportLogConst.LogMonitor).SendAsync(ReportLogConst.ReportLogCallback, validateResult);
            }
            catch (Exception ex)
            {
                validateResult.Message += "=> Ex From Caller";
                validateResult.Message = ex.Message;
                validateResult.Success = false;
                await this.Clients.Caller.SendAsync(ReportLogConst.ReportLogCallback, validateResult);
            }
        }

        private void CallServerLog(ReportLogArgs args)
        {
            //todo: support multi client logs => to diff files?
            var clientId = args.ClientId ?? "";
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                clientId = clientId + " ";
            }

            var logHelper = LogHelper.Instance;
            if (!string.IsNullOrWhiteSpace(args.Category))
            {
                logHelper.Log(clientId + args.Category + " " + args.Message, args.Level);
            }
            else
            {
                logHelper.Log(clientId + args.Message?.ToString(), args.Level);
            }
        }
    }
}
