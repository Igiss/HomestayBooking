using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net;
using HomestayBooking.Models;

namespace HomestayBooking.Controllers
{
    public class HomeController : Controller
    {
        private HomestayBookingEntities db = new HomestayBookingEntities();

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
                        ViewBag.Title = "Trang chủ";
                        ViewBag.UserRole = Session["UserRole"];
                        ViewBag.Username = Session["Username"];
                        break;
                    default:
                        FormsAuthentication.SignOut();
                        Session.Clear();
                        ViewBag.Title = "Trang chủ";
                        break;
                }
            }

            // Lấy danh sách Homestay có ít nhất 1 phòng khả dụng
            var homestays = db.Homestay.Include(h => h.Rooms)
                .Where(h => h.Rooms.Any(r => r.Available == true))
                .OrderByDescending(h => h.HomestayID)
                .ToList();
            ViewBag.Homestays = homestays;

            ViewBag.Title = "Trang chủ";
            return View();
        }

        public ActionResult Unauthorized()
        {
            ViewBag.Title = "Không có quyền truy cập";
            return View();
        }

        // GET: Rooms
        public ActionResult Rooms()
        {
            var rooms = db.Rooms.Include(r => r.RoomImages)
                               .Include(r => r.Homestay)
                               .Where(r => r.Available == true)
                               .ToList();
            return View(rooms);
        }

        // GET: RoomDetail/5
        public ActionResult RoomDetail(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var room = db.Rooms.Include(r => r.RoomImages)
                              .Include(r => r.Homestay)
                              .FirstOrDefault(r => r.RoomID == id);
            if (room == null)
                return HttpNotFound();
            
            return View(room);
        }

        // GET: HomestayDetail/5
        public ActionResult HomestayDetail(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var homestay = db.Homestay.Include(h => h.Rooms.Select(r => r.RoomImages))
                                      .FirstOrDefault(h => h.HomestayID == id);
            if (homestay == null)
                return HttpNotFound();
            return View(homestay);
        }

        // POST: PostReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostReview(int homestayId, int rating, string comment)
        {
            if (Session["UserID"] == null)
            {
                TempData["ReviewError"] = "Bạn cần đăng nhập để đánh giá.";
                return RedirectToAction("HomestayDetail", new { id = homestayId });
            }
            int userId = (int)Session["UserID"];
            // Tìm booking của user tại homestay này
            var booking = db.Bookings.FirstOrDefault(b => b.CustomerID == userId && b.Rooms.HomestayID == homestayId);
            if (booking == null)
            {
                TempData["ReviewError"] = "Bạn cần đặt phòng tại homestay này để đánh giá.";
                return RedirectToAction("HomestayDetail", new { id = homestayId });
            }
            // Kiểm tra đã đánh giá chưa
            var existedReview = db.Reviews.FirstOrDefault(r => r.BookingID == booking.BookingID);
            if (existedReview != null)
            {
                TempData["ReviewError"] = "Bạn đã đánh giá cho booking này rồi.";
                return RedirectToAction("HomestayDetail", new { id = homestayId });
            }
            // Lưu đánh giá
            var review = new Reviews
            {
                BookingID = booking.BookingID,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };
            db.Reviews.Add(review);
            db.SaveChanges();
            TempData["ReviewSuccess"] = "Cảm ơn bạn đã đánh giá!";
            return RedirectToAction("HomestayDetail", new { id = homestayId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
