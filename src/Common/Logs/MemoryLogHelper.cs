using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Utilities;

namespace Common.Logs
{
    public class MemoryLogger
    {
        public string Name { get; set; }
        public MemoryLogQueue Logs { get; set; } = new MemoryLogQueue();
        
        public void Log(object msg)
        {
            Logs.Enqueue(new MemoryLog { Message = $"{msg}" });
        }
        
        public MemoryLogger WithMaxLength(int maxLength)
        {
            if (maxLength > 0)
            {
                Logs.MaxLength = maxLength;
            }
            return this;
        }
    }

    public class MemoryLogQueue : ConcurrentQueue<MemoryLog>
    {
        public static int DefaultMaxLength = 100;
        
        public int MaxLength { get; set; } = DefaultMaxLength;

        public new void Enqueue(MemoryLog item)
        {
            if (Count >= MaxLength)
            {
                TryDequeue(out _);
            }
            base.Enqueue(item);
        }
    }
    
    public class MemoryLog
    {
        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.Now;
        public string Message { get; set; }
    }

    public class MemoryLogFactory
    {
        private MemoryLogFactory() { }

        public static MemoryLogFactory Instance = new MemoryLogFactory();

        public MemoryLogger GetLogger(string name)
        {
            var logger = Loggers.GetBagValue(name, Create(name), true);
            return logger;
        }

        public MemoryLogger CreateLogger(string name)
        {
            var logger = new MemoryLogger {Name = name};
            return logger;
        }

        public IDictionary<string, MemoryLogger> Loggers { get; set; } = new ConcurrentDictionary<string, MemoryLogger>(StringComparer.OrdinalIgnoreCase);
        
        private static MemoryLogger Create(string name)
        {
            var memoryLogger = new MemoryLogger();
            memoryLogger.Name = name;
            return memoryLogger;
        }
    }

    public static class MemoryLogFactoryExtensions
    {
        public static MemoryLogger GetLoggerForType(this MemoryLogFactory factory, Type theType)
        {
            if (theType == null) throw new ArgumentNullException(nameof(theType));
            return factory.GetLogger(theType.FullName);
        }
        public static MemoryLogger GetLoggerForType<T>(this MemoryLogFactory factory)
        {
            return factory.GetLoggerForType(typeof(T));
        }
        public static MemoryLogger GetLoggerForInstance(this MemoryLogFactory factory, object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            var name = $"{instance.GetType().FullName}[@]{instance.GetHashCode()}";
            return factory.GetLogger(name);
        }
        
        public static void FlushToFile(this MemoryLogger logger, string filePath, bool clearAfterFlush = false)
        {
            var logs = logger.Logs.ToArray();
            if (logs.Length > 0)
            {
                if (clearAfterFlush)
                {
                    logger.Logs.Clear();
                }
                var sb = new StringBuilder();
                foreach (var log in logs)
                {
                    sb.AppendLine($"{log.CreateAt.AsFormatDefault()} {log.Message}");
                }
                var content = sb.ToString();
                MakeSureDirExist(filePath);
                File.WriteAllText(filePath, content, Encoding.UTF8);
            }
        }
        private static void MakeSureDirExist(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath);
            if (string.IsNullOrWhiteSpace(dirPath))
            {
                dirPath = AppDomain.CurrentDomain.BaseDirectory;
            }
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}
