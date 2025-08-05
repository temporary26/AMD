# Service Starter Helper Script
# Starts all backend services in separate windows

param(
    [string]$Service = "all"
)

function Start-ServiceInNewWindow {
    param(
        [string]$Title,
        [string]$Path,
        [string]$Command
    )
    
    Write-Host "Starting $Title..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$Path'; $Command; Write-Host '$Title is running on the specified port' -ForegroundColor Green"
}

Write-Host "Starting Services..." -ForegroundColor Cyan

if ($Service -eq "all" -or $Service -eq "gateway") {
    Start-ServiceInNewWindow -Title "API Gateway" -Path "src\ApiGateway\UrlShortener.Gateway" -Command "dotnet run"
}

if ($Service -eq "all" -or $Service -eq "url") {
    Start-ServiceInNewWindow -Title "URL Service" -Path "src\Services\UrlShortener.UrlService" -Command "dotnet run"
}

if ($Service -eq "all" -or $Service -eq "analytics") {
    Start-ServiceInNewWindow -Title "Analytics Service" -Path "src\Services\UrlShortener.AnalyticsService" -Command "dotnet run"
}

if ($Service -eq "all" -or $Service -eq "user") {
    Start-ServiceInNewWindow -Title "User Service" -Path "src\Services\UrlShortener.UserService" -Command "dotnet run"
}

if ($Service -eq "all" -or $Service -eq "frontend") {
    Start-ServiceInNewWindow -Title "Frontend (Vue.js)" -Path "frontend" -Command "npm run dev"
}

Write-Host "All services are starting up!" -ForegroundColor Green
Write-Host "Frontend will be available at: http://localhost:3000" -ForegroundColor Cyan
Write-Host "API Gateway will be available at: http://localhost:5000" -ForegroundColor Cyan
Write-Host "URL Service will be available at: http://localhost:5001" -ForegroundColor Cyan
