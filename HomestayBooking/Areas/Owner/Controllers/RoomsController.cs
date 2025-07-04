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
    public class RoomsController : Controller
    {
        private HomestayBookingEntities db = new HomestayBookingEntities();

        // GET: Owner/Rooms?homestayId=1
        public ActionResult Index(int? homestayId)
        {
            if (homestayId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            // Kiểm tra xem homestay có thuộc về owner này không
            var homestay = db.Homestay.FirstOrDefault(h => h.HomestayID == homestayId && h.HostID == ownerId);
            if (homestay == null)
                return HttpNotFound();
            
            var rooms = db.Rooms.Include(r => r.RoomImages).Where(r => r.HomestayID == homestayId).ToList();
            ViewBag.HomestayID = homestayId;
            ViewBag.HomestayName = homestay.Name;
            return View(rooms);
        }

        // GET: Owner/Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var room = db.Rooms.Include(r => r.RoomImages)
                              .Include(r => r.Homestay)
                              .FirstOrDefault(r => r.RoomID == id && r.Homestay.HostID == ownerId);
            if (room == null)
                return HttpNotFound();
            return View(room);
        }

        // GET: Owner/Rooms/Create?homestayId=1
        public ActionResult Create(int? homestayId)
        {
            if (homestayId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            // Kiểm tra xem homestay có thuộc về owner này không
            var homestay = db.Homestay.FirstOrDefault(h => h.HomestayID == homestayId && h.HostID == ownerId);
            if (homestay == null)
                return HttpNotFound();
            
            ViewBag.HomestayID = homestayId;
            ViewBag.HomestayName = homestay.Name;
            var room = new Rooms { HomestayID = homestayId.Value, Available = true };
            return View(room);
        }

        // POST: Owner/Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomID,HomestayID,Name,Type,PricePerNight,Available")] Rooms room)
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            // Kiểm tra xem homestay có thuộc về owner này không
            var homestay = db.Homestay.FirstOrDefault(h => h.HomestayID == room.HomestayID && h.HostID == ownerId);
            if (homestay == null)
                return HttpNotFound();

            // Tự động tạo tên phòng dựa trên loại
            if (!string.IsNullOrEmpty(room.Type))
            {
                int typeValue;
                if (int.TryParse(room.Type, out typeValue))
                {
                    switch (typeValue)
                    {
                        case 1:
                            room.Name = "Phòng đơn";
                            break;
                        case 2:
                            room.Name = "Phòng đôi";
                            break;
                        case 3:
                        case 4:
                        case 5:
                            room.Name = "Family Room " + typeValue + " người";
                            break;
                        default:
                            room.Name = "Phòng loại " + typeValue;
                            break;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra giá trị PricePerNight
                if (room.PricePerNight.HasValue && (room.PricePerNight < 0 || room.PricePerNight > 999999999))
                {
                    ModelState.AddModelError("PricePerNight", "Giá không hợp lệ hoặc quá lớn.");
                    ViewBag.HomestayID = room.HomestayID;
                    ViewBag.HomestayName = homestay.Name;
                    return View(room);
                }
                db.Rooms.Add(room);
                db.SaveChanges();

                // Xử lý upload nhiều ảnh
                var files = Request.Files;
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null && file.ContentLength > 0 && file.FileName != "")
                        {
                            var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                            if (!allowedExts.Contains(ext))
                            {
                                ModelState.AddModelError("", "Chỉ cho phép upload ảnh JPG, PNG, GIF, WEBP.");
                                ViewBag.HomestayID = room.HomestayID;
                                ViewBag.HomestayName = homestay.Name;
                                return View(room);
                            }
                            var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                            var uniqueName = fileName + "_" + DateTime.Now.Ticks + "_" + i + ext;
                            var path = "/Content/Uploads/" + uniqueName;
                            var serverPath = Server.MapPath(path);
                            var dir = System.IO.Path.GetDirectoryName(serverPath);
                            if (!System.IO.Directory.Exists(dir))
                                System.IO.Directory.CreateDirectory(dir);
                            file.SaveAs(serverPath);
                            db.RoomImages.Add(new RoomImages
                            {
                                RoomID = room.RoomID,
                                ImageURL = path
                            });
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { homestayId = room.HomestayID });
            }
            ViewBag.HomestayID = room.HomestayID;
            ViewBag.HomestayName = homestay.Name;
            return View(room);
        }

        // GET: Owner/Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var room = db.Rooms.Include(r => r.RoomImages)
                              .Include(r => r.Homestay)
                              .FirstOrDefault(r => r.RoomID == id && r.Homestay.HostID == ownerId);
            if (room == null)
                return HttpNotFound();
            
            ViewBag.HomestayID = room.HomestayID;
            ViewBag.HomestayName = room.Homestay.Name;
            return View(room);
        }

        // POST: Owner/Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomID,HomestayID,Name,Type,PricePerNight,Available")] Rooms room)
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            // Kiểm tra xem homestay có thuộc về owner này không
            var homestay = db.Homestay.FirstOrDefault(h => h.HomestayID == room.HomestayID && h.HostID == ownerId);
            if (homestay == null)
                return HttpNotFound();

            // Tự động tạo tên phòng dựa trên loại
            if (!string.IsNullOrEmpty(room.Type))
            {
                int typeValue;
                if (int.TryParse(room.Type, out typeValue))
                {
                    switch (typeValue)
                    {
                        case 1:
                            room.Name = "Phòng đơn";
                            break;
                        case 2:
                            room.Name = "Phòng đôi";
                            break;
                        case 3:
                        case 4:
                        case 5:
                            room.Name = "Family Room " + typeValue + " người";
                            break;
                        default:
                            room.Name = "Phòng loại " + typeValue;
                            break;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();

                // Xử lý upload thêm ảnh
                var files = Request.Files;
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null && file.ContentLength > 0 && file.FileName != "")
                        {
                            var ext = System.IO.Path.GetExtension(file.FileName).ToLower();
                            if (!allowedExts.Contains(ext))
                            {
                                ModelState.AddModelError("", "Chỉ cho phép upload ảnh JPG, PNG, GIF, WEBP.");
                                ViewBag.HomestayID = room.HomestayID;
                                ViewBag.HomestayName = homestay.Name;
                                return View(room);
                            }
                            var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                            var uniqueName = fileName + "_" + DateTime.Now.Ticks + "_" + i + ext;
                            var path = "/Content/Uploads/" + uniqueName;
                            var serverPath = Server.MapPath(path);
                            var dir = System.IO.Path.GetDirectoryName(serverPath);
                            if (!System.IO.Directory.Exists(dir))
                                System.IO.Directory.CreateDirectory(dir);
                            file.SaveAs(serverPath);
                            db.RoomImages.Add(new RoomImages
                            {
                                RoomID = room.RoomID,
                                ImageURL = path
                            });
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { homestayId = room.HomestayID });
            }
            ViewBag.HomestayID = room.HomestayID;
            ViewBag.HomestayName = homestay.Name;
            return View(room);
        }

        // GET: Owner/Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var room = db.Rooms.Include(r => r.Homestay)
                              .FirstOrDefault(r => r.RoomID == id && r.Homestay.HostID == ownerId);
            if (room == null)
                return HttpNotFound();
            return View(room);
        }

        // POST: Owner/Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Accounts", new { area = "" });
            
            int ownerId = (int)Session["UserID"];
            var room = db.Rooms.Include(r => r.Homestay)
                              .FirstOrDefault(r => r.RoomID == id && r.Homestay.HostID == ownerId);
            if (room == null)
                return HttpNotFound();
            
            int homestayId = room.HomestayID;
            
            // Xóa tất cả ảnh của phòng
            var images = db.RoomImages.Where(img => img.RoomID == id).ToList();
            foreach (var img in images)
            {
                var serverPath = Server.MapPath(img.ImageURL);
                if (System.IO.File.Exists(serverPath))
                    System.IO.File.Delete(serverPath);
                db.RoomImages.Remove(img);
            }
            
            db.Rooms.Remove(room);
            db.SaveChanges();
            return RedirectToAction("Index", new { homestayId = homestayId });
        }

        // POST: Owner/Rooms/DeleteImage
        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            // Kiểm tra session
            if (Session["UserID"] == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });
            
            int ownerId = (int)Session["UserID"];
            var image = db.RoomImages.Include(img => img.Rooms.Homestay)
                                    .FirstOrDefault(img => img.ImageID == id && img.Rooms.Homestay.HostID == ownerId);
            if (image == null)
                return Json(new { success = false, message = "Không tìm thấy ảnh" });
            
            var serverPath = Server.MapPath(image.ImageURL);
            if (System.IO.File.Exists(serverPath))
                System.IO.File.Delete(serverPath);
            
            db.RoomImages.Remove(image);
            db.SaveChanges();
            return Json(new { success = true });
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