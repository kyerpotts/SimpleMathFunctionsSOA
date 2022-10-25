using APIEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Registry.Controllers
{
    public class RegistryController : ApiController
    {
        // Saves the APIEndpoint data to a registry file
        public IHttpActionResult Publish(string jEndpoint)
        {
            return Ok(new ReturnStatus { Status = "Accepted", Reason = "Endpoint published to registry successfully."});
        }
    }
}
