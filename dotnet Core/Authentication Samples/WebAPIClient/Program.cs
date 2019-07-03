using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIClient
{
    class Program
    {
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static Uri redirectUri = new Uri(ConfigurationManager.AppSettings["ida:RedirectUri"]);
        private static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        private static string valuesAppId = ConfigurationManager.AppSettings["todo:valuesAppId"];
        private static string valuesClientSecret = ConfigurationManager.AppSettings["ida:valuesClientSecret"];
        private static string valuesBaseAddress = ConfigurationManager.AppSettings["todo:valuesBaseAddress"];


        static void Main(string[] args)
        {
            Console.WriteLine("Trying to get token....");

            // This is all using ADAL - https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Adal-to-Msal
            // So Scopes which are available in the V2 EndPoints are not available here - you need the newer MSAL for that.
            // The Azure Active Directory (Azure AD) v2.0 endpoint supports the industry-standard OAuth 2.0 and OpenID Connect 1.0 protocols. The Microsoft Authentication Library (MSAL) is designed to work with the Azure AD v2.0 endpoint. It's also possible to use open-source libraries that support OAuth 2.0 and OpenID Connect 1.0.
            // https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-v2-libraries
            AuthenticationResult result = null;

            AuthenticationContext authContext = new AuthenticationContext(authority, new FileTokenCache());

            Task t = Task.Run(async () =>
            {
                // Make connection using users creds: this will pop open a login box.
                //result = await authContext.AcquireTokenAsync(valuesAppId, clientId, redirectUri, new PlatformParameters(PromptBehavior.Always));

                // Make connection using application credientials: uses appid + secret.
                ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);
                result = await authContext.AcquireTokenAsync(valuesAppId, clientCredential);

                Console.WriteLine(result.AccessToken);

                // Retrieve the user's To Do List.
                Console.WriteLine("Getting data....");
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, valuesBaseAddress + "/api/values");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                
                Console.WriteLine(response.IsSuccessStatusCode);
                Console.WriteLine(response.ReasonPhrase);

                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);



                if (response.IsSuccessStatusCode)
                {

                }

            });
            t.Wait();

            
        }
    }
}
