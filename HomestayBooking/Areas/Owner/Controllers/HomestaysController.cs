using System.Linq;
using System.Web.Mvc;
using HomestayBooking.Models;
using System.Data.Entity;

namespace HomestayBooking.Areas.Owner.Controllers
{
    public class HomestaysController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var db = new HomestayBookingEntities();
            var myHomestays = db.Homestay.Include(h => h.Account).Where(h => h.HostID == ownerId).ToList();
            return View(myHomestays);
        }

        public ActionResult Details(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var db = new HomestayBookingEntities();
            var homestay = db.Homestay.Include(h => h.Account).FirstOrDefault(h => h.HomestayID == id && h.HostID == ownerId);
            
            if (homestay == null)
            {
                return HttpNotFound();
            }
            
            return View(homestay);
        }

        public ActionResult Create()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Address,Description,Status")] Homestay homestay)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            if (ModelState.IsValid)
            {
                int ownerId = (int)Session["UserID"];
                homestay.HostID = ownerId;
                homestay.CreatedAt = System.DateTime.Now;
                
                var db = new HomestayBookingEntities();
                db.Homestay.Add(homestay);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(homestay);
        }

        public ActionResult Edit(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var db = new HomestayBookingEntities();
            var homestay = db.Homestay.FirstOrDefault(h => h.HomestayID == id && h.HostID == ownerId);
            
            if (homestay == null)
            {
                return HttpNotFound();
            }
            
            return View(homestay);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HomestayID,Name,Address,Description,Status")] Homestay homestay)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            if (ModelState.IsValid)
            {
                int ownerId = (int)Session["UserID"];
                var db = new HomestayBookingEntities();
                var existingHomestay = db.Homestay.FirstOrDefault(h => h.HomestayID == homestay.HomestayID && h.HostID == ownerId);
                
                if (existingHomestay == null)
                {
                    return HttpNotFound();
                }
                
                existingHomestay.Name = homestay.Name;
                existingHomestay.Address = homestay.Address;
                existingHomestay.Description = homestay.Description;
                existingHomestay.Status = homestay.Status;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(homestay);
        }

        public ActionResult Delete(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var db = new HomestayBookingEntities();
            var homestay = db.Homestay.FirstOrDefault(h => h.HomestayID == id && h.HostID == ownerId);
            
            if (homestay == null)
            {
                return HttpNotFound();
            }
            
            return View(homestay);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var db = new HomestayBookingEntities();
            var homestay = db.Homestay.FirstOrDefault(h => h.HomestayID == id && h.HostID == ownerId);
            
            if (homestay == null)
            {
                return HttpNotFound();
            }
            // Lấy tất cả Room thuộc Homestay
            var rooms = db.Rooms.Where(r => r.HomestayID == id).ToList();
            // Nếu có Room nào có Booking thì không cho xóa
            bool hasBooking = rooms.Any(r => r.Bookings != null && r.Bookings.Count > 0);
            if (hasBooking)
            {
                TempData["DeleteError"] = "Không thể xóa Homestay vì có phòng đã có đơn đặt.";
                return RedirectToAction("Delete", new { id = id });
            }
            // Xóa hết Room liên quan (nếu có)
            foreach (var room in rooms)
            {
                // Xóa RoomImages trước nếu có
                var images = db.RoomImages.Where(img => img.RoomID == room.RoomID).ToList();
                foreach (var img in images)
                {
                    db.RoomImages.Remove(img);
                }
                db.Rooms.Remove(room);
            }
            db.SaveChanges();
            // Xóa Homestay
            db.Homestay.Remove(homestay);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
} 