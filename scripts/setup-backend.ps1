# Backend Services Helper Script
# Builds and restores .NET projects

Write-Host "Setting up Backend Services..." -ForegroundColor Cyan

Write-Host "Restoring .NET packages..." -ForegroundColor Yellow
try {
    dotnet restore
    Write-Host ".NET packages restored successfully!" -ForegroundColor Green
} catch {
    Write-Host "Failed to restore .NET packages!" -ForegroundColor Red
    exit 1
}

Write-Host "Building solution..." -ForegroundColor Yellow
try {
    dotnet build --no-restore
    Write-Host "Solution built successfully!" -ForegroundColor Green
} catch {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Backend setup complete!" -ForegroundColor Green
