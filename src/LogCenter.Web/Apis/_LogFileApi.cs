using Common.Logs.Files;
using Microsoft.AspNetCore.Mvc;

namespace LogCenter.Web.Apis
{
    [ApiController]
    [Route("logs/files")]
    public class LogFileApi : ControllerBase
    {
        [HttpGet("{fileId}")]
        public IActionResult GetFiles([FromServices] LogFileService logFileDownloadService, string fileId)
        {
            //todo: download
            var stream = logFileDownloadService.GetFileAsStream(fileId);
            if (stream == null)
            {
                return NotFound();
            }
            return stream;
        }
    }
}
