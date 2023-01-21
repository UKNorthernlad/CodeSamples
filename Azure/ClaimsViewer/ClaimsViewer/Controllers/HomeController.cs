using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClaimsViewer.Models;
using System.Text;

namespace ClaimsViewer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var claims = User.Claims.ToList();

            StringBuilder b = new StringBuilder();

            foreach (var claim in claims)
            {
                b.Append("Type: " + claim.Type + ", Value: " + claim.Value + "<br/>");
            }

            ViewBag.Claims = b.ToString();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
