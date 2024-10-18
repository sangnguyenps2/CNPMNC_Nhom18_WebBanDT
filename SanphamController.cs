using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Controllers
{
    public class SanphamController : Controller
    {
        Qlbanhang db = new Qlbanhang();

        // GET: Sanpham
        public ActionResult dtiphonepartial()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 2).Take(4).ToList();
            return PartialView(ip);
        }

        public ActionResult dtsamsungpartial()
        {
            var ss = db.Sanphams.Where(n => n.Mahang == 1).Take(4).ToList();
            return PartialView(ss);
        }

        public ActionResult dtxiaomipartial()
        {
            var mi = db.Sanphams.Where(n => n.Mahang == 3).Take(4).ToList();
            return PartialView(mi);
        }

        // public ActionResult dttheohang()
        // {
        //     var mi = db.Sanphams.Where(n => n.Mahang == 5).Take(4).ToList();
        //     return PartialView(mi);
        // }

        public ActionResult XemChiTiet(int masp)
        {
            var sanpham = db.Sanphams.FirstOrDefault(s => s.Masp == masp);
            if (sanpham == null)
            {
                return HttpNotFound();
            }
            return View(sanpham);
        }

        public ActionResult Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                
                TempData["SearchQuery"] = query;
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home"); 
        }
    }
}
