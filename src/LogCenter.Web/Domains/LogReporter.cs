//using LogCenter.Common;
//using LogCenter.Common.RemoteLogs;

//namespace LogCenter.Web.Domains
//{
//    public class HubConst
//    {
//        public static string ReportLog = "ReportLog";
//        public static string ReportLogCallback = "ReportLogCallback";
//    }

//    public class ReportLogArgs
//    {
//        public string Category { get; set; }
//        public object Message { get; set; } //simple types or JObject
//        public int Level { get; set; }

//        public static ReportLogArgs Create(string category, dynamic message, int level)
//        {
//            return new ReportLogArgs() { Category = category, Level = level, Message = message };
//        }

//        public static bool Validate(ReportLogArgs args, out string message)
//        {
//            if (args == null)
//            {
//                message = "日志参数不能为空";
//                return false;
//            }

//            if (args.Message == null)
//            {
//                message = "日志消息不能为空";
//                return false;
//            }

//            message = string.Empty;
//            return true;
//        }
//    }

//    public static class ReportLogArgsExtensions
//    {
//        public static MessageResult Validate(this ReportLogArgs args)
//        {
//            var validate = ReportLogArgs.Validate(args, out var message);
//            return MessageResult.Create(validate, message, args);
//        }
//    }
//}
