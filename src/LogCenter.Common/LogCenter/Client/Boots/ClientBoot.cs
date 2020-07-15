using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCenter.Client.Boots
{
    public static class ClientBoot
    {
        private static IHostingEnvironment _hostingEnv = null;
        private static IConfiguration _configuration = null;
        private static IApplicationLifetime _applicationLifetime = null;

        public static IServiceCollection AddLogCenter(this IServiceCollection services, IHostingEnvironment hostingEnv, IConfiguration configuration)
        {
            _hostingEnv = hostingEnv;
            _configuration = configuration;
            
            services.AddSingleton<IServiceLocator, HttpRequestServiceLocator>();
            services.AddLogging(config =>
            {
                //a hack for ILogger injection!

                //should read from config! eg: => var hubUri = "ws://192.168.1.235:8000/hubs/logHub";
                var hubUri = "ws://localhost:1635/hubs/logHub";
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();
                config.AddRemoteLog(hubUri, true);
            });

            return services;
        }

        public static void UseLogCenter(this IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
            ServiceLocator.Initialize(app.ApplicationServices.GetService<IServiceLocator>());
        }
        
        internal static ILoggingBuilder AddRemoteLog(this ILoggingBuilder builder, string hubUri, bool enabled)
        {
            var reporter = RemoteHubReporter.Instance;
            reporter.HubUri = hubUri;
            reporter.Enabled = enabled;

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
