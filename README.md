# Homestay Booking System

Hệ thống đặt phòng homestay với chức năng phân quyền theo role.

## Tính năng chính

### 🔐 Hệ thống Authentication & Authorization

- **Đăng nhập/Đăng xuất** với Forms Authentication
- **Phân quyền theo Role**: Admin, Owner, Customer
- **Chuyển hướng tự động** theo role sau khi đăng nhập
- **Bảo vệ trang** với AuthenticationFilter

### 👥 Các Role và Quyền

#### 1. **Admin** (Quản trị viên)

- Truy cập: `/Admin/Home/Index`
- Quản lý toàn bộ hệ thống
- Xem thống kê tổng quan
- Quản lý người dùng

#### 2. **Owner** (Chủ homestay)

- Truy cập: `/Owner/Home/Index`
- Quản lý homestay của mình
- Xem đặt phòng và thu nhập
- Quản lý phòng và giá cả

#### 3. **Customer** (Khách hàng)

- Truy cập: `/Home/Index` (trang chủ)
- Tìm kiếm và đặt phòng
- Xem lịch sử đặt phòng

## Cấu trúc dự án

```
HomestayBooking/
├── Controllers/
│   ├── LoginController.cs          # Xử lý đăng nhập/đăng xuất
│   ├── HomeController.cs           # Trang chủ chính
│   └── AccountsController.cs       # Quản lý tài khoản
├── Areas/
│   ├── Admin/                      # Khu vực Admin
│   │   ├── Controllers/
│   │   ├── Views/
│   │   └── AdminAreaRegistration.cs
│   └── Owner/                      # Khu vực Owner
│       ├── Controllers/
│       ├── Views/
│       └── OwnerAreaRegistration.cs
├── Filters/
│   └── AuthenticationFilter.cs     # Filter kiểm tra quyền truy cập
├── Models/
│   └── Account.cs                  # Model tài khoản
└── Views/
    ├── Login/
    │   └── Index.cshtml            # Trang đăng nhập
    ├── Home/
    │   ├── Index.cshtml            # Trang chủ
    │   └── Unauthorized.cshtml     # Trang lỗi 403
    └── Shared/
        └── _Layout.cshtml          # Layout chính
```

## Cách sử dụng

### 1. Truy cập trang chủ

- Khi build và chạy project, mặc định sẽ hiển thị trang chủ tại `/`
- Nếu đã đăng nhập, hệ thống sẽ tự động chuyển hướng theo role:
  - **Admin** → `/Admin/Home/Index`
  - **Owner** → `/Owner/Home/Index`
  - **Customer** → Ở lại trang chủ với thông tin user

### 2. Đăng nhập

- Truy cập: `/Login` hoặc click "Đăng nhập" trên trang chủ
- Nhập username và password
- Hệ thống sẽ tự động chuyển hướng theo role

### 2. Thêm tài khoản test

Thêm dữ liệu vào bảng `Account`:

```sql
-- Admin
INSERT INTO Account (UserName, Password, Role) VALUES ('admin', 'admin123', 'admin');

-- Owner
INSERT INTO Account (UserName, Password, Role) VALUES ('owner', 'owner123', 'owner');

-- Customer
INSERT INTO Account (UserName, Password, Role) VALUES ('customer', 'customer123', 'customer');
```

### 3. Sử dụng AuthenticationFilter

```csharp
// Chỉ cho phép Admin
[AuthenticationFilter("admin")]
public class AdminController : Controller
{
    // ...
}

// Cho phép Admin và Owner
[AuthenticationFilter("admin", "owner")]
public class OwnerController : Controller
{
    // ...
}
```

## Cấu hình

### Web.config

```xml
<authentication mode="Forms">
  <forms loginUrl="~/Login" timeout="2880" />
</authentication>
```

### RouteConfig.cs

```csharp
// Đăng ký các Area
routes.MapRoute(
    name: "Admin",
    url: "Admin/{controller}/{action}/{id}",
    defaults: new { area = "Admin", controller = "Home", action = "Index", id = UrlParameter.Optional }
);
```

## Giao diện

### Trang đăng nhập

- Giao diện đẹp với Bootstrap
- Form validation
- Toggle password visibility
- Responsive design

### Navigation

- Hiển thị thông tin user đã đăng nhập
- Menu dropdown theo role
- Nút đăng xuất

### Dashboard

- **Admin**: Thống kê tổng quan, quản lý hệ thống
- **Owner**: Quản lý homestay, đặt phòng, thu nhập
- **Customer**: Tìm kiếm và đặt phòng

## Bảo mật

- Sử dụng Forms Authentication
- Session management
- Anti-forgery token
- Role-based authorization
- Secure password handling (cần cải thiện)

## Cải thiện có thể thực hiện

1. **Mã hóa mật khẩu** với bcrypt hoặc SHA256
2. **JWT Token** thay vì Forms Authentication
3. **Remember Me** functionality
4. **Password Reset** functionality
5. **Email verification**
6. **Two-factor authentication**
7. **Audit logging** cho các hoạt động quan trọng

## Chạy dự án

1. Mở solution trong Visual Studio
2. Restore NuGet packages
3. Cập nhật connection string trong Web.config
4. Build và Run project
5. Truy cập: `http://localhost:port/` (trang chủ mặc định)

## Tài khoản test

- **Admin**: admin / admin123
- **Owner**: owner / owner123
- **Customer**: customer / customer123
