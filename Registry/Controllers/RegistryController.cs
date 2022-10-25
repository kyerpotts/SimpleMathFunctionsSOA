using APIEndpoint;
using Registry.Models;
using Registry.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Registry.Controllers
{
    [RoutePrefix("api/registry")]
    public class RegistryController : ApiController
    {
        string endpointFilePath = "APIEndpoints.txt";

        [Route("publish/{jEndpoint}")]
        [Route("publish")]
        [HttpPost]
        [ServiceAuthentication]
        public IHttpActionResult Publish(string jEndpoint)
        {
            try
            {
                RegistryBusinessLayer.WriteAPIEndpointToFile(endpointFilePath, jEndpoint);
                return Ok(new ReturnStatus { Status = "Accepted", Reason = "Endpoint published to registry successfully." });
            }
            catch (IOException)
            {
                return Content(HttpStatusCode.BadRequest, new ReturnStatus { Status = "Denied", Reason = "An error occured when processing the request" });
            }
        }

        [Route("search/{searchterm}")]
        [Route("search")]
        [HttpGet]
        [ServiceAuthentication]
        public IHttpActionResult Search(string searchterm)
        {
            List<EndpointObject> returnList;
            try
            {
                returnList = RegistryBusinessLayer.FindEndpoints(endpointFilePath, searchterm);
                return Ok(returnList);
            }
            catch (IOException)
            {
                return Content(HttpStatusCode.BadRequest, new ReturnStatus { Status = "Denied", Reason = "An error occured when processing the request" });
            }
        }

        [Route("allservices")]
        [HttpGet]
        [ServiceAuthentication]
        public IHttpActionResult AllServices(string searchterm)
        {
            try
            {
                List<EndpointObject> returnList = RegistryBusinessLayer.GetAllEndpoints(endpointFilePath);
                return Ok(returnList);
            }
            catch (IOException)
            {
                return Content(HttpStatusCode.BadRequest, new ReturnStatus { Status = "Denied", Reason = "An error occured when processing the request" });
            }
        }

        [Route("unpublish/{jEndpoint}")]
        [Route("unpublish")]
        [HttpDelete]
        [ServiceAuthentication]
        public IHttpActionResult Unpublish(string jEndpoint)
        {
            try
            {
                List<EndpointObject> returnList = RegistryBusinessLayer.GetAllEndpoints(endpointFilePath);
                return Ok(returnList);
            }
            catch (IOException)
            {
                return Content(HttpStatusCode.BadRequest, new ReturnStatus { Status = "Denied", Reason = "An error occured when processing the request" });
            }
        }
    }
}
