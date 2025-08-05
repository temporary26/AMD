using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUrlShortenerService _urlShortenerService;

        public HomeController(ILogger<HomeController> logger, IUrlShortenerService urlShortenerService)
        {
            _logger = logger;
            _urlShortenerService = urlShortenerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(CreateShortenedUrlRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(request);
                }

                var userId = User.Identity?.IsAuthenticated == true ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
                var result = await _urlShortenerService.CreateShortenedUrlAsync(request, userId);

                ViewBag.Result = result;
                ViewBag.Success = true;
                return View();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shortened URL");
                ModelState.AddModelError("", "An error occurred while processing your request");
                return View(request);
            }
        }

        public async Task<IActionResult> MyUrls()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var urls = await _urlShortenerService.GetUserUrlsAsync(userId!);
            return View(urls);
        }

        public async Task<IActionResult> Statistics(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode))
            {
                return NotFound();
            }

            var stats = await _urlShortenerService.GetUrlStatisticsAsync(shortCode);
            if (stats == null)
            {
                return NotFound();
            }

            return View(stats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
