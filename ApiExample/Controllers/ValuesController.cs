using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiExample.Controllers
{
    [Route("test")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public JsonResult Get()
        {
            var caller = User as ClaimsPrincipal;

            // Confirms that we are authorised - sub confirms a subject (User login) as apposed to a client login.
            var subjectClaim = caller.FindFirst("sub");
            if (subjectClaim != null)
            {
                var moo = CallApiForData().GetAwaiter().GetResult();
                return Json(new
                {
                    message = "OK user",
                    client = caller.FindFirst("client_id").Value,
                    subject = subjectClaim.Value,
                    Address = moo // Value recevied from Auth API - not in token
                });
            }
            else if (caller.FindFirst("client_id") != null)
            {
                return Json(new
                {
                    message = "OK computer",
                    client = caller.FindFirst("client_id").Value
                });
            }
            else
            {
                return Json(new
                {
                    Nope = "[You are not authenticated]"
                });
            }
        }

        // Call the Auth Server for user access details
        private async Task<string> CallApiForData()
        {
            var discoveryClient = new DiscoveryClient("http://localhost:52590/");

            var metaDataResp = await discoveryClient.GetAsync();
            var accessToken = await HttpContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfoClient = new UserInfoClient(metaDataResp.UserInfoEndpoint);

            var response = await userInfoClient.GetAsync(accessToken);
            var claims = response.Claims;

            if (response.IsError)
            {
                throw new Exception("User info endpoint unavailable", response.Exception);
            }

            var address = response.Claims.FirstOrDefault(x => x.Type == "address")?.Value;

            return address;
        }
    }
}
