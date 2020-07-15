using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogCenter.Client
{
    public static class BootClient
    {
        private static IHostingEnvironment _hostingEnv = null;
        private static IConfiguration _configuration = null;
        private static IApplicationLifetime _applicationLifetime = null;

        public static IServiceCollection AddLogCenterClient(this IServiceCollection services, IHostingEnvironment hostingEnv, IConfiguration configuration)
        {
            _hostingEnv = hostingEnv;
            _configuration = configuration;
            
            services.AddSingleton<IServiceLocator, HttpRequestServiceLocator>();

            services.Configure<RemoteHubReporterConfig>(_configuration.GetSection("RemoteHubReporter"));
            services.AddScoped(sp => sp.GetService<IOptionsSnapshot<RemoteHubReporterConfig>>().Value); //ok => use "IOptionsSnapshot<>" instead of "IOptions<>" will auto load after changed

            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();
                config.AddRemoteLog();
            });

            return services;
        }

        public static void UseLogCenterClient(this IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
            ServiceLocator.Initialize(app.ApplicationServices.GetService<IServiceLocator>());

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var reporter = scope.ServiceProvider.GetRequiredService<RemoteHubReporter>();
                var config = scope.ServiceProvider.GetRequiredService<RemoteHubReporterConfig>();
                var initTask = reporter.Init(config);
                initTask.Wait();
            }
        }
        
        internal static ILoggingBuilder AddRemoteLog(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton(RemoteHubReporter.Instance);
            builder.AddProvider(new RemoteLoggerProvider());
            return builder;
        }
        internal static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            return _configuration;
        }
        internal static IHostingEnvironment GetHostingEnvironment(this IServiceCollection services)
        {
            return _hostingEnv;
        }
        internal static IApplicationLifetime GetApplicationLifetime(this IServiceCollection services)
        {
            return _applicationLifetime;
        }
    }
}
