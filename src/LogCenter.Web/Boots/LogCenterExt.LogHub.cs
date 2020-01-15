using LogCenter.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LogCenter.Web.Boots
{
    public static partial class LogCenterExt
    {
        private static IServiceCollection AddLogHub(IServiceCollection services, IHostingEnvironment hostingEnv)
        {
            return services;
        }

        private static IApplicationBuilder UseLogHub(IApplicationBuilder app, IHostingEnvironment hostingEnv)
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<LogHub>("/hubs/logHub");
            });
            return app;
        }
    }
}
