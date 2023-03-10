using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzureADAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;

namespace AzureADAuthentication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserLoggedInMessage = "You are now authenticated using " + User.Identity.AuthenticationType;
                ViewBag.UserClaims = "";
                // " and you are called " + (User.FindFirst(ClaimTypes.Name))?.Value + " or " + (User.FindFirst(ClaimTypes.NameIdentifier))?.Value + " if using JWT";
            }
            return View();
        }

        public async Task<IActionResult> AADLogout()
        {
            await HttpContext.SignOutAsync(AzureADDefaults.AuthenticationScheme);
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
