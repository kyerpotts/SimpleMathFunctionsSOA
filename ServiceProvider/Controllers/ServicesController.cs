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
        [Route("addtwonumbers/{firstnum}/{secondnum}")]
        [Route("addtwonumbers")]
        [HttpGet]
        [ServiceAuthentication]
        public IHttpActionResult AddTwoNumbers(int firstnum, int secondnum)
        {
            return Ok(firstnum + secondnum);
        }

        [Route("addthreenumbers/{firstnum}/{secondnum}/{thirdnum}")]
        [Route("addthreenumbers")]
        [HttpGet]
        public IHttpActionResult AddThreeNumbers(int firstnum, int secondnum, int thirdnum)
        {
            return Ok(firstnum + secondnum + thirdnum);
        }

        [Route("multwonumbers/{firstnum}/{secondnum}")]
        [Route("multwonumbers")]
        [HttpGet]
        public IHttpActionResult MulTwoNumbers(int firstnum, int secondnum)
        {
            return Ok(firstnum * secondnum);
        }

        [Route("multhreenumbers/{firstnum}/{secondnum}/{thirdnum}")]
        [Route("mullthreenumbers")]
        [HttpGet]
        public IHttpActionResult MulThreeNumbers(int firstnum, int secondnum, int thirdnum)
        {
            return Ok(firstnum * secondnum * thirdnum);
        }
    }
}
