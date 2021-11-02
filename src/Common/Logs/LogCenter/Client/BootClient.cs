using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Logs.LogCenter.Client
{
    public static class BootClient
    {
        private static bool _addRemoteHubReporterInvoked = false;
        public static ILoggingBuilder AddRemoteHubReporter(this ILoggingBuilder logging, IConfiguration configuration)
        {
            if (_addRemoteHubReporterInvoked)
            {
                return logging;
            }

            var remoteSection = configuration.GetSection("RemoteHubReporter");
            var reporterConfig = new LogReporterConfig();
            remoteSection.Bind(reporterConfig);
            if (reporterConfig.IgnoreCategories != null)
            {
                RemoteIgnoreLoggers.Instance.AddIgnores(reporterConfig.IgnoreCategories.ToArray());
            }

            logging.Services.Configure<LogReporterConfig>(remoteSection);
            logging.Services.AddScoped(sp => sp.GetService<IOptionsSnapshot<LogReporterConfig>>().Value);
            //ok => use "IOptionsSnapshot<>" instead of "IOptions<>" will auto load after changed

            var remoteHubReporter = LogReporter.Instance;
            remoteHubReporter.Config = reporterConfig;

            logging.Services.AddSingleton(remoteHubReporter);
            logging.AddProvider(new RemoteLoggerProvider(reporterConfig.ClientId));
            
            //remoteHubReporter.TryStart(reporterConfig).Wait();
#pragma warning disable 4014
            remoteHubReporter.TryStart(reporterConfig);
#pragma warning restore 4014
            _addRemoteHubReporterInvoked = true;
            return logging;
        }

        public static IHostBuilder AddRemoteHubReporter(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureLogging((hostContext, logging) =>
            {
                logging.AddRemoteHubReporter(hostContext.Configuration);
            });
            return hostBuilder;
        }

        ////not work for aspnetcore3.x
        //public static IServiceCollection AddRemoteHubReporter(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddLogging(logging =>
        //    {
        //        logging.AddRemoteHubReporter(configuration);
        //    });
        //    return services;
        //}
    }
}
