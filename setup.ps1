# URL Shortener - Initial Setup Script
# Run this script once on a new computer to set up everything

Write-Host "URL Shortener - Initial Setup" -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan
Write-Host "This will set up everything needed to run the URL Shortener application" -ForegroundColor White
Write-Host ""

# Check dependencies
Write-Host "Step 1: Checking dependencies..." -ForegroundColor Cyan
& ".\scripts\install-dependencies.ps1"
if ($LASTEXITCODE -ne 0) { exit 1 }

# Setup backend
Write-Host "`nStep 2: Setting up backend services..." -ForegroundColor Cyan
& ".\scripts\setup-backend.ps1"
if ($LASTEXITCODE -ne 0) { exit 1 }

# Setup frontend
Write-Host "`nStep 3: Setting up frontend..." -ForegroundColor Cyan
& ".\scripts\setup-frontend.ps1"
if ($LASTEXITCODE -ne 0) { exit 1 }

# Setup database
Write-Host "`nStep 4: Setting up database..." -ForegroundColor Cyan
& ".\scripts\setup-database.ps1"
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "`nSetup Complete!" -ForegroundColor Green
Write-Host "================`n" -ForegroundColor Green

Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run './start.ps1' to start all services" -ForegroundColor White
Write-Host "2. Open your browser to http://localhost:3000" -ForegroundColor White
Write-Host "3. Run './test.ps1' to test the API" -ForegroundColor White
Write-Host ""
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
