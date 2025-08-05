# Install Dependencies Helper Script
# Installs .NET SDK, Node.js, and SQL Server LocalDB

Write-Host "Installing Dependencies..." -ForegroundColor Cyan

# Check if .NET 8 SDK is installed
try {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion -like "8.*") {
        Write-Host ".NET 8 SDK already installed: $dotnetVersion" -ForegroundColor Green
    } else {
        throw "Wrong version"
    }
} catch {
    Write-Host ".NET 8 SDK not found. Please install it from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Red
    exit 1
}

# Check if Node.js is installed
try {
    $nodeVersion = node --version 2>$null
    Write-Host "Node.js already installed: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "Node.js not found. Please install it from: https://nodejs.org/" -ForegroundColor Red
    exit 1
}

# Check if SQL Server LocalDB is available
try {
    sqllocaldb info mssqllocaldb 2>$null | Out-Null
    Write-Host "SQL Server LocalDB is available" -ForegroundColor Green
} catch {
    Write-Host "SQL Server LocalDB not found. Please install SQL Server Express LocalDB" -ForegroundColor Red
    exit 1
}

Write-Host "All dependencies are installed!" -ForegroundColor Green
