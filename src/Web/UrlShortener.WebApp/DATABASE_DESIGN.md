# URL Shortener Database Design & Architecture

## Overview
This URL shortener application is built with ASP.NET Core 8.0, Entity Framework Core, and SQL Server. It provides a comprehensive solution for creating, managing, and tracking shortened URLs.

## Database Schema

### Tables Created

#### 1. ShortenedUrls Table
This is the main table that stores the shortened URL information.

**Columns:**
- `Id` (int, Primary Key) - Auto-incrementing unique identifier
- `OriginalUrl` (nvarchar(2048)) - The original long URL
- `ShortCode` (nvarchar(10), Unique) - The unique short code for the URL
- `CreatedAt` (datetime2) - When the URL was created
- `ExpiresAt` (datetime2, nullable) - Optional expiration date
- `ClickCount` (int, default 0) - Total number of clicks
- `IsActive` (bit, default true) - Whether the URL is active
- `UserId` (nvarchar(450), nullable) - Associated user ID (if authenticated)

**Indexes:**
- Unique index on `ShortCode` for fast lookups
- Index on `UserId` for user-specific queries
- Index on `CreatedAt` for date-based queries
- Index on `IsActive` for filtering active URLs

#### 2. UrlClicks Table
This table stores detailed click analytics for each shortened URL.

**Columns:**
- `Id` (int, Primary Key) - Auto-incrementing unique identifier
- `ShortenedUrlId` (int, Foreign Key) - References ShortenedUrls.Id
- `ClickedAt` (datetime2) - When the click occurred
- `IpAddress` (nvarchar(45)) - IP address of the visitor
- `UserAgent` (nvarchar(500)) - Browser/device information
- `Referer` (nvarchar(500)) - Referring website
- `Country` (nvarchar(100)) - Geographic location (country)
- `City` (nvarchar(100)) - Geographic location (city)

**Indexes:**
- Index on `ShortenedUrlId` for foreign key relationships
- Index on `ClickedAt` for time-based analytics
- Composite index on `(ShortenedUrlId, ClickedAt)` for efficient analytics queries

**Relationships:**
- One-to-Many relationship: One ShortenedUrl can have many UrlClicks
- Cascade delete: When a ShortenedUrl is deleted, all associated UrlClicks are deleted

## Features Implemented

### Core Functionality
1. **URL Shortening**: Generate unique short codes for long URLs
2. **Custom Codes**: Allow users to specify custom short codes
3. **URL Redirection**: Redirect short URLs to original URLs
4. **Expiration Support**: Optional expiration dates for URLs
5. **User Association**: Link URLs to authenticated users

### Analytics & Tracking
1. **Click Counting**: Track total clicks per URL
2. **Detailed Analytics**: Record IP, user agent, referrer, and location
3. **Statistics Dashboard**: View comprehensive statistics for each URL
4. **Recent Clicks**: Display recent activity with details

### Web Interface
1. **Modern UI**: Bootstrap-based responsive design with Font Awesome icons
2. **URL Creation Form**: Easy-to-use form for creating short URLs
3. **User Dashboard**: View and manage user's created URLs
4. **Statistics View**: Detailed analytics for each shortened URL
5. **Copy to Clipboard**: One-click copying of shortened URLs

### API Endpoints
1. **RESTful API**: Complete API for programmatic access
2. **Swagger Documentation**: Auto-generated API documentation
3. **Authentication Support**: User-specific operations for authenticated users

## API Endpoints

### URL Shortener API (`/api/urlshortener`)

1. **POST /api/urlshortener/shorten**
   - Create a new shortened URL
   - Supports custom codes and expiration dates
   - Returns shortened URL details

2. **GET /api/urlshortener/{shortCode}/stats**
   - Get detailed statistics for a short URL
   - Returns click analytics and recent activity

3. **GET /api/urlshortener/my-urls** (Authenticated)
   - Get all URLs created by the authenticated user
   - Returns list of user's shortened URLs

4. **DELETE /api/urlshortener/{shortCode}** (Authenticated)
   - Deactivate a shortened URL
   - Only the creator can deactivate their URLs

5. **GET /api/urlshortener/health**
   - Health check endpoint
   - Returns API status

### Redirect Controller
1. **GET /{shortCode}**
   - Redirect to original URL
   - Records click analytics
   - Returns 404 for invalid or expired URLs

## Data Models

### Request/Response DTOs
- `CreateShortenedUrlRequest`: For creating new short URLs
- `ShortenedUrlResponse`: Standard response for URL operations
- `UrlStatisticsResponse`: Detailed analytics response
- `ClickStatistic`: Individual click information

## Security Features

1. **Input Validation**: URL format validation and length limits
2. **User Isolation**: Users can only manage their own URLs
3. **Rate Limiting**: Could be implemented at the controller level
4. **SQL Injection Protection**: Entity Framework provides protection
5. **XSS Protection**: Razor views automatically encode output

## Performance Optimizations

1. **Database Indexes**: Strategic indexing for fast queries
2. **Async Operations**: All database operations are asynchronous
3. **Efficient Queries**: Optimized Entity Framework queries
4. **Background Processing**: Click recording happens asynchronously

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Font Awesome icons
- **API Documentation**: Swagger/OpenAPI
- **Logging**: Built-in ASP.NET Core logging

## Migration Files Created

1. `CreateIdentitySchema` - Sets up ASP.NET Core Identity tables
2. `CreateUrlShortenerTables` - Creates ShortenedUrls and UrlClicks tables

## Next Steps for Production

1. **Add Ocelot Gateway**: Implement API gateway as requested
2. **Add Rate Limiting**: Prevent abuse of the service
3. **Add Caching**: Redis cache for frequently accessed URLs
4. **Add Geographic IP Resolution**: Enhance location tracking
5. **Add URL Preview**: Show preview of destination URLs
6. **Add Bulk Operations**: API for bulk URL creation
7. **Add Export Features**: Export analytics data
8. **Add Domain Customization**: Custom domains for short URLs
9. **Add Click Analytics Charts**: Visual analytics dashboard
10. **Add URL Health Checking**: Monitor if original URLs are still valid

## Database Connection String

The application uses SQL Server LocalDB by default:
```
Server=(localdb)\\mssqllocaldb;Database=aspnet-WebApplication1-7802f894-2bc5-4db2-9104-23ed0cc5dda4;Trusted_Connection=True;MultipleActiveResultSets=true
```

This can be updated in `appsettings.json` to point to any SQL Server instance for production deployment.
