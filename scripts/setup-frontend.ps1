# Frontend Setup Helper Script
# Installs npm dependencies for the Vue.js frontend

Write-Host "Setting up Frontend..." -ForegroundColor Cyan

# Navigate to frontend directory
Set-Location "frontend"

Write-Host "Installing npm dependencies..." -ForegroundColor Yellow
try {
    npm install
    Write-Host "Frontend dependencies installed successfully!" -ForegroundColor Green
} catch {
    Write-Host "Failed to install frontend dependencies!" -ForegroundColor Red
    Set-Location ".."
    exit 1
}

# Return to root directory
Set-Location ".."

Write-Host "Frontend setup complete!" -ForegroundColor Green
