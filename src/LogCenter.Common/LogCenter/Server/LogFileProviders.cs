using System;
using System.IO;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace LogCenter.Server
{
    public interface ILogFileProvider : IFileProvider
    {
    }

    public class LogFileProvider : CompositeFileProvider, ILogFileProvider
    {
        public LogFileProvider(PhysicalFileProvider physicalFileProvider) : base(physicalFileProvider)
        {
        }
    }

    public class LogFileDownloadService
    {
        private readonly ILogFileProvider _fileProvider;

        public LogFileDownloadService(ILogFileProvider fileProvider)
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

    public static class LogFileExtensions
    {
        public static IServiceCollection AddTheLogFiles(this IServiceCollection services, string logDirPath)
        {
            //support log files browse and download
            var logFileProvider = new LogFileProvider(new PhysicalFileProvider(logDirPath));
            services.AddSingleton<ILogFileProvider>(logFileProvider);
            services.AddTransient<LogFileDownloadService>();
            return services;
        }

        public static IApplicationBuilder UseTheLogFiles(this IApplicationBuilder app, string logsRequestPath)
        {
            if (logsRequestPath == null) throw new ArgumentNullException(nameof(logsRequestPath));
            //make logs directory browser ok:
            var logFileProvider = app.ApplicationServices.GetRequiredService<ILogFileProvider>();
            var logsFileProvider = logFileProvider;
            var logOptions = new DirectoryBrowserOptions()
            {
                FileProvider = logsFileProvider,
                RequestPath = logsRequestPath
            };
            app.UseDirectoryBrowser(logOptions);
            return app;
        }
    }
}
