using System;
using Microsoft.AspNetCore.Hosting;
using movingtotheweb;

namespace mva
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>()
            .Build();

            host.Run();
        }
    }
}
