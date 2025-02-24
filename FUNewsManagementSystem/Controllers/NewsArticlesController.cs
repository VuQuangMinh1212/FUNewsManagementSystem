using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.BLL.ViewModels;
using FUNewsManagementSystem.DAL.Models;
using FUNewsManagementSystem.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FUNewsManagementSystem.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _accountService;
        private readonly ITagService _tagService;

        public NewsArticlesController(ITagService tagsService, INewsArticleService newsArticleService, ICategoryService categoryService, ISystemAccountService accountService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _accountService = accountService;
            _tagService = tagsService;
        }
        [HttpGet("NewsArticles/Index/page/{page:int?}")]
        public async Task<IActionResult> Index(string searchTitle, int? categoryFilter, int page = 1)
        {
            var articles = await _newsArticleService.GetFilteredNewsArticlesAsync(searchTitle, categoryFilter);
            var categories = await _categoryService.GetAllCategoriesAsync();

            ViewData["Categories"] = categories;
            ViewData["CurrentTitleFilter"] = searchTitle;
            ViewData["CurrentCategoryFilter"] = categoryFilter;



            //Pagination
            const int pageSize = 5;
            int resCount = articles.Count();
            var pager = new Pager(resCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var pagingNewsArticles = articles.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(pagingNewsArticles);
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
            ViewData["Tags"] = new SelectList(_tagService.GetAllTags(), "TagId", "TagName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create(CreateNewArticleViewModel newsArticleViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", newsArticleViewModel.CategoryId);
                ViewData["CreatedById"] = new SelectList(await _accountService.GetAllAccountsAsync(), "AccountId", "AccountName", newsArticleViewModel.CreatedById);
                ViewData["Tags"] = new SelectList(_tagService.GetAllTags(), "TagId", "TagName");

                return View(newsArticleViewModel);
            }

            var newsArticle = new NewsArticle
            {
                CategoryId = newsArticleViewModel.CategoryId,
                CreatedById = newsArticleViewModel.CreatedById,
                CreatedDate = newsArticleViewModel.CreatedDate,
                Headline = newsArticleViewModel.Headline,
                NewsArticleId = newsArticleViewModel.NewsArticleId,
                NewsContent = newsArticleViewModel.NewsContent,
                NewsSource = newsArticleViewModel.NewsSource,
                NewsStatus = newsArticleViewModel.NewsStatus,
                NewsTitle = newsArticleViewModel.NewsTitle
            };

            await _newsArticleService.AddNewsArticleAsync(newsArticle, newsArticleViewModel.TagsId);

            TempData["SuccessMessage"] = "NewsArticle created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(string id)
        {
            var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (newsArticle == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["Tags"] = new SelectList(_tagService.GetAllTags(), "TagId", "TagName");

            var createArticleViewModel = new CreateNewArticleViewModel
            {
                CategoryId = newsArticle.CategoryId,
                CreatedDate = newsArticle.CreatedDate,
                CreatedById = newsArticle.CreatedById,
                Headline = newsArticle.Headline,
                NewsArticleId = newsArticle.NewsArticleId,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                NewsStatus = newsArticle.NewsStatus,
                NewsTitle = newsArticle.NewsTitle
            };
            return View(createArticleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(string id, CreateNewArticleViewModel editArticle)
        {
            if (id != editArticle.NewsArticleId) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", editArticle.CategoryId);
                return View(editArticle);
            }

            var updateArticle = new NewsArticle
            {
                CategoryId = editArticle.CategoryId,
                CreatedById = editArticle.CreatedById,
                CreatedDate = editArticle.CreatedDate,
                Headline = editArticle.Headline,
                NewsArticleId = editArticle.NewsArticleId,
                NewsContent = editArticle.NewsContent,
                NewsSource = editArticle.NewsSource,
                NewsStatus = editArticle.NewsStatus,
                NewsTitle = editArticle.NewsTitle
            };

            await _newsArticleService.UpdateNewsArticleAsync(updateArticle, editArticle.TagsId); // No bool return

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

        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = new NewsReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                NewsArticles = await _newsArticleService.GetNewsReportAsync(startDate, endDate)
            };

            return View(viewModel);
        }
    }
}
