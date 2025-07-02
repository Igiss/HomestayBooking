using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HomestayBooking.Models;

namespace HomestayBooking.Controllers
{
    public class AccountsController : Controller
    {
        private HomestayBookingEntities db = new HomestayBookingEntities();

        // GET: Accounts
        public ActionResult Index()
        {
            return View(db.Account.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Login
        public ActionResult Login()
        {
            // Nếu đã đăng nhập thì chuyển hướng theo role
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("RedirectToRolePage");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra thông tin đăng nhập
                var account = db.Account.FirstOrDefault(a => a.UserName == username && a.Password == password);

                if (account != null)
                {
                    // Tạo authentication ticket
                    FormsAuthentication.SetAuthCookie(account.UserName, false);

                    // Lưu thông tin user vào session
                    Session["UserID"] = account.AccountID;
                    Session["Username"] = account.UserName;
                    Session["UserRole"] = account.Role;

                    // Chuyển hướng theo role
                    return RedirectToAction("RedirectToRolePage");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                }
            }

            return View();
        }

        // Chuyển hướng theo role
        public ActionResult RedirectToRolePage()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            string userRole = Session["UserRole"]?.ToString();

            switch (userRole?.ToLower())
            {
                case "admin":
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                case "owner":
                    return RedirectToAction("Index", "Home", new { area = "Owner" });
                case "customer":
                    return RedirectToAction("Index", "Home");
                default:
                    // Nếu không có role hoặc role không hợp lệ
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    return RedirectToAction("Login");
            }
        }

        // Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Register
        public ActionResult Create()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountID,UserName,Password,Role")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Account.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountID,UserName,Password,Role")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Account.Find(id);
            db.Account.Remove(account);
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
