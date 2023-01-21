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

            // Configure Health Checks - known as HealthCheckRegistrations
            // Each registration is given a name - so it can be referenced later.
            //
            // Reference checks from a dedicated class
            builder.Services.AddHealthChecks().AddCheck<SampleHealthCheck>("FooMicroService");
            // Or make inline checks
            builder.Services.AddHealthChecks()
                .AddCheck("ExternalRedisCache", () =>
                        HealthCheckResult.Healthy("Ok"))
                .AddCheck("Database", () =>
                        HealthCheckResult.Degraded("NotSoGood"))
                // Notice this HealthCheckRegistration has a tagged applied. This will be used later to apply runtime execution filtering.
                .AddCheck("InternalCache", () =>
                        HealthCheckResult.Healthy("Capacity: 50% remaining"), new[] { "Internal" });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Set the URL that you want to use for checking health.
            // Will invoke each registered health check when called.
            // app.MapHealthChecks("/hc");
            // If you are using a Startup.cs rather than the .Net 6.0 minimal startup code, you would put something like
            // endpoints.MapHealthChecks("/hc"); into the app.UseEndpoints() call.

            // By adding HealthCheckOptions we can format the results of running all the checks as JSON.
            app.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // If all you need is a quick "is this endpoint responding" check WITHOUT running any of the actual health checks, use a Predicate
            // Foreach HealthCheckRegistration a check is made to see if that particular test should be run - the Predicate will be true or false
            // Here we write an anonymous funtion to always return false - which means none of the tests are actually run.
            app.MapHealthChecks("/qhc", new HealthCheckOptions()
            {
                Predicate = (HealthCheckRegistration healthCheck) => false
            });

            // If you just need to run a particular test
            app.MapHealthChecks("/dbhc", new HealthCheckOptions()
            {
                Predicate = (HealthCheckRegistration blah) => 
                {
                    if (blah.Name == "Database")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            });

            // You can also run a limited range of tests based on how they have been tagged:
            app.MapHealthChecks("/ihc", new HealthCheckOptions
            {
                Predicate      = (HealthCheckRegistration healthCheck) => healthCheck.Tags.Contains("Internal"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // You can also override the default HTTP Reponse codes depending on the overall health status.
            // By default, Healthy = 200, Degraded = 200, Unhealthy = 503
            app.MapHealthChecks("/shc", new HealthCheckOptions()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status200OK // Setting unhealthy to HTTP 200
                }
            });


            // Additionlly you can:
            //   * Restrict healthcheck endpoints to only listen on certain hosts or ports.
            //   * Require authorization.
            //   * Create a custom UIResponseWriter to totally reformat the output or change it to XML etc.
            //   * Define a custom query for a database probe.
            //   * Create Liveness & Readiness checks/probes.

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}