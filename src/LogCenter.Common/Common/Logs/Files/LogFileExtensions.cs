using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Common.Logs.Files
{
    public static class LogFileExtensions
    {
        public static IServiceCollection AddTheLogFiles(this IServiceCollection services, string logDirPath)
        {
            //support log files browse and download
            var logFileProvider = new LogFileProvider(new PhysicalFileProvider(logDirPath));
            services.AddSingleton<ILogFileProvider>(logFileProvider);
            services.AddTransient<LogFileService>();
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