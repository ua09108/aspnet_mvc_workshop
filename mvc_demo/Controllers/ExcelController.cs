using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;

namespace mvc_demo.Controllers
{
    public class ExcelController : Controller
    {
        // GET: Excel
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if(postedFile != null)
            {
                string path = Server.MapPath("/Uploads/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;

                switch (extension)
                {
                    case ".xls":
                        conString = ConfigurationManager.ConnectionStrings["Testdb_2003"].ConnectionString;
                        break;
                    case ".xlsx":
                        conString = ConfigurationManager.ConnectionStrings["Testdb_2007"].ConnectionString;
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);


                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using(OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using(OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();

                            connExcel.Close();

                            connExcel.Open();
                            cmdExcel.CommandText = "select * from [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;

                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                conString = ConfigurationManager.ConnectionStrings["Testdb"].ConnectionString;
                using(SqlConnection con = new SqlConnection(conString))
                {
                    using(SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.mvc_customers";

                        //sqlBulkCopy.ColumnMappings.Add("Id", "CustomerId");
                        sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                        sqlBulkCopy.ColumnMappings.Add("Country", "Country");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }



            }

            return View();
        }
    }
}