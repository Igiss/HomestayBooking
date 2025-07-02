using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace HomestayBooking.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthenticationFilter : AuthorizeAttribute
    {
        public string[] AllowedRoles { get; set; }

        public AuthenticationFilter(params string[] roles)
        {
            AllowedRoles = roles;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            // Kiểm tra xem user đã đăng nhập chưa
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            // Nếu không có role nào được chỉ định, cho phép tất cả user đã đăng nhập
            if (AllowedRoles == null || AllowedRoles.Length == 0)
                return true;

            // Lấy role của user từ session
            string userRole = httpContext.Session["UserRole"]?.ToString();

            if (string.IsNullOrEmpty(userRole))
                return false;

            // Kiểm tra xem role của user có trong danh sách được phép không
            return Array.Exists(AllowedRoles, role =>
                role.Equals(userRole, StringComparison.OrdinalIgnoreCase));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Nếu chưa đăng nhập, chuyển hướng đến trang login
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "Accounts", action = "Login" }
                    )
                );
            }
            else
            {
                // Nếu đã đăng nhập nhưng không có quyền, chuyển hướng đến trang lỗi
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "Home", action = "Unauthorized" }
                    )
                );
            }
        }
    }
}