using System;
using System.Threading.Tasks;
using LogCenter.Common;
using LogCenter.Web.Boots;
using LogCenter.Web.Domains;
using Microsoft.AspNetCore.SignalR;

namespace LogCenter.Web.Hubs
{
    public class LogHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        public virtual async Task ReportLog(ReportLogArgs args)
        {
            var validateResult = args.Validate();
            validateResult.Data = args;

            if (!validateResult.Success)
            {
                validateResult.Message += "=> Failed From Caller";
                await this.Clients.Caller.SendAsync(HubConst.ReportLogCallback, validateResult);
            }

            try
            {
                validateResult.Message += "=> Success From All";
                CallServerLog(args);
                await this.Clients.All.SendAsync(HubConst.ReportLogCallback, validateResult);
            }
            catch (Exception ex)
            {
                validateResult.Message += "=> Ex From Caller";
                validateResult.Message = ex.Message;
                validateResult.Success = false;
                await this.Clients.Caller.SendAsync(HubConst.ReportLogCallback, validateResult);
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
