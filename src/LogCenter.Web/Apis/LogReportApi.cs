using System;
using System.Threading.Tasks;
using LogCenter.Common;
using LogCenter.Web.Boots;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogCenter.Web.Apis
{
    public interface ILogReportApi
    {
        Task<MessageResult> ReportLog([FromBody]ReportLogArgs args);
    }

    public class ReportLogArgs
    {
        public string Category { get; set; }
        public object Message { get; set; } //simple types or JObject
        public int Level { get; set; }

        public static ReportLogArgs Create(string category, dynamic message, int level)
        {
            return new ReportLogArgs() { Category = category, Level = level, Message = message };
        }

        public static bool Validate(ReportLogArgs args, out string message)
        {
            if (args == null)
            {
                message = "日志参数不能为空";
                return false;
            }

            if (args.Message == null)
            {
                message = "日志消息不能为空";
                return false;
            }
            
            message = string.Empty;
            return true;
        }
    }
    
    [Route("Api/Log")]
    public class LogReportApi : BaseLogCenterApi, ILogReportApi
    {
        [HttpPost("Report")]
        public Task<MessageResult> ReportLog([FromBody]ReportLogArgs args)
        {
            return ReportAsync(args);
        }

        private Task<MessageResult> ReportAsync(ReportLogArgs args)
        {
            var messageResult = new MessageResult();
            var logHelper = LogHelper.Instance;
            if (!ReportLogArgs.Validate(args, out var vMessage))
            {
                //not block it
                logHelper.Warn("Bad Request For LogReportApi: " + vMessage);
                messageResult.Message = vMessage;
                return Task.FromResult(messageResult);
            }

            var logger = logHelper.GetLogger(args.Category);
            logger.Log(args.Level.AsLogLevel(), args.Message?.ToString());

            messageResult.Success = true;
            return Task.FromResult(messageResult);
        }
    }

    public class RemoteLogProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            throw new System.NotImplementedException();
        }
    }

    public class RemoteLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }

}
