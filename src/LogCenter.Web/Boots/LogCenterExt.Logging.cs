using LogCenter.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCenter.Web.Boots
{
    public static partial class LogCenterExt
    {
        private static IServiceCollection AddCenterLogLogging(IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();

                //a hack for ILogger injection!
                config.Services.AddSingleton<LoggerWrapper>();
                config.Services.AddSingleton<ILogger>(sp => sp.GetService<LoggerWrapper>().Logger);
            });

            return services;
        }
        
        private static void UseCenterLogLogging(IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            LogHelper.Log(">>>>>>>>>>>>>>>>>>>>>>>> LogToNetCoreLoggingConfig.Setup");
            LogToNetCoreLoggingConfig.Setup(loggerFactory);
            LogHelper.Log(">>>>>>>>>>>>>>>>>>>>>>>> LogToNetCoreLoggingConfig.Setup OK");
            LogToNetCoreLoggingConfig.EnableDefault(false);
        }
    }
    
    public class LogToNetCoreLoggingConfig
    {
        private static bool init = false;
        private static ILoggerFactory _loggerFactory = null;
        public static void Setup(ILoggerFactory loggerFactory)
        {
            if (init)
            {
                return;
            }

            _loggerFactory = loggerFactory;
            var simpleLogFactory = SimpleLogFactory.Resolve();
            var logMessageActions = simpleLogFactory.LogActions;
            //适配到 => NetCoreLogging
            logMessageActions.SetActions("NetCoreLogging", true, LogToNetCoreLogging);
            init = true;
        }

        public static void EnableDefault(bool enable)
        {
            var simpleLogFactory = SimpleLogFactory.Resolve();
            var logMessageActions = simpleLogFactory.LogActions;
            //禁用默认的Debug
            logMessageActions["Default"].Enabled = enable;

        }

        private static void LogToNetCoreLogging(LogMessageArgs args)
        {
            if (args == null)
            {
                return;
            }

            var logger = _loggerFactory.CreateLogger(args.Category);
            logger.Log((LogLevel)args.Level, args.Message?.ToString());
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
}
