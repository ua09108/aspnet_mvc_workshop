using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvc_demo.Controllers
{
    public class AjaxPostController : Controller
    {
        // GET: AjaxPost
        public ActionResult Index(string ViewName)
        {
            if (string.IsNullOrEmpty(ViewName))
            { return View(); }
            else
            {
                return View(ViewName);
            }

        }

        public ActionResult Update(List<Player> players)
        {
            return Json(players);
        }

        public class Player
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}