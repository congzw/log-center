using LogCenter.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogCenter.Client.Boots
{
    public static partial class LogCenterExt
    {
        private static IHostingEnvironment _hostingEnv = null;
        private static IConfiguration _configuration = null;
        private static IApplicationLifetime _applicationLifetime = null;

        public static IServiceCollection AddLogCenter(this IServiceCollection services, IHostingEnvironment hostingEnv, IConfiguration configuration)
        {
            _hostingEnv = hostingEnv;
            _configuration = configuration;
            
            services.AddSingleton<IServiceLocator, HttpRequestServiceLocator>();
            AddClientLogging(services);
            return services;
        }

        public static void UseLogCenter(this IApplicationBuilder app, IApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
            UseLogCenterStaticFiles(app);
            
            ServiceLocator.Initialize(app.ApplicationServices.GetService<IServiceLocator>());
            UseClientLogging(app);
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
