namespace Common.Logs.LogCenter
{
    public class ReportLogConst
    {
        /// <summary>
        /// 连接时使用的参数名：LogMonitor => true or false
        /// 需要接受日志反馈的监控功能
        /// </summary>
        public static string ArgsLogMonitor = "LogMonitor";

        /// <summary>
        /// 连接时使用的参数名：ClientId => "*" or "SomeClient"
        /// 代表该实时日志监控端所属的分组: "*"接受所有日志， "SomeClient"只接收ReportLogArgs.Client == "SomeClient"的日志
        /// 例如：如果日志中的ReportLogArgs.ClientId被设置为"ClientA"，那么它将被发送给分组["*","ClientA"]
        /// </summary>
        public static string ArgsClientId = "ClientId";

        /// <summary>
        /// 接受所有日志的日志监控端使用的ClientId的值
        /// </summary>
        public static string AnyClientId = "*";

        /// <summary>
        /// 服务器端接收日志的方法名
        /// </summary>
        public static string ReportLog = "ReportLog";

        /// <summary>
        /// 服务器端接收日志后，回调通知实时日志监控端的回调方法名
        /// </summary>
        public static string ReportLogCallback = "ReportLogCallback";

        /// <summary>
        /// 远程日志分类的前缀
        /// </summary>
        public static string RemoteLogsPrefix = "RemoteLogs";
    }
}
