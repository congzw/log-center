using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
                
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();
                config.AddNLog("nlog.config"); //for file log
                //config.SetMinimumLevel(LogLevel.Trace);
            });
            return services;
        }

        private static ILogger _logger = null;
        private static void UseCenterLogLogging(IApplicationBuilder app)
        {
            //var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            _logger = app.ApplicationServices.GetService<ILogger>();
            _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>> NetCoreLogConfig.Setup");
            NetCoreLogConfig.Setup(_logger);
            _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>> NetCoreLogConfig.Setup OK");
            //NetCoreLogConfig.EnableDefault(false);

            var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private static void OnShutdown()
        {
            try
            {
                _logger?.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>> OnShutdown");
                NLog.LogManager.Shutdown();
            }
            catch (Exception e)
            {
                _logger?.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>> OnShutdown", e);
            }
        }
    }
    
    public class NetCoreLogConfig
    {
        private static bool init = false;
        private static readonly ILogger _logger = null;
        public static void Setup(ILogger logger)
        {
            if (init)
            {
                return;
            }

            var simpleLogFactory = (SimpleLogFactory)SimpleLogFactory.Resolve();
            var newSimpleLogFactory = new NetCoreLogFactory(simpleLogFactory, logger);
            SimpleLogFactory.Resolve = () => newSimpleLogFactory;

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

            var logLevel = (LogLevel) ((int) args.Level);
            _logger.Log(logLevel, args.Message?.ToString());
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

    public class NetCoreLog : ISimpleLog
    {
        public string Category { get; set; }

        private readonly ILogger _logger;

        public NetCoreLog(ILogger logger, string category)
        {
            Category = category;
            _logger = logger;
        }

        public SimpleLogLevel EnabledLevel { get; set; }
        
        private readonly SimpleLogLevelHelper _simpleLogLevelHelper = SimpleLogLevelHelper.Instance;
        public Task Log(object message, SimpleLogLevel level)
        {
            var logLevel = _simpleLogLevelHelper.ParseToLogLevel((int)level);
            _logger.Log(logLevel, message?.ToString());
            return Task.CompletedTask;
        }
    }

    internal class SimpleLogLevelHelper
    {
        public SimpleLogLevelHelper()
        {
            //Trace = 0,
            //Debug = 1,
            //Information = 2,
            //Warning = 3,
            //Error = 4,
            //Critical = 5,
            //None = 6

            LogLevels = new Dictionary<int, LogLevel>();
            var values = Enum.GetValues(typeof(LogLevel));
            foreach (int intValue in values)
            {
                LogLevels[intValue] = (LogLevel)intValue;
            }
            
            SimpleLogLevels = new Dictionary<int, SimpleLogLevel>();
            var simpleValues = Enum.GetValues(typeof(SimpleLogLevel));
            foreach (int simpleValue in simpleValues)
            {
                SimpleLogLevels[simpleValue] = (SimpleLogLevel)simpleValue;
            }

        }

        public IDictionary<int, LogLevel> LogLevels { get; set; }
        public IDictionary<int, SimpleLogLevel> SimpleLogLevels { get; set; }

        public SimpleLogLevel ParseToSimpleLogLevel(int level)
        {
            if (level <= 0)
            {
                return SimpleLogLevel.Trace;
            }

            if (level >= 6)
            {
                return SimpleLogLevel.None;
            }

            return SimpleLogLevels[level];
        }

        public LogLevel ParseToLogLevel(int level)
        {
            if (level <= 0)
            {
                return LogLevel.Trace;
            }

            if (level >= 6)
            {
                return LogLevel.None;
            }

            return LogLevels[level];
        }

        public static SimpleLogLevelHelper Instance = new SimpleLogLevelHelper();
    }
    
    public class NetCoreLogFactory : ISimpleLogFactory
    {
        private readonly ISimpleLogFactory _simpleLogFactory;
        private readonly ILogger _logger;

        public NetCoreLogFactory(ISimpleLogFactory simpleLogFactory, ILogger logger)
        {
            _logger = logger;
            _simpleLogFactory = simpleLogFactory;
            SimpleLogs = new Dictionary<string, ISimpleLog>();
        }

        public SimpleLogSettings Settings {
            get => _simpleLogFactory.Settings;
            set => _simpleLogFactory.Settings = value;
        }
        
        public LogMessageActions LogActions
        {
            get => _simpleLogFactory.LogActions;
            set => _simpleLogFactory.LogActions = value;
        }

        public IDictionary<string, ISimpleLog> SimpleLogs { get; set; }

        public ISimpleLog Create(string category)
        {
            return new NetCoreLog(_logger, category);
        }

        public ISimpleLog GetOrCreate(string category)
        {
            var tryFixCategory = Settings.TryFixCategory(category);
            var tryGetValue = SimpleLogs.TryGetValue(tryFixCategory, out var theOne);
            if (!tryGetValue || theOne == null)
            {
                theOne = Create(tryFixCategory);
                SimpleLogs.Add(tryFixCategory, theOne);
            }
            return theOne;
        }
    }
}
