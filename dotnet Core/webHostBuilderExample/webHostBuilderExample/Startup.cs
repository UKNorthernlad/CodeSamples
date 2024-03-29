using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace webHostBuilderExample
{
    // ConfigureServices and Configure are called by the ASP.NET Core runtime when the app starts.
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the DI container.
        // Called by the host before the Configure method to configure the app's services.
        public void ConfigureServices(IServiceCollection services)
        {
            //This is basically registering a whole load of Interfaces and concrete classes with the DI container.
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime.
        // Use this method to configure the services in the DI container
        // and to add middleware filter components into the HTTP request pipeline
        // Stuff in the DI container is accessible from here (as is from the rest of the application).
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
