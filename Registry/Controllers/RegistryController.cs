using APIEndpoint;
using Newtonsoft.Json;
using Registry.Models;
using Registry.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace Registry.Controllers
{
    [RoutePrefix("api/registry")]
    public class RegistryController : ApiController
    {
        string endpointFilePath = (HostingEnvironment.MapPath("~/APIReg.txt"));

        [Route("publish/{jEndpoint}")]
        [Route("publish")]
        [HttpPost]
        [ServiceAuthentication]
        public IHttpActionResult Publish([FromBody] EndpointObject jEndpoint)
        {
            try
            {
                RegistryBusinessLayer.WriteAPIEndpointToFile(endpointFilePath, jEndpoint);
                return Ok(new ReturnStatus { Status = "Accepted", Reason = "Endpoint published to registry successfully." });
            }
            catch (IOException)
            {
                return BadRequest(JsonConvert.SerializeObject(new ReturnStatus { Status = "Denied", Reason = "An error occured while processing the request" }));
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
                return BadRequest(JsonConvert.SerializeObject(new ReturnStatus { Status = "Denied", Reason = "An error occured while processing the request" }));
            }
        }

        [Route("allservices")]
        [HttpGet]
        [ServiceAuthentication]
        public IHttpActionResult AllServices()
        {
            try
            {
                List<EndpointObject> returnList = RegistryBusinessLayer.GetAllEndpoints(endpointFilePath);
                return Ok(returnList);
            }
            catch (IOException)
            {
                return BadRequest(JsonConvert.SerializeObject(new ReturnStatus { Status = "Denied", Reason = "An error occured while processing the request" }));
            }
        }

        [Route("unpublish/{jEndpointName}")]
        [Route("unpublish")]
        [HttpDelete]
        [ServiceAuthentication]
        public IHttpActionResult Unpublish(string jEndpointName)
        {
            try
            {
                List<EndpointObject> returnList = RegistryBusinessLayer.GetAllEndpoints(endpointFilePath);
                if (RegistryBusinessLayer.DeleteEndpoint(endpointFilePath, jEndpointName))
                {
                    return Ok(new ReturnStatus {  Status = "Accepted", Reason = "Endpoint successfully unpublished."});
                }
                else
                {
                    return BadRequest(JsonConvert.SerializeObject(new ReturnStatus { Status = "Denied", Reason = "Could not unpublish endpoint" }));
                }
            }
            catch (IOException)
            {
                return BadRequest(JsonConvert.SerializeObject(new ReturnStatus { Status = "Denied", Reason = "An error occured while processing the request" }));
            }
        }
    }
}
