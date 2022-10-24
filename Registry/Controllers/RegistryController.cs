using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Registry.Controllers
{
    public class RegistryController : Controller
    {
        // GET: Registry
        public ActionResult Index()
        {
            return View();
        }
    }
}