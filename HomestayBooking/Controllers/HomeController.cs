using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HomestayBooking.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Nếu đã đăng nhập, chuyển hướng theo role
            if (User.Identity.IsAuthenticated)
            {
                string userRole = Session["UserRole"]?.ToString();

                switch (userRole?.ToLower())
                {
                    case "admin":
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    case "owner":
                        return RedirectToAction("Index", "Home", new { area = "Owner" });
                    case "customer":
                        // Customer ở lại trang chủ
                        ViewBag.Title = "Trang chủ";
                        ViewBag.UserRole = Session["UserRole"];
                        ViewBag.Username = Session["Username"];
                        return View();      
                    default:
                        // Nếu role không hợp lệ, đăng xuất và hiển thị trang chủ
                        FormsAuthentication.SignOut();
                        Session.Clear();
                        ViewBag.Title = "Trang chủ";
                        return View();
                }
            }

            // Nếu chưa đăng nhập, hiển thị trang chủ công khai
            ViewBag.Title = "Trang chủ";
            return View();
        }

        public ActionResult Unauthorized()
        {
            ViewBag.Title = "Không có quyền truy cập";
            return View();
        }
    }
}
