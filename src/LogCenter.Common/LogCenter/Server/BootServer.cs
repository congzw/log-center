using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LogCenter.Server
{
    public static class BootServer
    {
        public static IServiceCollection AddLogCenterServer(this IServiceCollection services)
        {
            services.AddMySignalR();
            return services;
        }
        
        public static IApplicationBuilder UseLogCenterServer(this IApplicationBuilder app)
        {
            app.UseMySignalR();
            return app;
        }
    }
}
