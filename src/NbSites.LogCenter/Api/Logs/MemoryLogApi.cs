using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NbSites.LogCenter.Api.Logs.AppServices;

namespace NbSites.LogCenter.Api.Logs
{
    [Route("~/Api/Fx/Logs/MemoryLog/[action]")]
    public class MemoryLogApi : ControllerBase
    {
        private readonly IMemoryLogService _memoryLogService;

        public MemoryLogApi(IMemoryLogService memoryLogService)
        {
            _memoryLogService = memoryLogService;
        }
        
        [HttpGet]
        public List<MemoryLogVo> GetMemoryLogs([FromQuery] GetMemoryLogsArgs args)
        {
            return _memoryLogService.GetMemoryLogs(args);
        }

        [HttpGet]
        public MemoryLogVo GetMemoryLog([FromQuery] GetMemoryLogArgs args)
        {
            return _memoryLogService.GetMemoryLog(args);
        }
    }
}
