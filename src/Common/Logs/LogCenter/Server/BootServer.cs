using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Logs.LogCenter.Server
{
    public static class BootServer
    {
        private static bool _logCenterServerAdded = false;

        public static IServiceCollection AddLogCenterServer(this IServiceCollection services)
        {
            if (!_logCenterServerAdded)
            {
                //使用Newtonsoft做序列化
                services.AddSignalR().AddNewtonsoftJsonProtocol();
                _logCenterServerAdded = true;
            }
            return services;
        }

        public static IApplicationBuilder UseLogCenterServer(this IApplicationBuilder app, LogCenterServerOptions options = null)
        {
            //for test only!
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(name: "test-hub-api", pattern: "Api/LogCenter/Server/TestHubApi/GetTestResult", defaults: new { controller = "TestHubApi", action = "GetTestResult" });
                endpoints.MapHub<TestHub>("/hubs/test-hub", opt =>
                {
                    opt.Transports = HttpTransportType.WebSockets;
                });
            });

            //for log center
            app.UseLogCenterServer<LogHub>(options);
            return app;
        }

        public static IApplicationBuilder UseLogCenterServer<THub>(this IApplicationBuilder app, LogCenterServerOptions options = null) where THub : Hub
        {
            options ??= new LogCenterServerOptions
            {
                MapPath = "/hubs/logHub"
            };

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<THub>(options.MapPath, opt =>
                {
                    opt.Transports = HttpTransportType.WebSockets;
                });
            });
            return app;
        }
    }

    public class LogCenterServerOptions
    {
        public string MapPath { get; set; }
    }
}
