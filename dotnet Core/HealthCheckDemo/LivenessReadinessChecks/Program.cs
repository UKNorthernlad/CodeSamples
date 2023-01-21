// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0

// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HealthCheckDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Start a background process that takes 30 seconds to fully start.
            builder.Services.AddHostedService<StartupBackgroundService>();
            // We need to a single StartupHealthCheck object to the DI container because ???????
            builder.Services.AddSingleton<StartupHealthCheck>();

            // Register a the StartupHealthCheck....
            builder.Services.AddHealthChecks()
                .AddCheck<StartupHealthCheck>("Startup",
                tags: new[] { "ready" }
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure the endpoints
            // This will always respond with Healthy IF the application is reponding.
            app.MapHealthChecks("/hc/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            // This will only run checks tagged with "ready"
            app.MapHealthChecks("/hc/ready", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("ready")
            });




            // Additionlly you can:
            //   * XXXXXX

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}