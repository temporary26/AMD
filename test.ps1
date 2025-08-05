# URL Shortener - API Testing Script
# Tests the URL Shortener API endpoints

Write-Host "URL Shortener - API Testing" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Cyan
Write-Host "This will test all API endpoints to ensure everything is working" -ForegroundColor White
Write-Host ""

# Check if services are running
Write-Host "Checking if services are running..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5001/health" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "URL Service is running!" -ForegroundColor Green
} catch {
    Write-Host "URL Service is not running!" -ForegroundColor Red
    Write-Host "Run './start.ps1' first to start the services" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

# Run the API tests
Write-Host ""
& ".\scripts\test-api.ps1"

Write-Host ""
Write-Host "If tests failed, make sure all services are running with './start.ps1'" -ForegroundColor Cyan
Write-Host "Visit http://localhost:3000 to use the web interface" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
