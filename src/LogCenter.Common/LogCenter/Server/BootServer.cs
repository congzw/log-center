using System;
using Common.Logs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCenter.Server
{
    public static class BootServer
    {
        public static IServiceCollection AddLogCenterServer(this IServiceCollection services, Action<ILoggingBuilder> configure = null)
        {
            services.AddMySignalR();

            services.AddLogging(config =>
            {
                config.Services.AddSingleton<NetCoreLogHelper>();
                config.Services.AddSingleton<ILogHelper>(sp => sp.GetService<NetCoreLogHelper>());
                //a hack for ILogger injection!
                config.Services.AddSingleton<ILogger>(sp => sp.GetService<NetCoreLogHelper>().Logger);

                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();

                configure?.Invoke(config);
            });

            return services;
        }
        
        public static IApplicationBuilder UseLogCenterServer(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
        {
            app.UseMySignalR();

            var logHelper = app.ApplicationServices.GetService<NetCoreLogHelper>();
            logHelper.Info(">>>> OnInit NetCoreLogHelper Begin");
            LogHelper.Resolve = () => logHelper;
            logHelper.Info(">>>> OnInit NetCoreLogHelper Finished");

            configure?.Invoke(app);

            return app;
        }
    }
}
