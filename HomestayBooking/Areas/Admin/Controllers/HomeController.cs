using System.Web.Mvc;
using HomestayBooking.Filters;

namespace HomestayBooking.Areas.Admin.Controllers
{
    [AuthenticationFilter("admin")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Trang quản trị";
            ViewBag.UserRole = Session["UserRole"];
            ViewBag.Username = Session["Username"];
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
    }
}