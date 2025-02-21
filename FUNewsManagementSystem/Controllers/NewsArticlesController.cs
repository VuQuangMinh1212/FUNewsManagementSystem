using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _accountService;

        public NewsArticlesController(INewsArticleService newsArticleService, ICategoryService categoryService, ISystemAccountService accountService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(string searchTitle, int? categoryFilter)
        {
            var articles = await _newsArticleService.GetFilteredNewsArticlesAsync(searchTitle, categoryFilter);
            var categories = await _categoryService.GetAllCategoriesAsync();

            ViewData["Categories"] = categories;
            ViewData["CurrentTitleFilter"] = searchTitle;
            ViewData["CurrentCategoryFilter"] = categoryFilter;

            return View(articles);
        }

        public async Task<IActionResult> Details(string id)
        {
            var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (newsArticle == null) return NotFound();

            return View(newsArticle);
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName");
            ViewData["CreatedById"] = new SelectList(await _accountService.GetAllAccountsAsync(), "AccountId", "AccountName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create(NewsArticle newsArticle)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", newsArticle.CategoryId);
                ViewData["CreatedById"] = new SelectList(await _accountService.GetAllAccountsAsync(), "AccountId", "AccountName", newsArticle.CreatedById);
                return View(newsArticle);
            }

            await _newsArticleService.AddNewsArticleAsync(newsArticle); // No bool return

            TempData["SuccessMessage"] = "NewsArticle created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(string id)
        {
            var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (newsArticle == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", newsArticle.CategoryId);
            return View(newsArticle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(string id, NewsArticle newsArticle)
        {
            if (id != newsArticle.NewsArticleId) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", newsArticle.CategoryId);
                return View(newsArticle);
            }

            await _newsArticleService.UpdateNewsArticleAsync(newsArticle); // No bool return

            TempData["SuccessMessage"] = "News article updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (newsArticle == null) return NotFound();

            return View(newsArticle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _newsArticleService.DeleteNewsArticleAsync(id); // No bool return

            TempData["SuccessMessage"] = "News article deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> History()
        {
            // Get logged-in user's ID
            var userIdClaim = User.FindFirst("AccountId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !short.TryParse(userIdClaim, out short userId))
            {
                return Unauthorized();
            }

            // Call service to retrieve news articles
            var staffNews = await _newsArticleService.GetNewsArticlesByStaffIdAsync(userId);

            return View(staffNews);
        }
    }
}
