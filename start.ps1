# URL Shortener - Start Services Script
# Starts all services and opens the application in your browser

param(
    [switch]$NoBrowser,
    [string]$Service = "all"
)

Write-Host "URL Shortener - Starting Services" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan

# Start the services
& ".\scripts\start-services.ps1" -Service $Service

# Wait a moment for services to start
Write-Host "`nWaiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

# Open browser unless disabled
if (-not $NoBrowser -and ($Service -eq "all" -or $Service -eq "frontend")) {
    Write-Host "Opening browser..." -ForegroundColor Green
    Start-Process "http://localhost:3000"
}

Write-Host "`nAll services are starting!" -ForegroundColor Green
Write-Host "===========================`n" -ForegroundColor Green

Write-Host "Services running:" -ForegroundColor Yellow
if ($Service -eq "all" -or $Service -eq "frontend") {
    Write-Host "Frontend: http://localhost:3000" -ForegroundColor White
}
if ($Service -eq "all" -or $Service -eq "gateway") {
    Write-Host "API Gateway: http://localhost:5000" -ForegroundColor White
}
if ($Service -eq "all" -or $Service -eq "url") {
    Write-Host "URL Service: http://localhost:5001" -ForegroundColor White
}
if ($Service -eq "all" -or $Service -eq "analytics") {
    Write-Host "Analytics Service: http://localhost:5002" -ForegroundColor White
}
if ($Service -eq "all" -or $Service -eq "user") {
    Write-Host "User Service: http://localhost:5003" -ForegroundColor White
}

Write-Host ""
Write-Host "To stop services, close the PowerShell windows" -ForegroundColor Cyan
Write-Host "Run './test.ps1' to test the API" -ForegroundColor Cyan
Write-Host ""
Write-Host "Usage examples:" -ForegroundColor Yellow
Write-Host "  ./start.ps1                    # Start all services and open browser" -ForegroundColor White
Write-Host "  ./start.ps1 -NoBrowser         # Start all services without opening browser" -ForegroundColor White
Write-Host "  ./start.ps1 -Service frontend  # Start only frontend" -ForegroundColor White
Write-Host "  ./start.ps1 -Service url       # Start only URL service" -ForegroundColor White
