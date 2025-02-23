using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FUNewsManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsArticleService _newsArticleService;
  

        public HomeController(ILogger<HomeController> logger, INewsArticleService newsArticleService)
        {
            _logger = logger;
            _newsArticleService = newsArticleService;
        }

        public async Task<IActionResult> Index()
        {

            var activeNews = await _newsArticleService.GetAllNewsArticlesAsync();

            return View(activeNews);
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
