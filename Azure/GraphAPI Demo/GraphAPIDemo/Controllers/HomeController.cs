using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Security.Claims;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;


namespace WebApplication6.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult About()
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            string fullname = string.Format("{0}", cp.FindFirst(ClaimTypes.Name).Value);
            ViewBag.Message = string.Format("Dear {0}, welcome to the Expense Report App", fullname);
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Users()
        {
            //get the tenantName
            string tenantName = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

            // retrieve the clientId and password values from the Web.config file
            string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
            string password = ConfigurationManager.AppSettings["ida:ClientSecret"];

            var ctx = new AuthenticationContext(String.Format("https://login.windows.net/{0}", tenantName));
            var credentials = new ClientCredential(clientId, password);
            var resp = await ctx.AcquireTokenAsync("https://graph.windows.net", credentials);
            var graphUserUrl = "https://graph.windows.net/{0}/users?api-version=1.5";


 

            string requestUrl = String.Format(CultureInfo.InvariantCulture,graphUserUrl,tenantName);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resp.AccessToken);
            var usersData = await client.GetStringAsync(requestUrl);

            var users = JsonConvert.DeserializeObject<WebApplication6.Models.Users>(usersData);
            ViewBag.userList = users.value;


            // Only display the data if the user is a member of the "UserAdminPeeps" group.
            const string UserAdminPeeps = "af5ea704-8e00-4765-b289-ebdc030ace71"; //UserAdminPeeps

            // Ensure you turn on the sending of group claims in the token else the following line won't work
            // https://www.simple-talk.com/cloud/security-and-compliance/azure-active-directory-part-4-group-claims/

            Claim groupDevTestClaim = ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == "groups" && c.Value.Equals(UserAdminPeeps, StringComparison.CurrentCultureIgnoreCase));

            if (groupDevTestClaim !=null)
            {
                return View(users.value);
            }
            else
            {
                return View(new List<WebApplication6.Models.User>()); // return an empty set
            }



            
        }

    }
}