# Database Setup Helper Script
# Creates database and runs migrations

Write-Host "Setting up Database..." -ForegroundColor Cyan

# Start SQL Server LocalDB
Write-Host "Starting SQL Server LocalDB..." -ForegroundColor Yellow
sqllocaldb start mssqllocaldb

# Navigate to URL Service and run migrations
Set-Location "src\Services\UrlShortener.UrlService"

Write-Host "Running database migrations..." -ForegroundColor Yellow
try {
    dotnet ef database update
    Write-Host "Database migrations completed successfully!" -ForegroundColor Green
} catch {
    Write-Host "Database migration failed!" -ForegroundColor Red
    exit 1
}

# Return to root directory
Set-Location "..\..\..\"

Write-Host "Database setup complete!" -ForegroundColor Green
