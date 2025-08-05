using Microsoft.AspNetCore.Mvc;
using UrlShortener.UrlService.Services;
using UrlShortener.Contracts.Dtos;

namespace UrlShortener.UrlService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlsController : ControllerBase
{
    private readonly IUrlService _urlService;
    private readonly ILogger<UrlsController> _logger;

    public UrlsController(IUrlService urlService, ILogger<UrlsController> logger)
    {
        _urlService = urlService;
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

            var result = await _urlService.CreateShortenedUrlAsync(request);
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
    /// Get original URL by short code
    /// </summary>
    /// <param name="shortCode">The short code</param>
    /// <returns>Original URL</returns>
    [HttpGet("{shortCode}")]
    public async Task<ActionResult<string>> GetOriginalUrl(string shortCode)
    {
        try
        {
            var originalUrl = await _urlService.GetOriginalUrlAsync(shortCode);
            if (originalUrl == null)
            {
                return NotFound(new { error = "Short URL not found" });
            }

            return Ok(new { originalUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving URL for short code: {ShortCode}", shortCode);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Redirect to original URL
    /// </summary>
    /// <param name="shortCode">The short code</param>
    /// <returns>Redirect response</returns>
    [HttpGet("redirect/{shortCode}")]
    public async Task<IActionResult> RedirectToUrl(string shortCode)
    {
        try
        {
            var originalUrl = await _urlService.GetOriginalUrlAsync(shortCode);
            if (originalUrl == null)
            {
                return NotFound();
            }

            // Track the click (for future analytics integration)
            await _urlService.TrackClickAsync(shortCode, HttpContext);

            return Redirect(originalUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redirecting for short code: {ShortCode}", shortCode);
            return StatusCode(500);
        }
    }
}
