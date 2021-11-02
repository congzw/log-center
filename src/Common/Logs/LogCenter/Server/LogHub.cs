using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Common.Logs.LogCenter.Server
{
    [AllowAnonymous]
    public class LogHub : Hub
    {
        // => "ws://whatever/hubs/logHub?LogMonitor=true&ClientId=LogMonitor
        private readonly ILogger<LogHub> _logger;

        public LogHub(ILogger<LogHub> logger)
        {
            _logger = logger;
        }
        public override async Task OnConnectedAsync()
        {
            var isLogMonitor = this.Context.GetHttpContext().TryGetQueryParameterValue(ReportLogConst.ArgsLogMonitor, false);
            var clientId = this.Context.GetHttpContext().TryGetQueryParameterValue(ReportLogConst.ArgsClientId, string.Empty);
            _logger.LogDebug($"OnConnectedAsync => isLogMonitor:{isLogMonitor}, clientId:{clientId}");
            if (isLogMonitor)
            {
                //标识：需要接受日志反馈的监控功能
                await JoinGroupsAsync(clientId);
            }
            await base.OnConnectedAsync().ConfigureAwait(false);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //如果指定了group
            var isLogMonitor = this.Context.GetHttpContext().TryGetQueryParameterValue(ReportLogConst.ArgsLogMonitor, false);
            var clientId = this.Context.GetHttpContext().TryGetQueryParameterValue(ReportLogConst.ArgsClientId, string.Empty);
            _logger.LogDebug($"OnDisconnectedAsync => isLogMonitor:{isLogMonitor}, clientId:{clientId}");
            if (isLogMonitor)
            {
                //标识：需要接受日志反馈的监控功能
                await LeaveGroupsAsync(clientId);
            }
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        /// <summary>
        /// 服务器端用来接受和存储：ReportLogArgs
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task ReportLog(ReportLogArgs args)
        {
            var validateResult = args.Validate();
            validateResult.Data = args;

            if (!validateResult.Success)
            {
                validateResult.Message += "=> Failed From Caller";
                await this.Clients.Caller.SendAsync(ReportLogConst.ReportLogCallback, validateResult);
                return;
            }

            try
            {
                validateResult.Message += "=> Success From All";
                RemoteLogRepository.Instance.Log(args);
                await SendToGroupsAsync(validateResult, args, args.ClientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ex From Caller");
                validateResult.Message = ex.Message;
                validateResult.Success = false;
                await this.Clients.Caller.SendAsync(ReportLogConst.ReportLogCallback, validateResult);
            }
        }

        private async Task JoinGroupsAsync(string groupName)
        {
            _logger.LogInformation($"join group: {groupName}");
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
        }
        private async Task LeaveGroupsAsync(string groupName)
        {
            _logger.LogInformation($"leave group: {groupName}");
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
        }
        private async Task SendToGroupsAsync(ReportLogResult validateResult, ReportLogArgs args, string groupName)
        {
            //sendTo: SomeClient,* 
            //永远额外发送给组： *
            if (groupName == ReportLogConst.AnyClientId)
            {
                await this.Clients.Groups(groupName).SendAsync(ReportLogConst.ReportLogCallback, validateResult);
            }
            else
            {
                await this.Clients.Groups(groupName, ReportLogConst.AnyClientId).SendAsync(ReportLogConst.ReportLogCallback, validateResult);
            }
        }
    }
}
