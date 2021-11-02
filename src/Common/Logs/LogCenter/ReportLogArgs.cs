namespace Common.Logs.LogCenter
{
    public class ReportLogArgs
    {
        /// <summary>
        /// 用于支持多客户端的标识
        /// </summary>
        public string ClientId { get; set; } = string.Empty;
        public string Category { get; set; }
        public object Message { get; set; } //simple types or JObject
        public int Level { get; set; }

        public static ReportLogArgs Create(string category, dynamic message, int level)
        {
            return new ReportLogArgs { Category = category, Level = level, Message = message };
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
}