# Script to remove System.Net.Http.dll conflict from roslyn folder
# Run this script after each build to prevent System.Net.Http version conflicts

$roslynPath = "bin\roslyn\System.Net.Http.dll"

if (Test-Path $roslynPath) {
    Write-Host "Removing System.Net.Http.dll from roslyn folder to prevent version conflicts..."
    Remove-Item $roslynPath -Force
    Write-Host "File removed successfully!"
} else {
    Write-Host "System.Net.Http.dll not found in roslyn folder."
}

Write-Host "Done!" 