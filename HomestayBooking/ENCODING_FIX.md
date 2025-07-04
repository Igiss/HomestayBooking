# Hướng dẫn Fix Encoding Tiếng Việt

## Vấn đề
Font tiếng Việt hiển thị không đúng (có dấu hỏi, ô vuông, hoặc ký tự lạ).

## Nguyên nhân
- File không được lưu với encoding UTF-8
- Web.config chưa cấu hình encoding đúng
- Browser không nhận diện được encoding

## Cách khắc phục

### 1. Cấu hình Web.config
Đã thêm các cấu hình sau vào `Web.config`:

```xml
<system.web>
  <globalization requestEncoding="utf-8" responseEncoding="utf-8" culture="vi-VN" uiCulture="vi-VN" />
</system.web>

<system.webServer>
  <httpProtocol>
    <customHeaders>
      <add name="Content-Type" value="text/html; charset=utf-8" />
    </customHeaders>
  </httpProtocol>
</system.webServer>
```

### 2. Cấu hình Global.asax.cs
Đã thêm method `Application_BeginRequest()` để đảm bảo encoding UTF-8:

```csharp
protected void Application_BeginRequest()
{
    Response.ContentEncoding = System.Text.Encoding.UTF8;
    Response.HeaderEncoding = System.Text.Encoding.UTF8;
}
```

### 3. Cấu hình CSS
Đã thêm font Inter từ Google Fonts vào `Site.css`:

```css
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

* {
    font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
    font-feature-settings: 'liga' 1, 'kern' 1;
    text-rendering: optimizeLegibility;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
}
```

### 4. Lưu file với UTF-8 encoding

#### Trong Visual Studio:
1. Mở file `.cshtml`
2. File → Save As
3. Click vào mũi tên bên cạnh nút Save
4. Chọn "Save with Encoding"
5. Chọn "UTF-8 with BOM" hoặc "UTF-8"
6. Click Save

#### Trong VS Code:
1. Mở file `.cshtml`
2. Ctrl+Shift+P (hoặc Cmd+Shift+P trên Mac)
3. Gõ "Change File Encoding"
4. Chọn "Save with Encoding"
5. Chọn "UTF-8"

### 5. Test encoding
Truy cập: `/Home/TestEncoding` để kiểm tra encoding tiếng Việt.

## Các file cần kiểm tra encoding:
- `Views/Shared/_Layout.cshtml`
- `Views/Accounts/Login.cshtml`
- `Views/Home/Index.cshtml`
- `Views/Home/Unauthorized.cshtml`
- `Areas/Admin/Views/Home/Index.cshtml`
- `Areas/Admin/Views/Accounts/*.cshtml`
- `Areas/Admin/Views/Homestays/*.cshtml`
- `Areas/Owner/Views/Home/Index.cshtml`

## Lưu ý:
- Luôn lưu file với encoding UTF-8
- Đảm bảo `<meta charset="utf-8" />` trong HTML
- Sử dụng font hỗ trợ tiếng Việt tốt
- Test trên nhiều browser khác nhau 