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
            services.AddMyServiceLocator();
            
            _hostingEnv = hostingEnv;
            _configuration = configuration;

            var remoteSection = _configuration.GetSection("RemoteHubReporter");

            services.Configure<RemoteHubReporterConfig>(remoteSection);
            services.AddScoped(sp => sp.GetService<IOptionsSnapshot<RemoteHubReporterConfig>>().Value); //ok => use "IOptionsSnapshot<>" instead of "IOptions<>" will auto load after changed

            var clientId = remoteSection.GetValue("ClientId", string.Empty);

            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();
                config.AddRemoteLog(clientId);
            });

            return services;
        }

        public static void UseLogCenterClient(this IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            app.UseMyServiceLocator();

            _applicationLifetime = applicationLifetime;
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var reporter = scope.ServiceProvider.GetRequiredService<RemoteHubReporter>();
                var config = scope.ServiceProvider.GetRequiredService<RemoteHubReporterConfig>();
                var initTask = reporter.Init(config);
                initTask.Wait();
            }
        }
        
        internal static ILoggingBuilder AddRemoteLog(this ILoggingBuilder builder, string clientId)
        {
            builder.Services.AddSingleton(RemoteHubReporter.Instance);
            builder.AddProvider(new RemoteLoggerProvider(clientId));
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
