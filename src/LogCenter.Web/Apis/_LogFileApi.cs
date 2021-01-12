using LogCenter.Server;
using Microsoft.AspNetCore.Mvc;

namespace LogCenter.Web.Apis
{
    [ApiController]
    [Route("logs/files")]
    public class LogFileApi : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetFiles([FromServices] LogFileDownloadService logFileDownloadService, string id)
        {
            //todo: download
            var stream = logFileDownloadService.GetFileAsStream(id);
            if (stream == null)
            {
                return NotFound();
            }
            return stream;
        }
    }
}
