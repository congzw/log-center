using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;

namespace LogCenter.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .ConfigureApiBehaviorOptions(options =>
                {
                    //options.SuppressConsumesConstraintForFormFileParameters = true;
                    //options.SuppressInferBindingSourcesForParameters = true;
                    //options.SuppressModelStateInvalidFilter = true;
                    //options.SuppressMapClientErrors = true;
                    //options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
                });
            //mvcBuilder.AddApplicationPart(typeof(LogCenter.Web.AppServices.TestApiController).Assembly).AddControllersAsServices();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseDefaultFiles();
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".vue"] = "text/html";
            var staticFileOptions = new StaticFileOptions
            {
                ContentTypeProvider = provider
            };
            app.UseStaticFiles(staticFileOptions);

            app.UseMvc();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
