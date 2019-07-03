using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CookieAuthentication.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace CookieAuthentication.Controllers
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

        public async Task<IActionResult> CookiesLogin(string returnUrl = null)
        {
            const string Issuer = "https://gov.uk";

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, "Billy", ClaimValueTypes.String, Issuer),
                new Claim(ClaimTypes.Surname, "Bunter", ClaimValueTypes.String, Issuer),
                new Claim(ClaimTypes.Country, "UK", ClaimValueTypes.String, Issuer),
                new Claim("Hero", "Danger Mouse", ClaimValueTypes.String) };

            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await AuthenticationHttpContextExtensions.SignInAsync(HttpContext, userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            return View();
        }

        public async Task<IActionResult> CookiesLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

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
