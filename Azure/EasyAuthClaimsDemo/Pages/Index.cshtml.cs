using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public String ClaimsList { get; set; }

        public String HeadersList { get; set; }


        public void OnGet()
        {
            //List<Claim> claims = User.Claims.Where(c => c.Type == "groups").ToList();
            // Display Claims for debugging
            var claims = User.Claims.ToList();
            StringBuilder claimsBuilder = new StringBuilder();
            foreach (var claim in claims)
            {
                claimsBuilder.Append("Type: " + claim.Type + ", Value: " + claim.Value + "<br/>");
            }
            
            ClaimsList = claimsBuilder.ToString();

            // Display Headers for debugging
            var headers = Request.Headers.ToList();
            StringBuilder headerBuilder = new StringBuilder();
            foreach (var header in headers)
            {
                claimsBuilder.Append("<b>" + header.Key + "</b> : " + header.Value + "<br/>");
            }

            HeadersList = claimsBuilder.ToString();

        }
    }
}