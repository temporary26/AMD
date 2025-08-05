# URL Shortener Microservices - Development Setup

# Restore all packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Green
dotnet restore

# Build the entire solution
Write-Host "Building solution..." -ForegroundColor Green
dotnet build

Write-Host "Setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "To run the services:" -ForegroundColor Yellow
Write-Host "1. Using Docker: docker-compose up --build" -ForegroundColor White
Write-Host "2. Or run each service individually with: dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "Service URLs:" -ForegroundColor Yellow
Write-Host "- API Gateway: http://localhost:5000" -ForegroundColor White
Write-Host "- URL Service: http://localhost:5001" -ForegroundColor White
Write-Host "- Analytics Service: http://localhost:5002" -ForegroundColor White
Write-Host "- User Service: http://localhost:5003" -ForegroundColor White
Write-Host "- Web Application: http://localhost:5004" -ForegroundColor White
