# how to

## boot

```

//AddTheLogFiles
services.AddTheLogFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_nlogs"));
//UseTheLogFiles
app.UseTheLogFiles("/logs/files");


```

## api controller

``` csharp

    [ApiController]
    [Route("logs/files")]
    public class LogFileController : ControllerBase
    {
        [HttpGet("{fileId}")]
        public IActionResult GetFiles([FromServices] LogFileService logFileDownloadService, string fileId)
        {
            var stream = logFileDownloadService.GetFileAsStream(fileId);
            if (stream == null)
            {
                return NotFound();
            }
            return stream;
        }
    }

```