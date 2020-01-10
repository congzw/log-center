using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCenter.Web.Boots
{
    public static class LogCenterExt
    {
        private static IHostingEnvironment _hostingEnv = null;
        private static IConfiguration _configuration = null;
        public static IServiceCollection AddLogCenter(this IServiceCollection services, IHostingEnvironment hostingEnv, IConfiguration configuration)
        {
            _hostingEnv = hostingEnv;
            _configuration = configuration;

            //services.AddCommon(hostingEnv);
            services.AddLogging();
            return services;
        }

        internal static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            return _configuration;
        }

        internal static IHostingEnvironment GetHostingEnvironment(this IServiceCollection services)
        {
            return _hostingEnv;
        }
        
        private static IServiceCollection AddLogging(this IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();

                //a hack for ILogger injection!
                config.Services.AddSingleton<FooLog>();
                config.Services.AddSingleton<ILogger>(sp => sp.GetService<FooLog>().Logger);
            });
            return services;
        }
    }

    public class FooLog
    {
        public ILogger Logger { get; set; }

        public FooLog(ILogger<FooLog> logger)
        {
            Logger = logger;
        }
    }
}
