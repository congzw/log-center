using System;
using Common;
using Common.Logs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace LogCenter.Web.Boots
{
    public static partial class LogCenterExt
    {
        private static IServiceCollection AddRemoteLogsServer(IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                config.Services.AddSingleton<NetCoreLogHelper>();
                config.Services.AddSingleton<ILogHelper>(sp => sp.GetService<NetCoreLogHelper>());
                //a hack for ILogger injection!
                config.Services.AddSingleton<ILogger>(sp => sp.GetService<NetCoreLogHelper>().Logger);

                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();
                config.AddNLog("nlog.config"); //for file log
                //config.SetMinimumLevel(LogLevel.Trace);
            });
            return services;
        }

        private static void UseRemoteLogsServer(IApplicationBuilder app)
        {
            var serviceLocator = app.ApplicationServices.GetService<IServiceLocator>();
            var logHelper = serviceLocator.GetService<NetCoreLogHelper>();
            logHelper.Info(">>>> OnInit NetCoreLogHelper Begin");
            LogHelper.Resolve = serviceLocator.GetService<NetCoreLogHelper>;
            logHelper.Info(">>>> OnInit NetCoreLogHelper Finished");

            var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdownNLog, logHelper);
        }

        private static void OnShutdownNLog(object state)
        {
            var logHelper = state as ILogHelper;
            try
            {
                logHelper?.Info(">>>> OnShutdownNLog");
                NLog.LogManager.Shutdown();
            }
            catch (Exception e)
            {
                logHelper?.Error(e, ">>>> OnShutdownNLog Error");
            }
        }
    }
    
    public class NetCoreLogHelper : ILogHelper
    {
        public NetCoreLogHelper(ILogger<NetCoreLogHelper> loggerWrapper)
        {
            Logger = loggerWrapper ?? throw new ArgumentNullException(nameof(loggerWrapper));
        }

        public void Log(string message, int level)
        {
            Logger.Log(level.AsLogLevel(), message);
        }

        public ILogger Logger { get; set; }
    }

    public static class NetCoreLogExtensions
    {
        public static LogLevel AsLogLevel(this int level)
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
                return LogLevel.Trace;
            }
            if (level <= 1)
            {
                return LogLevel.Debug;
            }
            if (level <= 2)
            {
                return LogLevel.Information;
            }
            if (level <= 3)
            {
                return LogLevel.Warning;
            }
            if (level <= 4)
            {
                return LogLevel.Error;
            }
            //if (level <= 5)
            //{
            //    return LogLevel.Critical;
            //}
            //return LogLevel.None;
            return LogLevel.Critical;
        }
    }
}
