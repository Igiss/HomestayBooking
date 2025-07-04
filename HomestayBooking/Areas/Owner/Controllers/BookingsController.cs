using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomestayBooking.Models;
using HomestayBooking.Filters;

namespace HomestayBooking.Areas.Owner.Controllers
{
    public class BookingsController : Controller
    {
        private HomestayBookingEntities db = new HomestayBookingEntities();

        // GET: Owner/Bookings
        public ActionResult Index()
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            
            // Lấy tất cả đơn đặt phòng của các homestay thuộc về owner này
            var bookings = db.Bookings
                .Include(b => b.Account)
                .Include(b => b.Rooms)
                .Include(b => b.Rooms.Homestay)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .Where(b => b.Rooms.Homestay.HostID == ownerId)
                .OrderByDescending(b => b.BookingID)
                .ToList();
            
            return View(bookings);
        }

        // GET: Owner/Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            
            var booking = db.Bookings
                .Include(b => b.Account)
                .Include(b => b.Rooms)
                .Include(b => b.Rooms.Homestay)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .FirstOrDefault(b => b.BookingID == id && b.Rooms.Homestay.HostID == ownerId);
            
            if (booking == null)
                return HttpNotFound();
            
            return View(booking);
        }

        // GET: Owner/Bookings/Create
        public ActionResult Create()
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            
            // Chỉ cho phép chọn phòng của homestay thuộc về owner này
            ViewBag.CustomerID = new SelectList(db.Account, "AccountID", "UserName");
            ViewBag.RoomID = new SelectList(db.Rooms.Where(r => r.Homestay.HostID == ownerId), "RoomID", "Name");
            
            return View();
        }

        // POST: Owner/Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,CustomerID,RoomID,CheckInDate,CheckOutDate,Status,CreatedAt")] Bookings booking)
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            
            // Kiểm tra xem phòng có thuộc về homestay của owner này không
            var room = db.Rooms.Include(r => r.Homestay).FirstOrDefault(r => r.RoomID == booking.RoomID && r.Homestay.HostID == ownerId);
            if (room == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                booking.CreatedAt = DateTime.Now;
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Account, "AccountID", "UserName", booking.CustomerID);
            ViewBag.RoomID = new SelectList(db.Rooms.Where(r => r.Homestay.HostID == ownerId), "RoomID", "Name", booking.RoomID);
            
            return View(booking);
        }

        // GET: Owner/Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            
            var booking = db.Bookings
                .Include(b => b.Rooms)
                .Include(b => b.Rooms.Homestay)
                .FirstOrDefault(b => b.BookingID == id && b.Rooms.Homestay.HostID == ownerId);
            
            if (booking == null)
                return HttpNotFound();
            
            ViewBag.CustomerID = new SelectList(db.Account, "AccountID", "UserName", booking.CustomerID);
            ViewBag.RoomID = new SelectList(db.Rooms.Where(r => r.Homestay.HostID == ownerId), "RoomID", "Name", booking.RoomID);
            
            return View(booking);
        }

        // POST: Owner/Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,CustomerID,RoomID,CheckInDate,CheckOutDate,Status,CreatedAt")] Bookings booking)
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            
            // Kiểm tra xem booking có thuộc về homestay của owner này không
            var existingBooking = db.Bookings
                .Include(b => b.Rooms)
                .Include(b => b.Rooms.Homestay)
                .FirstOrDefault(b => b.BookingID == booking.BookingID && b.Rooms.Homestay.HostID == ownerId);
            
            if (existingBooking == null)
                return HttpNotFound();

            // Kiểm tra xem phòng mới có thuộc về homestay của owner này không
            var room = db.Rooms.Include(r => r.Homestay).FirstOrDefault(r => r.RoomID == booking.RoomID && r.Homestay.HostID == ownerId);
            if (room == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                existingBooking.CustomerID = booking.CustomerID;
                existingBooking.RoomID = booking.RoomID;
                existingBooking.CheckInDate = booking.CheckInDate;
                existingBooking.CheckOutDate = booking.CheckOutDate;
                existingBooking.Status = booking.Status;
                existingBooking.CreatedAt = booking.CreatedAt;
                
                db.Entry(existingBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.CustomerID = new SelectList(db.Account, "AccountID", "UserName", booking.CustomerID);
            ViewBag.RoomID = new SelectList(db.Rooms.Where(r => r.Homestay.HostID == ownerId), "RoomID", "Name", booking.RoomID);
            
            return View(booking);
        }

        // GET: Owner/Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            
            var booking = db.Bookings
                .Include(b => b.Account)
                .Include(b => b.Rooms)
                .Include(b => b.Rooms.Homestay)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .FirstOrDefault(b => b.BookingID == id && b.Rooms.Homestay.HostID == ownerId);
            
            if (booking == null)
                return HttpNotFound();
            
            return View(booking);
        }

        // POST: Owner/Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            
            var booking = db.Bookings
                .Include(b => b.Rooms)
                .Include(b => b.Rooms.Homestay)
                .FirstOrDefault(b => b.BookingID == id && b.Rooms.Homestay.HostID == ownerId);
            
            if (booking == null)
                return HttpNotFound();
            
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Owner/Bookings/UpdateStatus
        [HttpPost]
        public ActionResult UpdateStatus(int bookingId, string status)
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });
            
            int ownerId = (int)Session["UserID"];
            
            var booking = db.Bookings
                .Include(b => b.Rooms)
                .Include(b => b.Rooms.Homestay)
                .FirstOrDefault(b => b.BookingID == bookingId && b.Rooms.Homestay.HostID == ownerId);
            
            if (booking == null)
                return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng" });
            
            booking.Status = status;
            db.SaveChanges();
            
            return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
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