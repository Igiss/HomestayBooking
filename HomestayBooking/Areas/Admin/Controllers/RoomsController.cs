using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomestayBooking.Models;
using HomestayBooking.Filters;

namespace HomestayBooking.Areas.Admin.Controllers
{
    [AuthenticationFilter("admin")]
    public class RoomsController : Controller
    {
        private HomestayBookingEntities db = new HomestayBookingEntities();

        // GET: Admin/Rooms?homestayId=1
        public ActionResult Index(int? homestayId)
        {
            if (homestayId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var rooms = db.Rooms.Include(r => r.RoomImages).Where(r => r.HomestayID == homestayId).ToList();
            ViewBag.HomestayID = homestayId;
            return View(rooms);
        }

        // GET: Admin/Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var room = db.Rooms.Include(r => r.RoomImages).FirstOrDefault(r => r.RoomID == id);
            if (room == null)
                return HttpNotFound();
            return View(room);
        }

        // GET: Admin/Rooms/Create?homestayId=1
        public ActionResult Create(int? homestayId)
        {
            if (homestayId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.HomestayID = homestayId;
            var room = new Rooms { HomestayID = homestayId.Value, Available = true };
            return View(room);
        }

        // POST: Admin/Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomID,HomestayID,Name,Type,PricePerNight,Available")] Rooms room)
        {
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
                db.Rooms.Add(room);
                db.SaveChanges();

                // Xử lý upload nhiều ảnh
                var files = Request.Files;
                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null && file.ContentLength > 0 && file.FileName != "")
                        {
                            var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                            var ext = System.IO.Path.GetExtension(file.FileName);
                            var uniqueName = fileName + "_" + DateTime.Now.Ticks + ext;
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
            return View(room);
        }

        // GET: Admin/Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var room = db.Rooms.Include(r => r.RoomImages).FirstOrDefault(r => r.RoomID == id);
            if (room == null)
                return HttpNotFound();
            ViewBag.HomestayID = room.HomestayID;
            return View(room);
        }

        // POST: Admin/Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomID,HomestayID,Name,Type,PricePerNight,Available")] Rooms room)
        {
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
                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null && file.ContentLength > 0 && file.FileName != "")
                        {
                            var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                            var ext = System.IO.Path.GetExtension(file.FileName);
                            var uniqueName = fileName + "_" + DateTime.Now.Ticks + ext;
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
            return View(room);
        }

        // GET: Admin/Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var room = db.Rooms.Find(id);
            if (room == null)
                return HttpNotFound();
            return View(room);
        }

        // POST: Admin/Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var room = db.Rooms.Find(id);
            int homestayId = room.HomestayID;
            // Xóa ảnh liên quan
            var images = db.RoomImages.Where(img => img.RoomID == id).ToList();
            foreach (var img in images)
            {
                db.RoomImages.Remove(img);
            }
            db.Rooms.Remove(room);
            db.SaveChanges();
            return RedirectToAction("Index", new { homestayId = homestayId });
        }

        // Xóa ảnh riêng lẻ (AJAX hoặc link)
        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            var img = db.RoomImages.Find(id);
            if (img != null)
            {
                db.RoomImages.Remove(img);
                db.SaveChanges();
                // Có thể xóa file vật lý nếu muốn
            }
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