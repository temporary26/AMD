# API Testing Helper Script
# Comprehensive tests for the URL Shortener API

Write-Host "URL Shortener API Tests" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan

$baseUrl = "http://localhost:5001"
$apiUrl = "$baseUrl/api/urls"

# Test 1: Create short URL without custom alias
Write-Host "`nTest 1: Creating short URL..." -ForegroundColor Yellow
$body1 = @{
    originalUrl = "https://www.google.com"
} | ConvertTo-Json

try {
    $response1 = Invoke-RestMethod -Uri $apiUrl -Method POST -Body $body1 -ContentType "application/json"
    Write-Host "Short URL created successfully!" -ForegroundColor Green
    Write-Host "   Original: $($response1.originalUrl)" -ForegroundColor White
    Write-Host "   Short: $($response1.shortUrl)" -ForegroundColor White
    Write-Host "   Short Code: $($response1.shortCode)" -ForegroundColor White
    
    # Test redirect
    Write-Host "`nTesting redirect..." -ForegroundColor Yellow
    $redirectUrl = "$baseUrl/$($response1.shortCode)"
    try {
        $redirectResponse = Invoke-WebRequest -Uri $redirectUrl -MaximumRedirection 0 -ErrorAction SilentlyContinue
        if ($redirectResponse.StatusCode -eq 302) {
            Write-Host "Redirect working! Status: 302" -ForegroundColor Green
            Write-Host "   Location: $($redirectResponse.Headers.Location)" -ForegroundColor White
        }
    } catch {
        Write-Host "Redirect test failed or service not running" -ForegroundColor Yellow
    }
} catch {
    Write-Host "Failed to create short URL!" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Create short URL with custom alias
Write-Host "`nTest 2: Creating short URL with custom alias..." -ForegroundColor Yellow
$customAlias = "my-google-$(Get-Random -Minimum 100 -Maximum 999)"
$body2 = @{
    originalUrl = "https://www.google.com"
    customShortCode = $customAlias
} | ConvertTo-Json

try {
    $response2 = Invoke-RestMethod -Uri $apiUrl -Method POST -Body $body2 -ContentType "application/json"
    Write-Host "Short URL with custom alias created!" -ForegroundColor Green
    Write-Host "   Original: $($response2.originalUrl)" -ForegroundColor White
    Write-Host "   Short: $($response2.shortUrl)" -ForegroundColor White
    Write-Host "   Custom Code: $($response2.shortCode)" -ForegroundColor White
} catch {
    Write-Host "Failed to create short URL with custom alias!" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Get all URLs
Write-Host "`nTest 3: Getting all URLs..." -ForegroundColor Yellow
try {
    $allUrls = Invoke-RestMethod -Uri $apiUrl -Method GET
    Write-Host "Retrieved $($allUrls.Count) URLs from database" -ForegroundColor Green
    
    if ($allUrls.Count -gt 0) {
        Write-Host "   Recent URLs:" -ForegroundColor White
        $allUrls | Select-Object -First 3 | ForEach-Object {
            Write-Host "   - $($_.shortCode) -> $($_.originalUrl)" -ForegroundColor Gray
        }
    }
} catch {
    Write-Host "Failed to retrieve URLs!" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Health check
Write-Host "`nTest 4: Health check..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET
    Write-Host "Service is healthy!" -ForegroundColor Green
    Write-Host "   Status: $($healthResponse.status)" -ForegroundColor White
} catch {
    Write-Host "Health check failed!" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nAPI Tests Complete!" -ForegroundColor Green
Write-Host "Visit http://localhost:3000 to use the web interface" -ForegroundColor Cyan
