using System;
using LogCenter.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace LogCenter.Web.Boots
{
    public static partial class LogCenterExt
    {
        private static IServiceCollection AddCenterLogLogging(IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                //a hack for ILogger injection!
                config.Services.AddSingleton<LoggerWrapper>();
                config.Services.AddSingleton<ILogger>(sp => sp.GetService<LoggerWrapper>().Logger);
                config.Services.AddSingleton<NetCoreLogHelper>();
                config.Services.AddSingleton<ILogHelper>(sp => sp.GetService<NetCoreLogHelper>());
                
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();
                config.AddNLog("nlog.config"); //for file log
                //config.SetMinimumLevel(LogLevel.Trace);
            });
            return services;
        }

        private static void UseCenterLogLogging(IApplicationBuilder app)
        {
            //SimpleLogHelper.InitSimpleLog();
            
            var serviceLocator = app.ApplicationServices.GetService<IServiceLocator>();
            var logHelper = serviceLocator.GetService<NetCoreLogHelper>();
            logHelper.Info(">>>> OnInit NetCoreLogHelper Begin");
            NetCoreLogHelper.InitNetCoreLog(serviceLocator);
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
    
    public class LoggerWrapper
    {
        public ILogger Logger { get; set; }

        public LoggerWrapper(ILogger<LoggerWrapper> logger)
        {
            Logger = logger;
        }
    }

    public class NetCoreLogHelper : ILogHelper
    {
        public IServiceLocator ServiceLocator { get; }

        public NetCoreLogHelper(IServiceLocator serviceLocator)
        {
            ServiceLocator = serviceLocator;
            DefaultLogger = ServiceLocator.GetService<ILogger>();
        }

        public void Log(string message, int level)
        {
            DefaultLogger.Log(level.AsLogLevel(), message);
        }

        public ILogger DefaultLogger { get; set; }

        #region init for NetCoreLogHelper

        public static void InitNetCoreLog(IServiceLocator serviceLocator)
        {
            LogHelper.Resolve = serviceLocator.GetService<NetCoreLogHelper>;
        }

        #endregion
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

        public static ILogger GetLogger(this ILogHelper helper, string categoryName)
        {
            var serviceLocator = ServiceLocator.Current;
            var loggerFactory = serviceLocator.GetService<ILoggerFactory>();
            return loggerFactory.CreateLogger(categoryName);
        }

        public static ILogger GetLogger(this ILogHelper helper, Type theType)
        {
            var serviceLocator = ServiceLocator.Current;
            var loggerFactory = serviceLocator.GetService<ILoggerFactory>();
            return loggerFactory.CreateLogger(theType);
        }

        public static ILogger GetLogger<T>(this ILogHelper helper)
        {
            var serviceLocator = ServiceLocator.Current;
            var loggerFactory = serviceLocator.GetService<ILoggerFactory>();
            return loggerFactory.CreateLogger<T>();
        }
    }
}
