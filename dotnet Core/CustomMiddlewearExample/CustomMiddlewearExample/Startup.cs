using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomMiddlewearExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMemoryCache();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-2.2

            // Typical middlewear order:
            // 1. Exception / error handling
            // 2. HTTP Strict Transport Security Protocol
            // 3. HTTPS redirection
            // 4. Static file server
            // 5. Cookie policy enforcement
            // 6. Authentication
            // 7. Session
            // 8. MVC

            // Any calls to app.Run().... will short circuit the pipeline and go with what's been generated so far.




            // app.Use() will do it's own thing then pass control to the next middlewear.

            if (env.IsDevelopment())
            {
                // 1.Exception / error handling
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // 2. HTTP Strict Transport Security Protocol
                app.UseHsts();
            }

            // 3.HTTPS redirection
            app.UseHttpsRedirection();

            // 4. Static file server
            // None

            // 5. Cookie policy enforcement
            // None

            // 6. Authentication
            app.UseAuthentication();

            // 7. Session
            // None

            app.UseMvc();
            // 8. MVC

            // A small custom inline middlewear
            app.Use(async (context, next) =>
            {
                // Do work that doesn't write to the Response.
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });


            // Adding a custom middlewear
            app.UseRequestCulture();

            // https://dotnetcoretutorials.com/2017/03/10/creating-custom-middleware-asp-net-core/
            app.UseAPIRateLimiter();


        // In-memory caching 
        // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2




        }
    }
}
