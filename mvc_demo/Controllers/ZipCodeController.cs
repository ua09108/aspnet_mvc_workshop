using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_demo.Models;
using PagedList;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Configuration;
using System.IO;

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


        private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            JObject jo = new JObject();
            string result = string.Empty;

            if (file == null)
            {
                jo.Add("Result", false);
                jo.Add("Msg", "請上傳檔案!");
                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }
            if (file.ContentLength <= 0)
            {
                jo.Add("Result", false);
                jo.Add("Msg", "請上傳正確的檔案.");
                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }

            string fileExtName = Path.GetExtension(file.FileName).ToLower();

            if (!fileExtName.Equals(".xls", StringComparison.OrdinalIgnoreCase)
                &&
                !fileExtName.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                jo.Add("Result", false);
                jo.Add("Msg", "請上傳 .xls 或 .xlsx 格式的檔案");
                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }

            try
            {
                var uploadResult = this.FileUploadHandler(file);

                jo.Add("Result", !string.IsNullOrWhiteSpace(uploadResult));
                jo.Add("Msg", !string.IsNullOrWhiteSpace(uploadResult) ? uploadResult : "");

                result = JsonConvert.SerializeObject(jo);
            }
            catch (Exception ex)
            {
                jo.Add("Result", false);
                jo.Add("Msg", ex.Message);
                result = JsonConvert.SerializeObject(jo);
            }
            return Content(result, "application/json");
        }

        /// <summary>
        /// Files the upload handler.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">file;上傳失敗：沒有檔案！</exception>
        /// <exception cref="System.InvalidOperationException">上傳失敗：檔案沒有內容！</exception>
        private string FileUploadHandler(HttpPostedFileBase file)
        {
            string result;

            if (file == null)
            {
                throw new ArgumentNullException("file", "上傳失敗：沒有檔案！");
            }
            if (file.ContentLength <= 0)
            {
                throw new InvalidOperationException("上傳失敗：檔案沒有內容！");
            }

            try
            {
                string virtualBaseFilePath = Url.Content(fileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string newFileName = string.Concat(
                    DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    Path.GetExtension(file.FileName).ToLower());

                string fullFilePath = Path.Combine(Server.MapPath(fileSavedPath), newFileName);
                file.SaveAs(fullFilePath);

                result = newFileName;
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

    }
}