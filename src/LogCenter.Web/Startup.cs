using System.Collections.Generic;
using System.IO;
using LogCenter.Web.Boots;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace LogCenter.Web
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; set; }
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            HostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

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

            services.AddLogCenter(HostingEnvironment, Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles(new DefaultFilesOptions() { DefaultFileNames = new List<string>() { "index.html" } });

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".vue"] = "text/html" }
                }
            });

            var fileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"));
            var imageRequestPath = "/MyImages";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath = imageRequestPath
            });
            
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = fileProvider,
                RequestPath = imageRequestPath
            });

            app.UseMvc();
        }
    }
}
