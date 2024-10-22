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
            try
            {
                Session["userReg"] = nguoidung;

                // Thêm người dùng  mới
                db.Nguoidungs.Add(nguoidung);
                // Lưu lại vào cơ sở dữ liệu
                db.SaveChanges();
                // Nếu dữ liệu đúng thì trả về trang đăng nhập
                if (ModelState.IsValid)
                {
                    //return RedirectToAction("Dangnhap");
                    ViewBag.RegOk = "Đăng kí thành công. Đăng nhập ngay";
                    ViewBag.isReg = true;
                    return View("Dangky");

                }
                else
                {
                    return View("Dangky");
                }

            }
            catch
            {
                return View();
            }
        }

        public ActionResult Dangnhap()
        {
            return View();

        }


        [HttpPost]
        public ActionResult Dangnhap(FormCollection userlog)
        {
            string userMail = userlog["userMail"].ToString();
            string password = userlog["password"].ToString();
            var islogin = db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(userMail) && x.Matkhau.Equals(password));

            if (islogin != null)
            {
                if (userMail == "Admin@gmail.com")
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Admin/Home");
                }
                else
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Fail = "Tài khoản hoặc mật khẩu không chính xác.";
                return View("Dangnhap");
            }

        }
        public ActionResult DangXuat()
        {
            Session["use"] = null;
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Profile( int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoiDung = db.Nguoidungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

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
            ViewBag.IDQuyen = new SelectList(db.PhanQuyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }

        // POST: Admin/Nguoidungs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,Hoten,Email,Dienthoai,Matkhau,IDQuyen, Anhdaidien,Diachi")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
                db.SaveChanges();
                //@ViewBag.show = "Chỉnh sửa hồ sơ thành công";
                //return View(nguoidung);
                return RedirectToAction("Profile", new { id = nguoidung.MaNguoiDung });

            }
            ViewBag.IDQuyen = new SelectList(db.PhanQuyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }
        // Thêm Quên mật khẩu
        // Action hiển thị form quên mật khẩu
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // Action xử lý logic quên mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Vui lòng nhập email!";
                return View();
            }

            // Tìm kiếm người dùng qua email
            var user = db.Nguoidungs.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Email này không tồn tại trong hệ thống!";
                return View();
            }

            // Tạo mã xác thực hoặc token để khôi phục mật khẩu
            var resetToken = Guid.NewGuid().ToString();
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiration = DateTime.Now.AddHours(1); // Token hết hạn sau 1 giờ
            db.SaveChanges();

            // Gửi email với liên kết để đặt lại mật khẩu (có chứa token)
            string resetLink = Url.Action("ResetPassword", "User", new { token = resetToken }, Request.Url.Scheme);
            string emailBody = $"Vui lòng nhấn vào liên kết này để đặt lại mật khẩu: <a href='{resetLink}'>Đặt lại mật khẩu</a>";
            // Bạn cần tích hợp dịch vụ gửi email ở đây (SendEmailToUser(user.Email, emailBody))

            ViewBag.Message = "Một liên kết khôi phục mật khẩu đã được gửi đến email của bạn.";
            return View();
        }

        // Action hiển thị form để đặt lại mật khẩu
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            // Tìm người dùng qua token
            var user = db.Nguoidungs.FirstOrDefault(u => u.ResetPasswordToken == token && u.ResetPasswordTokenExpiration > DateTime.Now);
            if (user == null)
            {
                ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("ForgotPassword");
            }

            return View(new ResetPasswordModel { Token = token });
        }

        // Action xử lý việc đặt lại mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm người dùng qua token
            var user = db.Nguoidungs.FirstOrDefault(u => u.ResetPasswordToken == model.Token && u.ResetPasswordTokenExpiration > DateTime.Now);
            if (user == null)
            {
                ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("ForgotPassword");
            }

            // Đặt lại mật khẩu mới
            user.Matkhau = model.NewPassword; // Hash lại mật khẩu trước khi lưu
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiration = null;
            db.SaveChanges();

            ViewBag.Message = "Mật khẩu của bạn đã được cập nhật thành công.";
            return RedirectToAction("Dangnhap");
        }
        //
        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }
    }
}