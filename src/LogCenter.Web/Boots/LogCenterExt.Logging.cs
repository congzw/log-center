using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogCenter.Web.Boots
{
    public static partial class LogCenterExt
    {
        private static IServiceCollection AddCenterLogLogging(IServiceCollection services)
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
