using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

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
            var baseDirectory = GetBaseDirectory();
            var configuration = new ConfigurationBuilder().SetBasePath(baseDirectory)
                .AddJsonFile("LogCenterConfig.json", true)
                .Build();

            var webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .UseStartup<Startup>();

            return webHostBuilder;
        }


        private static string GetBaseDirectory()
        {
            var basePath = Environment.CurrentDirectory;
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            }
            return basePath;
        }
    }
}
