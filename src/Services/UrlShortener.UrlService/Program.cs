using Microsoft.EntityFrameworkCore;
using UrlShortener.UrlService.Data;
using UrlShortener.UrlService.Services;
using UrlShortener.Common.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Entity Framework
builder.Services.AddDbContext<UrlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Caching services
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    try
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "UrlShortener";
        });
        Console.WriteLine("Redis cache configured successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to configure Redis: {ex.Message}. Falling back to in-memory cache.");
        builder.Services.AddMemoryCache();
    }
}
else
{
    Console.WriteLine("No Redis connection string found. Using in-memory cache.");
    builder.Services.AddMemoryCache();
}

// Add cache service
builder.Services.AddScoped<ICacheService, CacheService>();

// Add application services
builder.Services.AddScoped<IUrlService, UrlShortener.UrlService.Services.UrlService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "URL Service API v1");
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "URL", Timestamp = DateTime.UtcNow }));

app.Run();
