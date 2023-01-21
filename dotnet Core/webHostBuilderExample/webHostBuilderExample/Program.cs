using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace webHostBuilderExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Adds basic stuff around config files, logging, sets content root path etc.
            var hostBuilder = Host.CreateDefaultBuilder();

            // Add ASP.Net Core web hosting specific settings - manually specify the middlewear
            var webHostBuilder = hostBuilder.ConfigureWebHost(MyWebHostBuilder);

            // Use this to instead add a "common" set of middlewears.
            //var webHostBuilder = hostBuilder.ConfigureWebHostDefaults(MyWebHostBuilderDefaults);

            var host = webHostBuilder.Build();

            host.Run();
        }

        static void MyWebHostBuilder(IWebHostBuilder webBuilder)
        {
            webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            });

            webBuilder.ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.Configure(options =>
                {
                    options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                                        | ActivityTrackingOptions.TraceId
                                                        | ActivityTrackingOptions.ParentId;
                });
                loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddEventSourceLogger();
            });

            webBuilder.UseContentRoot(Directory.GetCurrentDirectory());

            webBuilder.UseKestrel();

            webBuilder.UseStartup<Startup>();
        }

        static void MyWebHostBuilderDefaults(IWebHostBuilder webBuilder)
        {
            webBuilder.UseStartup<Startup>();
        }

    }

}
