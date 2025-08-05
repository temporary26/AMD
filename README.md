# URL Shortener

A modern, microservices-based URL shortener application built with .NET 8 and Vue.js.

## 🚀 Quick Start

### Initial Setup
```powershell
.\setup.ps1
```
This will install all dependencies and packages automatically.

### Running the Application
```powershell
.\start.ps1
```
This will build the solution and start all web services.


## 📋 Prerequisites

### Required Dependencies

1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
   - Version: 8.0 or later

2. **[Node.js](https://nodejs.org/)**
   - Version: 18.x or later

3. **[SQL Server LocalDB 2022](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver17)**
   - Required for data persistence

## 🏗️ Architecture

This application follows a microservices architecture with the following components:

### Services
- **API Gateway** (Port 5000) - Routes requests to appropriate services
- **URL Service** (Port 5001) - Core URL shortening functionality
- **Analytics Service** (Port 5002) - Usage analytics and metrics
- **User Service** (Port 5003) - User management

### Frontend
- **Vue.js Application** (Port 3000) - Modern web interface

## 📦 Dependencies

### .NET Packages
The following packages are automatically restored during setup:
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.App
- Ocelot (API Gateway)
- Microsoft.Extensions.Diagnostics.HealthChecks
- Swashbuckle.AspNetCore (Swagger)

### Frontend Packages
The following npm packages are automatically installed:
- vue@^3.4.0
- vite@^5.0.0
- @vitejs/plugin-vue
- axios

## 🔧 Manual Installation

If the automatic setup fails, you can install dependencies manually:

### Backend
```powershell
dotnet restore
```

### Frontend
```powershell
cd frontend
npm install
```

## 🌐 Ports

Ensure the following ports are available:
- **3000** - Frontend (Vue.js)
- **5000** - API Gateway
- **5001** - URL Service
- **5002** - Analytics Service
- **5003** - User Service

## 📁 Project Structure

```
├── src/
│   ├── ApiGateway/           # Ocelot API Gateway
│   ├── Services/             # Microservices
│   │   ├── UrlService/       # URL shortening logic
│   │   ├── AnalyticsService/ # Analytics and metrics
│   │   └── UserService/      # User management
│   ├── Shared/               # Common libraries
│   └── Web/                  # Web application
├── frontend/                 # Vue.js frontend
├── scripts/                  # PowerShell automation scripts
├── setup.ps1                 # Initial setup script
├── start.ps1                 # Start all services
└── test.ps1                  # API testing script
```

## 🔍 Troubleshooting

### Setup Issues
- Check that all prerequisites are installed
- Verify the required ports are not in use
- Review `Dependencies.txt` for detailed package information

### Service Issues
- Run `test.ps1` to verify all services are responding
- Check individual service logs
- Ensure SQL Server LocalDB is running

**Need help?** Check the `Dependencies.txt` file for detailed package information or run the test suite to diagnose issues.
