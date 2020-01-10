using System.Threading.Tasks;
using LogCenter.Common;
using Microsoft.AspNetCore.Mvc;

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
            if (!ReportLogArgs.Validate(args, out var vMessage))
            {
                //not block it
                LogHelper.Log("Bad Request For LogReportApi: " + vMessage);

                messageResult.Message = vMessage;
                return Task.FromResult(messageResult);
            }

            var logHelper = LogHelper.Instance;
            var simpleLog = logHelper.GetLogger(args.Category);

            var simpleLogLevel = Parse(args.Level);

            //not block
            simpleLog.Log(args.Message, simpleLogLevel);

            messageResult.Success = true;
            return Task.FromResult(messageResult);
        }

        private SimpleLogLevel Parse(int level)
        {
            //Trace = 0,
            //Debug = 1,
            //Information = 2,
            //Warning = 3,
            //Error = 4,
            //Critical = 5,
            //None = 6

            if (level <= 0)
            {
                return SimpleLogLevel.Trace;
            }
            if (level <= 1)
            {
                return SimpleLogLevel.Debug;
            }
            if (level <= 2)
            {
                return SimpleLogLevel.Information;
            }
            if (level <= 3)
            {
                return SimpleLogLevel.Warning;
            }
            if (level <= 4)
            {
                return SimpleLogLevel.Error;
            }
            if (level <= 5)
            {
                return SimpleLogLevel.Critical;
            }
            return SimpleLogLevel.None;
        }
    }
}
