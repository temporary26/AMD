# Install Dependencies Helper Script
# Installs .NET SDK, Node.js, and SQL Server LocalDB

Write-Host "Installing Dependencies..." -ForegroundColor Cyan

# Check if .NET SDK is installed (8.0 or higher)
try {
    $dotnetVersion = dotnet --version 2>$null
    $versionMajor = [int]($dotnetVersion.Split('.')[0])
    if ($versionMajor -ge 8) {
        Write-Host ".NET SDK already installed: $dotnetVersion (compatible with .NET 8+)" -ForegroundColor Green
    } else {
        throw "Version too old"
    }
} catch {
    Write-Host ".NET SDK not found or version is too old. Please install .NET 8 or higher from: https://dotnet.microsoft.com/download" -ForegroundColor Red
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
