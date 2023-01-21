using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventLogWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1
            CreateHostBuilder(args).Build().Run();

            Console.WriteLine("2nd line.");
        }

        // This creates a "Host" object that does a load of useful things, e.g. reads env variables, appsettings config files, etc.
        // Also sets up various logging providers.
        // Sets up the a DI container.
        // See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<SampleService>();
        });









    }
}
