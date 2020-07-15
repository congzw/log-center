using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LogCenter.Server
{
    public static class BootSignalR
    {
        private static bool added = false;
        private static bool used = false;

        internal static IServiceCollection AddMySignalR(this IServiceCollection services)
        {
            if (!added)
            {
                services.AddSignalR();
                added = true;
            }
            return services;
        }

        internal static IApplicationBuilder UseMySignalR(this IApplicationBuilder app)
        {
            if (!used)
            {
                app.UseSignalR(routes =>
                {
                    routes.MapHub<LogHub>("/hubs/logHub");
                });
                used = true;
            }

            return app;
        }
    }
}
