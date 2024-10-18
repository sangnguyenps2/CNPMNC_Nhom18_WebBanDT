using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
namespace Ictshop.Controllers
{
    public class UserController : Controller
    {
        Qlbanhang db = new Qlbanhang();

        // ĐĂNG KÝ
        public ActionResult Dangky()
        {
            return View();
        }

        // ĐĂNG KÝ PHƯƠNG THỨC POST
        [HttpPost]
        public ActionResult Dangky(Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại chưa
                if (db.Nguoidungs.Any(x => x.Email == nguoidung.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(nguoidung);
                }

                // Thêm người dùng mới
                db.Nguoidungs.Add(nguoidung);
                if (db.SaveChanges() > 0)
                {
                    ViewBag.RegOk = "Đăng ký thành công!";
                    ViewBag.isReg = true;
                    return RedirectToAction("Dangnhap");
                }
            }

            // Nếu không hợp lệ, trả về view với model để hiển thị lỗi
            return View(nguoidung);
        } 

        // Đăng nhập
        public ActionResult Dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangnhap(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var foundUser = db.Nguoidungs.FirstOrDefault(u => u.Email == loginModel.Email && u.Matkhau == loginModel.Matkhau);
                if (foundUser == null)
                {
                    ViewBag.ErrorInfo = "Email hoặc mật khẩu không đúng.";
                    return View(loginModel);
                }


                Session["IdUser"] = foundUser.MaNguoiDung;
                Session["use"] = foundUser; 

                return RedirectToAction("Index", "Home");
            }
            return View(loginModel);
        }



        // Đăng xuất
        public ActionResult DangXuat()
        {
            Session.Clear(); // Xóa tất cả session
            return RedirectToAction("Index", "Home");
        }

        // Cập nhật hồ sơ
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            return View(nguoidung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profile", new { id = nguoidung.MaNguoiDung });
            }
            return View(nguoidung);
        }

        
    }
}