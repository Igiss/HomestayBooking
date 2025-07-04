using System.Web.Mvc;
using HomestayBooking.Filters;
using System.Linq;
using HomestayBooking.Models;

namespace HomestayBooking.Areas.Admin.Controllers
{
    [AuthenticationFilter("admin")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new HomestayBookingEntities();
            ViewBag.Title = "Trang quản trị";
            ViewBag.UserRole = Session["UserRole"];
            ViewBag.Username = Session["Username"];
            ViewBag.TotalHomestay = db.Homestay.Count();
            ViewBag.TotalRoom = db.Rooms.Count();
            ViewBag.TotalAccount = db.Account.Count();
            ViewBag.TotalBooking = db.Bookings.Count();
            ViewBag.BookingPending = db.Bookings.Count(b => b.Status == "pending");
            ViewBag.BookingConfirmed = db.Bookings.Count(b => b.Status == "confirmed");
            ViewBag.BookingCancelled = db.Bookings.Count(b => b.Status == "cancelled");
            return View();
        }

        public ActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            return View();
        }

        public ActionResult Users()
        {
            ViewBag.Title = "Quản lý người dùng";
            return View();
        }

        public ActionResult Reports()
        {
            ViewBag.Title = "Báo cáo";
            return View();
        }

        public ActionResult RedirectToAccounts()
        {
            return RedirectToAction("Index", "Accounts", new { area = "Admin" });
        }

        public ActionResult Unauthorized()
        {
            ViewBag.Title = "Không có quyền truy cập";
            return View();
        }
    }
}