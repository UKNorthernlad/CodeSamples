using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWithBearerTokenAuth.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            List<string> messages = new List<string>();

            string message1 = "Hello " + (User.FindFirst(ClaimTypes.Name))?.Value;
            string message2 = "Hello " + (User.FindFirst(ClaimTypes.NameIdentifier))?.Value;
            messages.Add(message1);
            messages.Add(message2);

            foreach (var claim in User.Claims)
            {
                messages.Add(string.Format("{0} = {1}", claim.Type, claim.Value));
            }

            // NameIdentified is the "sub" claim - for AAD it's a GUID for the user object or App's Service Principal.
            // For Microsoft accounts, it's a string of numbers and letters.
            switch ((User.FindFirst(ClaimTypes.NameIdentifier))?.Value)
            {
                case "891991bf-9346-4953-99d4-9fe76cc225e3":
                    messages.Add("You are team A.");
                    break;
                case "24216fb1-3039-4e18-9df1-89529c0fa36f":
                    messages.Add("You are team B.");
                    break;
                case "BHGfNlwel9WBsjTPMdOSl69TaOaU0XZfblEZyBPaNMk":
                    messages.Add("You are Brian King.");
                    break;
                default:
                    messages.Add(string.Format("You are unknown to the system. NameIdentifier={0}", (User.FindFirst(ClaimTypes.NameIdentifier))?.Value));
                    break;
            }

            return messages.ToArray();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return id.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            // For more information on protecting this API from Cross Site Request Forgery (CSRF) attacks, see https://go.microsoft.com/fwlink/?LinkID=717803
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            // For more information on protecting this API from Cross Site Request Forgery (CSRF) attacks, see https://go.microsoft.com/fwlink/?LinkID=717803
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            // For more information on protecting this API from Cross Site Request Forgery (CSRF) attacks, see https://go.microsoft.com/fwlink/?LinkID=717803
        }
    }
}
