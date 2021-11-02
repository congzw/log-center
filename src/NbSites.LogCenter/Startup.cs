using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NbSites.LogCenter.Boots;

namespace NbSites.LogCenter
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnv;

        public Startup(IConfiguration config, IWebHostEnvironment webHostEnv)
        {
            _config = config;
            _webHostEnv = webHostEnv;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.MainConfigureService(_webHostEnv, _config);
            services.AddControllers(opt =>
            {
                //opt.
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<Startup>>();
                logger.LogInformation("LOG TEST");
            }
            app.MainConfigure(env);
            //app.UseOrchardCore();
        }
    }
}