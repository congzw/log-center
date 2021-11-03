using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Logs;

namespace NbSites.LogCenter.Api.Logs.AppServices
{
    public interface IMemoryLogService : IAutoInjectAsTransient
    {
        List<MemoryLogVo> GetMemoryLogs(GetMemoryLogsArgs args);
        MemoryLogVo GetMemoryLog(GetMemoryLogArgs args);
    }

    public class GetMemoryLogArgs
    {
        public string Name { get; set; }
    }
    
    public class GetMemoryLogsArgs
    {
        public string Like { get; set; }
        public bool? WithLogs { get; set; }
    }

    public class MemoryLogVo
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public List<string> Logs { get; set; } = new List<string>();
    }

    public class MemoryLogService : IMemoryLogService
    {
        public List<MemoryLogVo> GetMemoryLogs(GetMemoryLogsArgs args)
        {
            var items = new List<MemoryLogVo>();
            if (args == null) throw new ArgumentNullException(nameof(args));

            var loggers = MemoryLogFactory.Instance.Loggers.Values.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(args.Like))
            {
                loggers = loggers.Where(x => x.Name.Contains(args.Like, StringComparison.OrdinalIgnoreCase));
            }

            var withLogs = args.WithLogs.HasValue && args.WithLogs.Value;
            foreach (var logger in loggers)
            {
                var item = new MemoryLogVo();
                item.Name = logger.Name;
                var theLogs = logger.Logs.ToList();
                item.Count = theLogs.Count;
                if (withLogs)
                {
                    foreach (var theLog in theLogs)
                    {
                        item.Logs.Add(theLog.Message);
                    }
                }
                items.Add(item);
            }
            return items;
        }

        public MemoryLogVo GetMemoryLog(GetMemoryLogArgs args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (string.IsNullOrWhiteSpace(args.Name))
            {
                return null;
            }

            if (MemoryLogFactory.Instance.Loggers.TryGetValue(args.Name, out var logger))
            {
                var item = new MemoryLogVo();
                item.Name = logger.Name;
                var theLogs = logger.Logs.ToList();
                item.Count = theLogs.Count;
                foreach (var theLog in theLogs)
                {
                    item.Logs.Add(theLog.Message);
                }
                return item;
            }
            return null;
        }
    }
}
