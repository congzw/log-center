using System.Reflection;
using Common.Logs;
using Common.Logs.LogCenter.Server;
using Common.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common
{
    public static class StartupForCommon
    {
        public static IServiceCollection AddTheCommon(this IServiceCollection services, Assembly[] assemblies)
        {
            //service locator
            services.AddSingleton(RootServiceProvider.Instance);

            //lazy singleton
            var lazySingleton = LazySingleton.Instance;
            services.AddSingleton(lazySingleton);
            //反射自动补齐：LazySingleton
            var singletonItems = lazySingleton.GetLazySingletonItems(assemblies);
            foreach (var singletonItem in singletonItems)
            {
                services.AddSingleton(singletonItem.Type, singletonItem.Value);
            }

            //replace net core log
            services.Replace(ServiceDescriptor.Singleton<ISimpleLog, NetCoreLog>());
            
            //setup self as a log center
            services.AddLogCenterServer();

            ////add contexts
            //services.AddTheContexts();

            return services;
        }

        public static IApplicationBuilder UseTheCommon(this IApplicationBuilder app)
        {
            var root = app.ApplicationServices;

            //service locator
            var rootServiceProvider = root.GetRequiredService<RootServiceProvider>();
            rootServiceProvider.Root = root;

            //lazy singleton
            var lazySingleton = root.GetService<LazySingleton>();
            lazySingleton.RootServiceProvider = root;

            return app;
        }
    }
}
