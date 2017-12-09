using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_demo.Controllers
{
    public class BootBoxController : Controller
    {
        // GET: BootBox
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BootBox()
        {
            return View();
        }
    }
}