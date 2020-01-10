using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace LogCenter.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            //var configuration = new ConfigurationBuilder().SetBasePath("todo")
            //    .AddJsonFile("LogCenterConfig.json", true)
            //    .Build();

            var webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            return webHostBuilder;
        }
    }
}
