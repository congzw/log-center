using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCenter.Client.Boots
{
    public static partial class LogCenterExt
    {
        private static IServiceCollection AddClientLogging(IServiceCollection services)
        {
            services.AddLogging(config =>
            {
                //a hack for ILogger injection!

                var hubUri = "ws://localhost:1635/hubs/logHub";
                config.ClearProviders();
                config.AddConsole();
                config.AddDebug();
                config.AddRemoteLog(hubUri, true);
            });
            return services;
        }

        private static void UseClientLogging(IApplicationBuilder app)
        {
        }
    }
    
}
