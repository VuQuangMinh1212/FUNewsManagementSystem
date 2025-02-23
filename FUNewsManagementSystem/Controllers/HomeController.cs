using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FUNewsManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsArticleService _newsArticlesService;

        // Inject Logger and NewsArticlesService
        public HomeController(ILogger<HomeController> logger, INewsArticleService newsArticlesService)
        {
            _logger = logger;
            _newsArticlesService = newsArticlesService;
        }

        // Home Page - Display latest news
        public async Task<IActionResult> Index()
        {
            var latestNews = await _newsArticlesService.GetLatestNews(12);
            ViewBag.LatestNews = latestNews;
            return View();
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

        public IActionResult AccessDenied()
        {
            ViewBag.ErrorMessage = "You do not have permission to access this page.";
            return View();
        }
    }
}
