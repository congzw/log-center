namespace LogCenter.Proxy
{
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

    //public static class ReportLogArgsExtensions
    //{
    //    public static MessageResult Validate(this ReportLogArgs args)
    //    {
    //        var validate = ReportLogArgs.Validate(args, out var message);
    //        return MessageResult.Create(validate, message, args);
    //    }
    //}

    //public class MessageResult
    //{
    //    public string Message { get; set; }
    //    public bool Success { get; set; }
    //    public object Data { get; set; }

    //    public static MessageResult Create(bool success, string message, object data = null)
    //    {
    //        return new MessageResult() { Success = success, Message = message, Data = data };
    //    }
    //    public static MessageResult CreateSuccess(string message, object data = null)
    //    {
    //        return new MessageResult() { Success = true, Message = message, Data = data };
    //    }
    //    public static MessageResult CreateFail(string message, object data = null)
    //    {
    //        return new MessageResult() { Success = false, Message = message, Data = data };
    //    }
    //}
}