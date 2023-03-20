using Middleware.Authentication.AppService;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();




            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // This application is designed to work with App Service Easy Auth
            // Claims are passed-in via HTTP Headers or calling the /.auth/me endpoint
            // In this stackoverflow post, somebody is trying to extract the headers to populate the User variable.
            // https://stackoverflow.com/questions/42260708/azure-apps-easyauth-claims-with-net-core
            // Somebody created a middlewear to do it automatically:
            // https://github.com/lpunderscore/azureappservice-authentication-middleware
            // https://www.nuget.org/packages/AzureAppserviceAuthenticationMiddleware/
            // TODO - Get this working - currently throws a 500 error on the website when enabled.
            //app.UseAzureAppServiceAuthentication();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}