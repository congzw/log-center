using System.IO;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Common.Logs.Files
{
    public class LogFileService
    {
        private readonly ILogFileProvider _fileProvider;

        public LogFileService(ILogFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public FileStreamResult GetFileAsStream(string logFileId)
        {
            var fileInfo = _fileProvider.GetFileInfo(logFileId);
            if (!File.Exists(fileInfo.PhysicalPath))
            {
                return null;
            }
            var stream = fileInfo
                .CreateReadStream();
            return new FileStreamResult(stream, MediaTypeNames.Text.Plain);
        }
    }
}