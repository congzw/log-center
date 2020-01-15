using System;
using System.Threading.Tasks;
using LogCenter.Common;
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
            var result = args.Validate();
            if (!result.Success)
            {
                await this.Clients.Caller.SendAsync(HubConst.ReportLogCallback, result);
            }

            try
            {
                await this.Clients.Others.SendAsync(HubConst.ReportLog, args);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }

            await this.Clients.Caller.SendAsync(HubConst.ReportLogCallback, result);
        }
    }
}
