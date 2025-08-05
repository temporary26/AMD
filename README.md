# URL Shortener Microservices

A modern, scalable URL shortening service built with .NET 9 using microservices architecture.

## Architecture Overview

```
UrlShortener.Solution/
├── src/
│   ├── ApiGateway/
│   │   └── UrlShortener.Gateway/          # API Gateway (Ocelot)
│   ├── Services/
│   │   ├── UrlShortener.UrlService/       # URL shortening and retrieval
│   │   ├── UrlShortener.AnalyticsService/ # Click tracking and analytics
│   │   └── UrlShortener.UserService/      # User management and authentication
│   ├── Shared/
│   │   ├── UrlShortener.Contracts/        # Shared DTOs and contracts
│   │   └── UrlShortener.Common/           # Common utilities and services
│   └── Web/
│       └── UrlShortener.WebApp/           # Web frontend application
├── tests/                                 # Unit and integration tests
├── docs/                                  # Documentation
└── docker-compose.yml                     # Docker orchestration
```

## Services

### 1. API Gateway (Port 5000)
- **Technology**: Ocelot
- **Purpose**: Single entry point for all client requests
- **Features**: Routing, load balancing, rate limiting

### 2. URL Service (Port 5001)
- **Purpose**: Core URL shortening functionality
- **Features**: 
  - Create shortened URLs
  - Retrieve original URLs
  - Custom short codes
  - URL validation

### 3. Analytics Service (Port 5002)
- **Purpose**: Track and analyze URL usage
- **Features**:
  - Click tracking
  - Usage statistics
  - Geographic analytics
  - Reporting

### 4. User Service (Port 5003)
- **Purpose**: User management and authentication
- **Features**:
  - User registration/login
  - JWT token management
  - User profile management
  - Role-based access control

### 5. Web Application (Port 5004)
- **Purpose**: Frontend web interface
- **Features**:
  - User-friendly URL shortening interface
  - Dashboard with analytics
  - User account management

## Shared Libraries

### UrlShortener.Contracts
- DTOs and data contracts shared between services
- API models and request/response objects

### UrlShortener.Common
- Shared utilities and helper classes
- Caching service abstractions
- Common middleware and extensions

## Getting Started

### Prerequisites
- .NET 9 SDK
- Docker Desktop
- SQL Server (or use Docker)
- Redis (or use Docker)

### Running with Docker Compose

1. **Clone and navigate to the project:**
   ```bash
   cd "c:\Users\bombardino crocodilo\Downloads\AMD"
   ```

2. **Build and run all services:**
   ```bash
   docker-compose up --build
   ```

3. **Access the applications:**
   - API Gateway: http://localhost:5000
   - URL Service: http://localhost:5001
   - Analytics Service: http://localhost:5002
   - User Service: http://localhost:5003
   - Web Application: http://localhost:5004

### Running Locally

1. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

2. **Update connection strings in appsettings.json for each service**

3. **Run database migrations:**
   ```bash
   # Navigate to each service and run migrations
   cd src/Services/UrlShortener.UrlService
   dotnet ef database update
   
   cd ../UrlShortener.AnalyticsService
   dotnet ef database update
   
   cd ../UrlShortener.UserService
   dotnet ef database update
   ```

4. **Start services individually:**
   ```bash
   # Terminal 1 - URL Service
   cd src/Services/UrlShortener.UrlService
   dotnet run
   
   # Terminal 2 - Analytics Service
   cd src/Services/UrlShortener.AnalyticsService
   dotnet run
   
   # Terminal 3 - User Service
   cd src/Services/UrlShortener.UserService
   dotnet run
   
   # Terminal 4 - API Gateway
   cd src/ApiGateway/UrlShortener.Gateway
   dotnet run
   
   # Terminal 5 - Web App
   cd src/Web/UrlShortener.WebApp
   dotnet run
   ```

## API Endpoints

### Through API Gateway (Port 5000)

#### URL Operations
- `POST /api/urls/shorten` - Create shortened URL
- `GET /api/urls/{shortCode}` - Get original URL
- `GET /api/urls/user/{userId}` - Get user's URLs

#### Analytics
- `POST /api/analytics/track` - Track URL click
- `GET /api/analytics/stats/{shortCode}` - Get URL statistics

#### User Management
- `POST /api/users/register` - Register new user
- `POST /api/users/login` - User login
- `GET /api/users/profile` - Get user profile

## Configuration

### Environment Variables
- `ConnectionStrings__DefaultConnection` - Database connection string
- `ConnectionStrings__Redis` - Redis connection string
- `JwtSettings__Key` - JWT signing key
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience

### Database
Each service has its own database:
- `UrlShortenerDB` - URL Service
- `UrlShortenerAnalyticsDB` - Analytics Service
- `UrlShortenerUserDB` - User Service
- `UrlShortenerWebDB` - Web Application

## Development

### Adding New Features
1. Define contracts in `UrlShortener.Contracts`
2. Implement business logic in appropriate service
3. Update API Gateway routing if needed
4. Add corresponding UI in Web Application

### Testing
```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test tests/UrlShortener.UrlService.Tests
```

## Deployment

### Production Considerations
- Configure proper connection strings
- Set up SSL certificates
- Configure Redis cluster for caching
- Set up monitoring and logging
- Configure load balancers for high availability

### Health Checks
Each service exposes a `/health` endpoint for monitoring.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

This project is licensed under the MIT License.
