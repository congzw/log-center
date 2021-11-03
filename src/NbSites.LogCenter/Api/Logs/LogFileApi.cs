using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using NbSites.LogCenter.Api.Logs.AppServices;

namespace NbSites.LogCenter.Api.Logs
{
    [Route("~/Api/Fx/Logs/Files/[action]")]
    public class LogFileApi : ControllerBase
    {
        private readonly ILogFileService _logFileService;

        public LogFileApi(ILogFileService logFileService)
        {
            _logFileService = logFileService;
        }

        [HttpGet]
        public List<string> SearchLogFiles([FromQuery] SearchLogFilesArgs args)
        {
            return _logFileService.SearchLogFiles(args);
        }

        [HttpPost]
        public MessageResult DeleteLogFiles([FromBody] DeleteLogFilesArgs args)
        {
            return _logFileService.DeleteLogFiles(args);
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> Download(string fileId)
        {
            var readLogFile = await _logFileService.ReadLogFile(fileId);
            if (!readLogFile.Success)
            {
                var contentResult = new ContentResult();
                contentResult.StatusCode = 404;
                contentResult.Content = readLogFile.Message;
                return contentResult;
            }

            var contents = (string)readLogFile.Data;
            var bytes = System.Text.Encoding.UTF8.GetBytes(contents);
            return new FileContentResult(bytes, "text/plain");
        }
    }
}
