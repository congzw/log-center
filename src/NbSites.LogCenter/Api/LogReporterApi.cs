using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Logs.LogCenter;
using Common.Logs.LogCenter.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NbSites.LogCenter.Api
{
    //使用[HttpGet]是为了方便浏览器直接测试
    [ApiController]
    [Route("Api/Fx/Logs/LogReporter/[action]")]
    public class LogReporterApi : ControllerBase
    {
        [HttpGet]
        public Task<LogReporterStatus> GetReporterStatus()
        {
            return LogReporter.Instance.GetStatus();
        }

        [HttpGet]
        public Task<LogReporterStatus> TryStart([FromServices] LogReporterConfig config)
        {
            return LogReporter.Instance.TryStart(config);
        }

        [HttpGet]
        public Task<LogReporterStatus> TryStop()
        {
            return LogReporter.Instance.TryStop();
        }
        
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public LogReporterConfig GetConfig()
        {
            return LogReporter.Instance.Config;
        }

        /// <summary>
        /// 获取忽略发送到远程的日志类别
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MessageResult GetIgnoreLogs()
        {
            var ignoreCaches = RemoteIgnoreLoggers.Instance.GetIgnoreCaches();
            var items = new List<string>();
            foreach (var item in ignoreCaches)
            {
                items.Add(item.Value ? $"{item.Key} ignored!" : $"{item.Key} not ignored.");
            }
            var messageResult = MessageResult.CreateSuccess("OK");
            messageResult.Data = items.OrderBy(x => x);
            return messageResult;
        }
        
        /// <summary>
        /// 直接向远端日志中心发送日志进行测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageResult> TestReportLog([FromBody] ReportLogArgs args)
        {
            var messageResult = new MessageResult();
            messageResult.Data = args;

            var reporter = LogReporter.Instance;
            if (reporter.ShouldReport())
            {
                messageResult.Message = "Not ShouldReport!";
                return messageResult;
            }

            await reporter.ReportLog(args);

            messageResult.Success = true;
            return messageResult;
        }
        
        /// <summary>
        /// 通过本地的日志系统进行测试
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        [HttpGet]
        public MessageResult TestLog([FromServices] ILogger<LogReporterApi> logger)
        {
            var logLevels = Enum.GetValues<LogLevel>();
            foreach (var logLevel in logLevels)
            {
                logger.Log(logLevel, $"test log for {logLevel} ({(int)logLevel})");
            }
            return MessageResult.CreateSuccess("OK");
        }
    }
}
