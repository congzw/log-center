using Common.Logs.LogCenter.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace NbSites.LogCenter
{
    public static class NLogExtensions
    {
        public static IHostBuilder ConfigureNLog(this IHostBuilder hostBuilder)
        {
            //参照NLog的netcore3.x的配置方法，接管日志系统
            //1 设置并使用nlog.config(Xml配置)
            //2 清理所有默认的Providers，SetMinimumLevel（仅使用nlog.config的配置方式）
            //3 区别了"NbSites.*"和"*"两种日志的配置方式，并将日志的文件目录统一："${basedir}/nlogs/" => AppDomain.CurrentDomain.BaseDirectory
            return hostBuilder.ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddRemoteHubReporter(hostContext.Configuration);
                }).UseNLog();
        }
    }
}
