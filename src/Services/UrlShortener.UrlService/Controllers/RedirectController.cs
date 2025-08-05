using Microsoft.AspNetCore.Mvc;
using UrlShortener.UrlService.Services;

namespace UrlShortener.UrlService.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly ILogger<RedirectController> _logger;

        public RedirectController(IUrlService urlService, ILogger<RedirectController> logger)
        {
            _urlService = urlService;
            _logger = logger;
        }

        /// <summary>
        /// Simple redirect endpoint - just /{shortCode}
        /// </summary>
        /// <param name="shortCode">The short code</param>
        /// <returns>Redirect to original URL</returns>
        [HttpGet("/{shortCode}")]
        public async Task<IActionResult> RedirectToUrl(string shortCode)
        {
            try
            {
                var originalUrl = await _urlService.GetOriginalUrlAsync(shortCode);
                if (originalUrl == null)
                {
                    return NotFound("Short URL not found");
                }

                // Track the click
                await _urlService.TrackClickAsync(shortCode, HttpContext);

                _logger.LogInformation("Redirecting {ShortCode} to {OriginalUrl}", shortCode, originalUrl);
                return Redirect(originalUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redirecting for short code: {ShortCode}", shortCode);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
