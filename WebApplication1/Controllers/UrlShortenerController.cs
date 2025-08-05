using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ILogger<UrlShortenerController> _logger;

        public UrlShortenerController(IUrlShortenerService urlShortenerService, ILogger<UrlShortenerController> logger)
        {
            _urlShortenerService = urlShortenerService;
            _logger = logger;
        }

        /// <summary>
        /// Create a shortened URL
        /// </summary>
        /// <param name="request">URL shortening request</param>
        /// <returns>Shortened URL response</returns>
        [HttpPost("shorten")]
        public async Task<ActionResult<ShortenedUrlResponse>> CreateShortenedUrl([FromBody] CreateShortenedUrlRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.Identity?.IsAuthenticated == true ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
                var result = await _urlShortenerService.CreateShortenedUrlAsync(request, userId);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shortened URL");
                return StatusCode(500, new { error = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get URL statistics by short code
        /// </summary>
        /// <param name="shortCode">The short code</param>
        /// <returns>URL statistics</returns>
        [HttpGet("{shortCode}/stats")]
        public async Task<ActionResult<UrlStatisticsResponse>> GetUrlStatistics(string shortCode)
        {
            try
            {
                var stats = await _urlShortenerService.GetUrlStatisticsAsync(shortCode);
                
                if (stats == null)
                {
                    return NotFound(new { error = "Short URL not found" });
                }

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for {ShortCode}", shortCode);
                return StatusCode(500, new { error = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get all URLs created by the authenticated user
        /// </summary>
        /// <returns>List of user's shortened URLs</returns>
        [HttpGet("my-urls")]
        [Authorize]
        public async Task<ActionResult<List<ShortenedUrlResponse>>> GetMyUrls()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var urls = await _urlShortenerService.GetUserUrlsAsync(userId);
                return Ok(urls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user URLs");
                return StatusCode(500, new { error = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Deactivate a shortened URL
        /// </summary>
        /// <param name="shortCode">The short code to deactivate</param>
        /// <returns>Success status</returns>
        [HttpDelete("{shortCode}")]
        [Authorize]
        public async Task<ActionResult> DeactivateUrl(string shortCode)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var success = await _urlShortenerService.DeactivateUrlAsync(shortCode, userId);

                if (!success)
                {
                    return NotFound(new { error = "Short URL not found or you don't have permission to delete it" });
                }

                return Ok(new { message = "URL deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating URL {ShortCode}", shortCode);
                return StatusCode(500, new { error = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>API status</returns>
        [HttpGet("health")]
        public ActionResult GetHealth()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
