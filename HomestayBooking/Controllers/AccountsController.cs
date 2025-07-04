using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
            var accounts = db.Account.ToList();

            return View(accounts);
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
        public ActionResult Login(string username, string password)
        {
            try
            {
                // Kiểm tra thông tin đăng nhập
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin đăng nhập!");
                    return View();
                }

                // Kiểm tra kết nối database
                if (db == null)
                {
                    ModelState.AddModelError("", "Lỗi kết nối database!");
                    return View();
                }

                var account = db.Account.FirstOrDefault(a => a.UserName == username && a.Password == password);

                if (account != null)
                {
                    // Tạo authentication ticket
                    FormsAuthentication.SetAuthCookie(account.UserName, false);

                    // Lưu thông tin user vào session
                    Session["UserID"] = account.AccountID;
                    Session["Username"] = account.UserName;
                    Session["UserRole"] = account.Role;
                    
                    // Chuyển hướng trực tiếp theo role thay vì qua RedirectToRolePage
                    var role = account.Role?.Trim().ToLower();
                    
                    switch (role)
                    {
                        case "admin":
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        case "owner":
                        case "host":
                            return RedirectToAction("Index", "Home", new { area = "Owner" });
                        case "customer":
                            return RedirectToAction("Index", "Home");
                        default:
                            ModelState.AddModelError("", $"Role không hợp lệ: '{account.Role}'. Vui lòng kiểm tra lại!");
                            return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
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
                    FormsAuthentication.SignOut();
                    Session.Clear();
                    return RedirectToAction("Login");
            }
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Accounts");
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
                try
                {
                    db.Account.Add(account);
                    db.SaveChanges();

                    // Đăng nhập luôn cho user vừa tạo
                    FormsAuthentication.SetAuthCookie(account.UserName, false);
                    Session["UserID"] = account.AccountID;
                    Session["Username"] = account.UserName;
                    Session["UserRole"] = account.Role;

                    // Chuyển về trang Home
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException?.InnerException;
                    Console.WriteLine(innerException?.Message);
                    // hoặc log ra file/log system
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng thử lại sau.");
                }
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
                try
                {
                    db.Entry(account).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException?.InnerException;
                    Console.WriteLine(innerException?.Message);
                    // hoặc log ra file/log system
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng thử lại sau.");
                }
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
            try
            {
                db.Account.Remove(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.InnerException;
                Console.WriteLine(innerException?.Message);
                // hoặc log ra file/log system
                ModelState.AddModelError("", "Có lỗi xảy ra khi xóa dữ liệu. Vui lòng thử lại sau.");
            }
            return View(account);
        }

        // Test action để kiểm tra và tạo tài khoản
        public ActionResult TestSetup()
        {
            try
            {
                // Test kết nối database
                var accountCount = db.Account.Count();
                ViewBag.DbStatus = $"Database OK - Có {accountCount} tài khoản";
                
                // Kiểm tra xem có tài khoản host nào không
                var hostAccount = db.Account.FirstOrDefault(a => a.Role == "host");
                
                if (hostAccount == null)
                {
                    // Tạo tài khoản host mẫu
                    var newHost = new Account
                    {
                        UserName = "host",
                        Password = "123456",
                        Role = "host"
                    };
                    
                    db.Account.Add(newHost);
                    db.SaveChanges();
                    
                    ViewBag.Message = "Đã tạo tài khoản host: username=host, password=123456";
                }
                else
                {
                    ViewBag.Message = $"Tài khoản host đã tồn tại: {hostAccount.UserName}";
                }
                
                // Hiển thị tất cả tài khoản
                var accounts = db.Account.ToList();
                ViewBag.Accounts = accounts;
                

                
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Database Error: {ex.Message}";
                ViewBag.DbStatus = "Database FAILED";
                return View();
            }
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
