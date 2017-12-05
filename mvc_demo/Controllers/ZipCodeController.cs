using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_demo.Models;
using PagedList;


namespace mvc_demo.Controllers
{
    public class ZipCodeController : Controller
    {
        private TaiwanZipEF db = new TaiwanZipEF();
        
        // GET: ZipCode
        public ActionResult Index(int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;

            var query = db.TaiwanZipCode.OrderBy(x => x.Sequence).ThenBy(x => x.ID).ThenBy(x => x.Zip);
            var result = query.ToPagedList(currentPage, 10);

            return View(result);
        }
    }
}