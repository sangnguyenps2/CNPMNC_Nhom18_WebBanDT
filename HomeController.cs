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
            if (userId == null)
            {
                return RedirectToAction("Dangnhap", "User");
            }

            // Lấy tất cả sản phẩm từ cơ sở dữ liệu
            var products = db.Sanphams.ToList();

            // Đảm bảo ViewBag.SearchResults không null
            ViewBag.SearchResults = ViewBag.SearchResults ?? new List<Sanpham>();

            return View(products); // Truyền tất cả sản phẩm đến view
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
