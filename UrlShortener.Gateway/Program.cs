using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using UrlShortener.Gateway.HealthChecks;
using UrlShortener.Gateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add configuration for Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container
builder.Services.AddOcelot()
    .AddPolly();

// Add HTTP client factory for health checks
builder.Services.AddHttpClient();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<GatewayHealthCheck>("gateway")
    .AddCheck<DownstreamServiceHealthCheck>("downstream-services");

// Add CORS for cross-origin requests
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add logging
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<GatewayErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors();

// Add health check endpoint before Ocelot
app.MapHealthChecks("/gateway/health");

// Add a simple info endpoint
app.MapGet("/gateway/info", () => new
{
    Name = "UrlShortener Gateway",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow,
    Routes = new[]
    {
        "GET /gateway/health - Gateway health check",
        "GET /gateway/info - Gateway information",
        "* /api/v1/urlshortener/* - URL Shortener API",
        "GET /api/v1/health - Downstream service health",
        "GET /r/{shortcode} - URL redirection",
        "GET /swagger/* - API documentation"
    }
});

// Use Ocelot middleware for everything else
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/gateway"), 
    appBuilder => appBuilder.UseOcelot().Wait());

app.Run();
