using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("")]
    public class RedirectController : Controller
    {
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ILogger<RedirectController> _logger;

        public RedirectController(IUrlShortenerService urlShortenerService, ILogger<RedirectController> logger)
        {
            _urlShortenerService = urlShortenerService;
            _logger = logger;
        }

        /// <summary>
        /// Redirect short URL to original URL
        /// </summary>
        /// <param name="shortCode">The short code</param>
        /// <returns>Redirect to original URL</returns>
        [HttpGet("{shortCode}")]
        public async Task<ActionResult> RedirectToOriginalUrl(string shortCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(shortCode))
                {
                    return NotFound();
                }

                var shortenedUrl = await _urlShortenerService.GetByShortCodeAsync(shortCode);

                if (shortenedUrl == null)
                {
                    _logger.LogWarning("Short code {ShortCode} not found", shortCode);
                    return NotFound("Short URL not found or has expired");
                }

                // Record the click asynchronously
                var ipAddress = GetClientIpAddress();
                var userAgent = Request.Headers["User-Agent"].ToString();
                var referer = Request.Headers["Referer"].ToString();

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _urlShortenerService.RecordClickAsync(shortCode, ipAddress, userAgent, referer);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error recording click for {ShortCode}", shortCode);
                    }
                });

                _logger.LogInformation("Redirecting {ShortCode} to {OriginalUrl}", shortCode, shortenedUrl.OriginalUrl);

                return Redirect(shortenedUrl.OriginalUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing redirect for {ShortCode}", shortCode);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private string? GetClientIpAddress()
        {
            // Check for forwarded IP first (in case of proxy/load balancer)
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
