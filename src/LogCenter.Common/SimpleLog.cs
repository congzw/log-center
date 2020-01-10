using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogCenter.Common
{
    //内部使用的简单日志，不依赖任何第三方类库
    public interface ISimpleLog
    {
        SimpleLogLevel EnabledLevel { get; set; }
        Task Log(object message, SimpleLogLevel level);
    }

    public enum SimpleLogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }

    internal class SimpleLog : ISimpleLog
    {
        public SimpleLog(LogMessageActions actions)
        {
            LogActions = actions;
        }

        public LogMessageActions LogActions { get; set; }

        public string Category { get; set; }

        public SimpleLogLevel EnabledLevel { get; set; }

        public Task Log(object message, SimpleLogLevel level)
        {
            if (this.ShouldLog(level))
            {
                var args = LogMessageArgs.Create(Category, message, level);
                LogActions.Invoke(args);
            }
            return Task.CompletedTask;
        }
    }

    public static class SimpleLogExtensions
    {
        public static bool ShouldLog(this SimpleLogLevel currentLevel, SimpleLogLevel enabledLevel)
        {
            return currentLevel >= enabledLevel && currentLevel != SimpleLogLevel.None;
        }

        public static bool ShouldLog(this ISimpleLog simpleLog, SimpleLogLevel currentLevel)
        {
            return currentLevel.ShouldLog(simpleLog.EnabledLevel);
        }

        public static void LogInfo(this ISimpleLog simpleLog, string message)
        {
            simpleLog.Log(message, SimpleLogLevel.Information);
        }

        public static void LogEx(this ISimpleLog simpleLog, Exception ex, string message = null)
        {
            var logMessage = string.Format("{0} => {1}", message ?? ex.Message, ex.StackTrace);
            simpleLog.Log(logMessage, SimpleLogLevel.Error);
        }
    }

    #region factory and settings

    public interface ISimpleLogFactory
    {
        SimpleLogSettings Settings { get; set; }
        LogMessageActions LogActions { get; set; }

        ISimpleLog Create(string category);
        ISimpleLog GetOrCreate(string category);
    }

    public class SimpleLogFactory : ISimpleLogFactory
    {
        public SimpleLogFactory(SimpleLogSettings settings, LogMessageActions actions)
        {
            Settings = settings;
            LogActions = actions;
            SimpleLogs = new ConcurrentDictionary<string, ISimpleLog>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, ISimpleLog> SimpleLogs { get; set; }

        public SimpleLogSettings Settings { get; set; }
        public LogMessageActions LogActions { get; set; }

        public ISimpleLog Create(string category)
        {
            var tryFixCategory = Settings.TryFixCategory(category);
            var simpleLogLevel = Settings.GetEnabledLevel(tryFixCategory);
            return new SimpleLog(LogActions) { Category = tryFixCategory, EnabledLevel = simpleLogLevel };
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

        #region for di extensions

        private static readonly Lazy<ISimpleLogFactory> LazyInstance = new Lazy<ISimpleLogFactory>(() => new SimpleLogFactory(new SimpleLogSettings(), new LogMessageActions()));
        public static Func<ISimpleLogFactory> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }

    public class SimpleLogSingleton<T>
    {
        private ISimpleLog _simpleLog;
        public ISimpleLog Logger => _simpleLog ?? (_simpleLog = SimpleLogFactory.Resolve().GetOrCreateLogFor<T>());

        public static SimpleLogSingleton<T> Instance = new SimpleLogSingleton<T>();
    }

    public class SimpleLogSettings
    {
        public SimpleLogSettings()
        {
            Items = new ConcurrentDictionary<string, SimpleLogSetting>(StringComparer.OrdinalIgnoreCase);
            Default = new SimpleLogSetting() { Category = DefaultCategory, EnabledLevel = SimpleLogLevel.Trace };
        }

        public void SetEnabledLevel(string category, SimpleLogLevel level)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentNullException(nameof(category));
            }

            var key = category.Trim();
            var tryGetValue = Items.TryGetValue(key, out var setting);
            if (!tryGetValue || setting == null)
            {
                setting = new SimpleLogSetting();
                setting.Category = key;
                Items.Add(key, setting);
            }
            setting.EnabledLevel = level;
        }

        public SimpleLogLevel GetEnabledLevel(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentNullException(nameof(category));
            }

            var key = category.Trim();
            var tryGetValue = Items.TryGetValue(key, out var setting);
            if (!tryGetValue || setting == null)
            {
                //todo:try find first by key start with?
                return Default.EnabledLevel;
            }
            return setting.EnabledLevel;
        }

        public string TryFixCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return DefaultCategory;
            }

            return category.Trim();
        }

        private SimpleLogSetting _default;

        public SimpleLogSetting Default
        {
            get { return _default; }
            set
            {
                if (value == null || value.Category != DefaultCategory)
                {
                    throw new InvalidOperationException();
                }

                _default = value;
                SetEnabledLevel(DefaultCategory, value.EnabledLevel);
            }
        }

        public static string DefaultCategory = "Default";
        public static string DefaultPrefix = string.Empty;
        public IDictionary<string, SimpleLogSetting> Items { get; set; }
    }

    public class SimpleLogSetting
    {
        public string Category { get; set; }
        public SimpleLogLevel EnabledLevel { get; set; }
    }

    public static class SimpleLogFactoryExtensions
    {
        public static ISimpleLog CreateLogFor(this ISimpleLogFactory factory, Type type)
        {
            return factory.Create(type.FullName);
        }

        public static ISimpleLog CreateLogFor<T>(this ISimpleLogFactory factory)
        {
            return factory.CreateLogFor(typeof(T));
        }

        public static ISimpleLog CreateLogFor(this ISimpleLogFactory factory, object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (instance is Type type)
            {
                return factory.CreateLogFor(type);
            }
            return factory.CreateLogFor(instance.GetType());
        }

        public static ISimpleLog GetOrCreateLogFor(this ISimpleLogFactory factory, Type type)
        {
            return factory.GetOrCreate(type.FullName);
        }

        public static ISimpleLog GetOrCreateLogFor<T>(this ISimpleLogFactory factory)
        {
            return factory.GetOrCreateLogFor(typeof(T));
        }

        public static ISimpleLog GetOrCreateLogFor(this ISimpleLogFactory factory, object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (instance is Type type)
            {
                return factory.GetOrCreateLogFor(type);
            }
            return factory.GetOrCreateLogFor(instance.GetType());
        }
    }

    #endregion

    #region for simple extensions used

    public class LogMessageActions : Dictionary<string, LogMessageAction>
    {
        private static string _prefix = null;

        private static void LogMessage(LogMessageArgs args)
        {
            if (_prefix == null)
            {
                _prefix = string.IsNullOrWhiteSpace(SimpleLogSettings.DefaultPrefix)
                    ? string.Empty
                    : string.Format("[{0}]", SimpleLogSettings.DefaultPrefix.Trim());
            }
            Trace.WriteLine(string.Format("{0} [{1}][{2}]{3} {4}", args.Category, "SimpleLog", args.Level.ToString(), _prefix, args.Message));
        }

        public LogMessageActions()
        {
            var defaultCategory = SimpleLogSettings.DefaultCategory;
            this.Add(defaultCategory, new LogMessageAction(defaultCategory, true, LogMessage));
        }

        public void Invoke(LogMessageArgs args)
        {
            //todo result cache?
            var logMessageActions = this.Values.Where(x => x.Enabled).ToList();
            foreach (var logMessageAction in logMessageActions)
            {
                logMessageAction.Action(args);
            }
        }

        public LogMessageActions SetActions(string name, bool enabled, Action<LogMessageArgs> action)
        {
            this[name] = new LogMessageAction(name, enabled, action);
            return this;
        }
    }

    public class LogMessageAction
    {
        public LogMessageAction(string name, bool enabled, Action<LogMessageArgs> action)
        {
            //todo validate
            Name = name;
            Enabled = enabled;
            Action = action;
        }

        public string Name { get; set; }
        public bool Enabled { get; set; }
        public Action<LogMessageArgs> Action { get; set; }
    }

    public class LogMessageArgs
    {
        public LogMessageArgs(string category, object message, SimpleLogLevel level)
        {
            Category = category;
            Message = message;
            Level = level;
        }

        public string Category { get; set; }
        public object Message { get; set; }
        public SimpleLogLevel Level { get; set; }

        public static LogMessageArgs Create(string category, object message, SimpleLogLevel level)
        {
            return new LogMessageArgs(category, message, level);
        }
    }

    #region demo for async notify

    ////how to use: => DemoForAsyncNotifyConfig.Setup();
    //public class DemoForAsyncNotifyConfig
    //{
    //    public static void Setup()
    //    {
    //        var simpleLogFactory = SimpleLogFactory.Resolve();
    //        var logActions = simpleLogFactory.LogActions;
    //        logActions["AsyncNotify"] = new LogMessageAction("AsyncNotify", true, args =>
    //        {
    //            //todo your Async Notify code!
    //        });
    //    }
    //}

    #endregion

    #region demo for init log for net core logging


    ////how to use: => LogToNetCoreLoggingConfig.Setup();
    //public class LogToNetCoreLoggingConfig
    //{
    //    private static bool init = false;
    //    public static void Setup()
    //    {
    //        if (init)
    //        {
    //            return;
    //        }
    //        var simpleLogFactory = SimpleLogFactory.Resolve();
    //        var logMessageActions = simpleLogFactory.LogActions;
    //        ////禁用默认的Debug
    //        //logMessageActions["Default"].Enabled = false;

    //        //适配到 => NetCoreLogging
    //        logMessageActions.SetActions("NetCoreLogging", true, LogToNetCoreLogging);
    //        init = true;
    //    }
    //    private static void LogToNetCoreLogging(LogMessageArgs args)
    //    {
    //        if (args == null)
    //        {
    //            return;
    //        }

    //        var loggerFactory = MyLogConfig.Instance.Factory;
    //        var logger = loggerFactory.CreateLogger(args.Category);
    //        logger.Log((LogLevel)args.Level, args.Message?.ToString());
    //    }
    //}

    #endregion
    #endregion

    //utils code only use LogHelper
    public class LogHelper
    {
        public ISimpleLog GetLogger(string category = null)
        {
            var logger = _simpleLogFactory.GetOrCreate(category);
            return logger;
        }

        private readonly ISimpleLogFactory _simpleLogFactory;
        public LogHelper()
        {
            _simpleLogFactory = SimpleLogFactory.Resolve();
        }
        public static LogHelper Instance = new LogHelper();
        public static Task Log(object message)
        {
            var simpleLog = Instance.GetLogger(null);
            return simpleLog.Log(message, SimpleLogLevel.Trace);
        }
    }

    public static class LogHelperExtensions
    {
        public static ISimpleLog GetLogger(this LogHelper logHelper, Type theType)
        {
            var logger = logHelper.GetLogger(theType?.FullName);
            return logger;
        }

        public static ISimpleLog GetLogger<T>(this LogHelper logHelper)
        {
            var logger = logHelper.GetLogger(typeof(T));
            return logger;
        }
    }
}