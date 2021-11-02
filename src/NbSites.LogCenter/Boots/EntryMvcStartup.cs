using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common;
using Common.Fx.DI;
using Common.Logs.LogCenter.Server;
using Common.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NbSites.LogCenter.Boots
{
    public static class EntryMvcStartup
    {
        public static IServiceCollection MainConfigureService(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
        {
            var reflectHelper = ReflectHelper.Instance;
            var assemblies = reflectHelper.GetAssembliesFrom(AppDomain.CurrentDomain.BaseDirectory, new[] { "Common", "NbSites" }).ToArray();

            //AppRegistry
            AppRegistry.Instance.Init(assemblies).SetupSingletons(services);
            
            //AppRuntime
            AppRuntime.Instance.Init(runtime =>
            {
                runtime.Bags["Env"] = new
                {
                    env.EnvironmentName,
                    env.ApplicationName,
                    env.ContentRootPath,
                    env.WebRootPath,
                    AppContext.BaseDirectory,
                    AppContext.TargetFrameworkName
                };
            });

            //Common
            services.AddTheCommon(assemblies);

            //DI
            services.AddTheScrutor(assemblies);

            ////ApiDoc
            //services.AddTheApiDoc(assemblies);

            ////mini profiler
            //services.AddTheMiniProfiler();

            ////TheDbContext, SharedModelBinder
            //services.AddTheDbContext(config);
            //services.AddSharedModelBinder(assemblies);

            return services;
        }

        public static IApplicationBuilder MainConfigure(this IApplicationBuilder app, IWebHostEnvironment webEnv)
        {
            app.UseTheCommon();

            if (webEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMyStaticFiles(webEnv);
            
            app.UseRouting();
            app.UseLogCenterServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            return app;
        }

        public static IMvcCoreBuilder ConfigTheMvcCore(this IMvcCoreBuilder mvcCoreBuilder)
        {
            //mvcCoreBuilder.AddTheMvcFilter();
            return mvcCoreBuilder;
        }

        private static void UseMyStaticFiles(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //解决linux的大小写问题
                var physicalFileProvider = env.WebRootFileProvider;
                var myPhysicalFileProvider = new MyPhysicalFileProvider(physicalFileProvider);
                myPhysicalFileProvider.InitMap("", physicalFileProvider);
                env.WebRootFileProvider = myPhysicalFileProvider;
            }

            app.UseDefaultFiles(new DefaultFilesOptions() { DefaultFileNames = new List<string>() { "index.html" } });
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings =
                    {
                        [".vue"] = "text/html"
                    }
                }
            });
        }
    }
}
