using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Qlbanhang db = new Qlbanhang();
            var userId = Session["IdUser"];
            string query = TempData["SearchQuery"] as string;
            IEnumerable<Sanpham> products;

            if (!string.IsNullOrEmpty(query))
            {
                products = db.Sanphams
                            .Where(p => p.Tensp.Contains(query) || p.Giatien.ToString().Contains(query))
                            .ToList();

                ViewBag.SearchQuery = query;

                if (!products.Any())
                {
                    ViewBag.Message = "Không có sản phẩm nào được tìm thấy.";
                }
            }
            else
            {
                products = db.Sanphams.ToList();
            }

            return View(products);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult SlidePartial()
        {
            return PartialView();
        }

        
    }
}
