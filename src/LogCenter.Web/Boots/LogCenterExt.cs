using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCenter.Web.Boots
{
    public static partial class LogCenterExt
    {
        private static IHostingEnvironment _hostingEnv = null;
        private static IConfiguration _configuration = null;
        public static IServiceCollection AddLogCenter(this IServiceCollection services, IHostingEnvironment hostingEnv, IConfiguration configuration)
        {
            _hostingEnv = hostingEnv;
            _configuration = configuration;

            AddCenterLogLogging(services);
            return services;
        }
        public static void UseLogCenter(this IApplicationBuilder app)
        {
            UseLogCenterStaticFiles(app);
        }

        internal static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            return _configuration;
        }
        internal static IHostingEnvironment GetHostingEnvironment(this IServiceCollection services)
        {
            return _hostingEnv;
        }
    }
}
