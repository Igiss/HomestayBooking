using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomestayBooking.Models;

namespace HomestayBooking.Controllers
{
    public class BookingsController : Controller
    {
        private HomestayBookingEntities db = new HomestayBookingEntities();

        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Account).Include(b => b.Rooms).Include(b => b.Payments).Include(b => b.Reviews);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bookings bookings = db.Bookings.Find(id);
            if (bookings == null)
            {
                return HttpNotFound();
            }
            return View(bookings);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Account, "AccountID", "UserName");
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "Name");
            ViewBag.BookingID = new SelectList(db.Payments, "BookingID", "PaymentMethod");
            ViewBag.BookingID = new SelectList(db.Reviews, "BookingID", "Comment");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,CustomerID,RoomID,CheckInDate,CheckOutDate,Status,CreatedAt")] Bookings bookings)
        {
            if (ModelState.IsValid)
            {
                db.Bookings.Add(bookings);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Account, "AccountID", "UserName", bookings.CustomerID);
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "Name", bookings.RoomID);
            ViewBag.BookingID = new SelectList(db.Payments, "BookingID", "PaymentMethod", bookings.BookingID);
            ViewBag.BookingID = new SelectList(db.Reviews, "BookingID", "Comment", bookings.BookingID);
            return View(bookings);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bookings bookings = db.Bookings.Find(id);
            if (bookings == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Account, "AccountID", "UserName", bookings.CustomerID);
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "Name", bookings.RoomID);
            ViewBag.BookingID = new SelectList(db.Payments, "BookingID", "PaymentMethod", bookings.BookingID);
            ViewBag.BookingID = new SelectList(db.Reviews, "BookingID", "Comment", bookings.BookingID);
            return View(bookings);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,CustomerID,RoomID,CheckInDate,CheckOutDate,Status,CreatedAt")] Bookings bookings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookings).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Account, "AccountID", "UserName", bookings.CustomerID);
            ViewBag.RoomID = new SelectList(db.Rooms, "RoomID", "Name", bookings.RoomID);
            ViewBag.BookingID = new SelectList(db.Payments, "BookingID", "PaymentMethod", bookings.BookingID);
            ViewBag.BookingID = new SelectList(db.Reviews, "BookingID", "Comment", bookings.BookingID);
            return View(bookings);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bookings bookings = db.Bookings.Find(id);
            if (bookings == null)
            {
                return HttpNotFound();
            }
            return View(bookings);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bookings bookings = db.Bookings.Find(id);
            db.Bookings.Remove(bookings);
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
