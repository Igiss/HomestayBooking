using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HomestayBooking.Models;
using HomestayBooking.Filters;

namespace HomestayBooking.Areas.Admin.Controllers
{
    [AuthenticationFilter("admin")]
    public class HomestaysController : Controller
    {
        private HomestayBookingEntities db = new HomestayBookingEntities();

        // GET: Admin/Homestays
        public ActionResult Index()
        {
            var homestays = db.Homestay.Include(h => h.Account);
            return View(homestays.ToList());
        }

        // GET: Admin/Homestays/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Homestay homestay = db.Homestay.Find(id);
            if (homestay == null)
            {
                return HttpNotFound();
            }
            return View(homestay);
        }

        // GET: Admin/Homestays/Create
        public ActionResult Create()
        {
            ViewBag.HostID = new SelectList(db.Account.Where(a => a.Role == "host"), "AccountID", "UserName");
            return View();
        }

        // POST: Admin/Homestays/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HomestayID,HostID,Name,Address,Description,Status,CreatedAt")] Homestay homestay)
        {
            if (ModelState.IsValid)
            {
                homestay.CreatedAt = DateTime.Now;
                db.Homestay.Add(homestay);
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
                            // Tạo tên file duy nhất
                            var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                            var ext = System.IO.Path.GetExtension(file.FileName);
                            var uniqueName = fileName + "_" + DateTime.Now.Ticks + ext;
                            var path = "/Content/Uploads/" + uniqueName;
                            var serverPath = Server.MapPath(path);
                            // Đảm bảo thư mục tồn tại
                            var dir = System.IO.Path.GetDirectoryName(serverPath);
                            if (!System.IO.Directory.Exists(dir))
                                System.IO.Directory.CreateDirectory(dir);
                            file.SaveAs(serverPath);

                            // Lưu vào RoomImages
                            db.RoomImages.Add(new RoomImages
                            {
                                RoomID = homestay.HomestayID, // RoomID = HomestayID
                                ImageURL = path
                            });
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            ViewBag.HostID = new SelectList(db.Account.Where(a => a.Role == "host"), "AccountID", "UserName", homestay.HostID);
            return View(homestay);
        }

        // GET: Admin/Homestays/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Homestay homestay = db.Homestay.Find(id);
            if (homestay == null)
            {
                return HttpNotFound();
            }
            ViewBag.HostID = new SelectList(db.Account.Where(a => a.Role == "host"), "AccountID", "UserName", homestay.HostID);
            return View(homestay);
        }

        // POST: Admin/Homestays/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HomestayID,HostID,Name,Address,Description,Status,CreatedAt")] Homestay homestay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(homestay).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HostID = new SelectList(db.Account.Where(a => a.Role == "host"), "AccountID", "UserName", homestay.HostID);
            return View(homestay);
        }

        // GET: Admin/Homestays/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Homestay homestay = db.Homestay.Find(id);
            if (homestay == null)
            {
                return HttpNotFound();
            }
            return View(homestay);
        }

        // POST: Admin/Homestays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Homestay homestay = db.Homestay.Find(id);
            db.Homestay.Remove(homestay);
            db.SaveChanges();
            return RedirectToAction("Index");
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