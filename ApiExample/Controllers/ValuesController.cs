using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;

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

            var subjectClaim = caller.FindFirst("sub");
            if (subjectClaim != null)
            {
                return Json(new
                {
                    message = "OK user",
                    client = caller.FindFirst("client_id").Value,
                    subject = subjectClaim.Value
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
                    Nope = "Nothing accessable by anything detected"
                });
            }
        }

        private async Task<string> CallApiForData()
        {
            var discoveryClient = new DiscoveryClient("http://localhost:52590/");

            var metaDataResp = await discoveryClient.GetAsync();

            var userInfoClient = new UserInfoClient(metaDataResp.UserInfoEndpoint);

            var accessToken = await HttpContext.Authentication
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await userInfoClient.GetAsync(accessToken);

            if (response.IsError)
            {
                throw new Exception("User info endpoint unavailable", response.Exception);
            }

            var address = response.Claims.FirstOrDefault(x => x.Type == "address")?.Value;

            return address;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
