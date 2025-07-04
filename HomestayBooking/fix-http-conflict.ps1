# Script để khắc phục lỗi System.Net.Http
Write-Host "Đang khắc phục lỗi System.Net.Http..." -ForegroundColor Green

# Xóa thư mục packages và bin để clean build
Write-Host "Xóa thư mục packages và bin..." -ForegroundColor Yellow
if (Test-Path "..\packages") {
    Remove-Item "..\packages" -Recurse -Force
}
if (Test-Path "bin") {
    Remove-Item "bin" -Recurse -Force
}
if (Test-Path "obj") {
    Remove-Item "obj" -Recurse -Force
}

# Restore NuGet packages
Write-Host "Restore NuGet packages..." -ForegroundColor Yellow
nuget restore "..\HomestayBooking.sln"

# Build lại dự án
Write-Host "Build lại dự án..." -ForegroundColor Yellow
msbuild "..\HomestayBooking.sln" /p:Configuration=Debug /p:Platform="Any CPU" /t:Clean,Build

Write-Host "Hoàn thành! Hãy thử chạy lại ứng dụng." -ForegroundColor Green 