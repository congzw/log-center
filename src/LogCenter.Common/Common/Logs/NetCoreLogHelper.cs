using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Logs
{
    public class NetCoreLogHelper : LogHelper, ILogHelper
    {
        public NetCoreLogHelper(ILogger<LogHelper> loggerWrapper)
        {
            Logger = loggerWrapper ?? throw new ArgumentNullException(nameof(loggerWrapper));
        }

        public string MessagePrefix { get; set; } = ">>>> ";
        public new void Log(string message, int level, string category = "")
        {
            if (MessagePrefix != null)
            {
                message = MessagePrefix + message;
            }
            if (!string.IsNullOrWhiteSpace(category))
            {
                message = "[" + category + "]" + message;
            }
            Logger.Log(level.AsLogLevel(), message);
        }

        public ILogger Logger { get; set; }
    }

    public static class NetCoreLogHelperExtensions
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
            if (level <= 5)
            {
                return LogLevel.Critical;
            }
            return LogLevel.None;
        }

        public static void Log(this ILogHelper logHelper, string message, LogLevel level)
        {
            logHelper.Log(message, (int)level);
        }

        public static IServiceCollection AddNetCoreLogHelper(this IServiceCollection services)
        {
            services.AddSingleton<NetCoreLogHelper>();
            services.AddSingleton<ILogHelper, NetCoreLogHelper>(sp => sp.GetRequiredService<NetCoreLogHelper>());
            services.AddSingleton<LogHelper>(sp => sp.GetRequiredService<NetCoreLogHelper>());
            return services;
        }

        public static IApplicationBuilder UseNetCoreLogHelper(this IApplicationBuilder app)
        {
            var logHelper = app.ApplicationServices.GetRequiredService<NetCoreLogHelper>();
            logHelper.Info("LogHelper.Resolve start");
            LogHelper.Resolve = () => logHelper;
            logHelper.Info("LogHelper.Resolve end");
            return app;
        }
    }
}
