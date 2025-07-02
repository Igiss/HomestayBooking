using System.Web.Mvc;
using HomestayBooking.Filters;

namespace HomestayBooking.Areas.Owner.Controllers
{
    [AuthenticationFilter("owner")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Trang chủ chủ homestay";
            ViewBag.UserRole = Session["UserRole"];
            ViewBag.Username = Session["Username"];
            return View();
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
    }
}