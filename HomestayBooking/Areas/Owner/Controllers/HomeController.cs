using System.Web.Mvc;
using HomestayBooking.Filters;
using System.Linq;
using HomestayBooking.Models;
using System.Data.Entity;
using System.Runtime.CompilerServices;

namespace HomestayBooking.Areas.Owner.Controllers
{
    [AuthenticationFilter("owner", "host")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Trang chủ chủ homestay";
            ViewBag.UserRole = Session["UserRole"];
            ViewBag.Username = Session["Username"];
            int ownerId = Session["UserID"] != null ? (int)Session["UserID"] : 0;
            var db = new HomestayBookingEntities();
            ViewBag.TotalHomestay = db.Homestay.Count(h => h.HostID == ownerId);
            ViewBag.TotalRoom = db.Rooms.Count(r => r.Homestay.HostID == ownerId);
            ViewBag.TotalBooking = db.Bookings.Count(b => b.Rooms.Homestay.HostID == ownerId);
            ViewBag.TotalReview = db.Reviews.Count(r => r.Bookings.Rooms.Homestay.HostID == ownerId);
            ViewBag.TotalEarnings = db.Payments.Where(p => p.Bookings.Rooms.Homestay.HostID == ownerId).Select(p => (decimal?)p.Amount).Sum() ?? 0;
            
            // Lấy danh sách homestay của owner
            var homestays = db.Homestay.Where(h => h.HostID == ownerId).ToList();
            
            // Lấy danh sách booking gần đây
            var recentBookings = db.Bookings
                .Include(b => b.Rooms)
                .Include(b => b.Rooms.Homestay)
                .Include(b => b.Account)
                .Where(b => b.Rooms.Homestay.HostID == ownerId)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToList();
            
            ViewBag.RecentBookings = recentBookings;
            
            return View(homestays);
        }

        public ActionResult MyHomestays()
        {
            ViewBag.Title = "Homestay của tôi";
            return View();
        }

        public ActionResult Bookings()
        {
            ViewBag.Title = "Quản lý đặt phòng";
            return View();
        }

        public ActionResult Earnings()
        {
            ViewBag.Title = "Thu nhập";
            return View();
        }

        public ActionResult Unauthorized()
        {
            ViewBag.Title = "Không có quyền truy cập";
            return View();
        }
    }
}