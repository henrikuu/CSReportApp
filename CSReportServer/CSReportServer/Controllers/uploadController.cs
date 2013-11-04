using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace CSReportServer.Controllers
{
    public class uploadController : Controller
    {
        //
        // GET: /upload/

        private bool success = false;

        public ActionResult Index()
        {

            try
            {
                foreach (string fileInfo in Request.Files)
                {
                    string savePath = AppDomain.CurrentDomain.BaseDirectory + "\\uploadFolder";
                    string filename = Request.Files[fileInfo].FileName;

                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);

                    Request.Files[fileInfo].SaveAs(savePath + "\\" + filename);

                    success = true;
                }
            }
            catch (HttpException e)
            {
                Console.WriteLine("HttpException: " + e.Message);
            }

            if (success)
                this.ViewData.Add("uploadSuccess", true);
            else
                this.ViewData.Add("uploadSuccess", false);

            return View();
        }

    }
}
