using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Fx.DI
{
    public static class ScrutorExtensions
    {
        public static IServiceCollection AddTheScrutor(this IServiceCollection services, Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            ////默认的策略，建议在启动的最开始阶段调用，这样后期可以被自定义替换
            ////使用Append，这样多个注册可以取到所有注册的列表
            //var strategy = RegistrationStrategy.Append;
            //services.Scan(scan => scan.FromAssemblies(assemblies)
            //    .AddClasses(classes => classes.AssignableTo<IAutoInjectAsTransient>())
            //    .UsingRegistrationStrategy(strategy)
            //    .AsSelfWithInterfaces()
            //    .WithTransientLifetime());

            //services.Scan(scan => scan.FromAssemblies(assemblies)
            //    .AddClasses(classes => classes.AssignableTo<IAutoInjectAsScoped>())
            //    .UsingRegistrationStrategy(strategy)
            //    .AsSelfWithInterfaces()
            //    .WithScopedLifetime());

            //services.Scan(scan => scan.FromAssemblies(assemblies)
            //    .AddClasses(classes => classes.AssignableTo<IAutoInjectAsSingleton>())
            //    .UsingRegistrationStrategy(strategy)
            //    .AsSelfWithInterfaces()
            //    .WithSingletonLifetime());

            //services.Scan(scan => scan
            //    .AddTypes(typeof(IDynamicVoProvider<>))
            //    .AddClasses()
            //    .AsImplementedInterfaces());


            //services.Scan(scan => scan
            //    .FromAssembliesOf(typeof(IOpenGeneric<>))
            //    .AddClasses(classes => classes.AssignableTo(typeof(IOpenGeneric<>)))
            //    .AsSelfWithInterfaces()
            //);
            
            ////services.Scan(scan => scan
            ////    .AddTypes(typeof(OpenGeneric<>))
            ////    .AddClasses()
            ////    .AsSelfWithInterfaces());

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                
                .AddClasses(classes => classes.AssignableTo<IAutoInjectAsSingleton>())
                .AsSelfWithInterfaces()
                .WithSingletonLifetime()

                .AddClasses(classes => classes.AssignableTo<IAutoInjectAsTransient>())
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
                
                .AddClasses(classes => classes.AssignableTo<IAutoInjectAsScoped>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
            );

            return services;
        }
    }
}