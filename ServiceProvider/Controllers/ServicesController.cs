using ServiceProvider.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServiceProvider.Controllers
{
    [RoutePrefix("api/services")]
    public class ServicesController : ApiController
    {
        [Route("addtwonumbers/{operand1}/{operand2}")]
        [Route("addtwonumbers")]
        [HttpGet]
        [ServiceAuthentication]
        public IHttpActionResult AddTwoNumbers(int operand1, int operand2)
        {
            return Ok(operand1 + operand2);
        }

        [Route("addthreenumbers/{operand1}/{operand2}/{operand3}")]
        [Route("addthreenumbers")]
        [HttpGet]
        [ServiceAuthentication]
        public IHttpActionResult AddThreeNumbers(int operand1, int operand2,int operand3)
        {
            return Ok(operand1 + operand2 + operand3);
        }

        [Route("multwonumbers/{operand1}/{operand2}")]
        [Route("multwonumbers")]
        [HttpGet]
        [ServiceAuthentication]
        public IHttpActionResult MulTwoNumbers(int operand1, int operand2)
        {
            return Ok(operand1 * operand2);
        }

        [Route("multhreenumbers/{operand1}/{operand2}/{operand3}")]
        [Route("mullthreenumbers")]
        [HttpGet]
        [ServiceAuthentication]
        public IHttpActionResult MulThreeNumbers(int operand1, int operand2, int operand3)
        {
            return Ok(operand1 * operand2 * operand3);
        }
    }
}
