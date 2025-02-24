using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.ViewModel;
using FUNewsManagementSystem.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("NewsArticles/Index/page/{page:int?}")]
        public async Task<IActionResult> Index(string searchTitle, int? categoryFilter, int page = 1)
        {
            var articles = await _newsArticleService.GetFilteredNewsArticlesAsync(searchTitle, categoryFilter);
            var categories = await _categoryService.GetAllCategoriesAsync();

            ViewData["Categories"] = categories;
            ViewData["CurrentTitleFilter"] = searchTitle;
            ViewData["CurrentCategoryFilter"] = categoryFilter;

            // Pagination
            const int pageSize = 5;
            int resCount = articles.Count();
            var pager = new Pager(resCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var pagingNewsArticles = articles.Skip(recSkip).Take(pager.PageSize)
                .Select(news => new NewsArticleViewModel
                {
                    NewsArticleId = news.NewsArticleId,
                    NewsTitle = news.NewsTitle,
                    Headline = news.Headline,
                    CreatedDate = news.CreatedDate,
                    NewsContent = news.NewsContent,
                    NewsSource = news.NewsSource,
                    CategoryId = news.CategoryId,
                    NewsStatus = news.NewsStatus,
                    CreatedById = news.CreatedById,
                    UpdatedById = news.UpdatedById,
                    ModifiedDate = news.ModifiedDate,
                    CategoryName = news.Category != null ? news.Category.CategoryName : "None",
                    CreatedByName = news.CreatedBy != null ? news.CreatedBy.AccountName : "Unknown"
                }).ToList();

            ViewBag.Pager = pager;
            return View(pagingNewsArticles);
        }

        public async Task<IActionResult> Details(string id)
        {
            var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (newsArticle == null)
                return NotFound();

            var viewModel = new NewsArticleViewModel
            {
                NewsArticleId = newsArticle.NewsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CategoryId = newsArticle.CategoryId,
                NewsStatus = newsArticle.NewsStatus,
                CreatedById = newsArticle.CreatedById,
                UpdatedById = newsArticle.UpdatedById,
                ModifiedDate = newsArticle.ModifiedDate
            };

            return View(viewModel);
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
        public async Task<IActionResult> Create(NewsArticleViewModel newsArticleVM)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", newsArticleVM.CategoryId);
                ViewData["CreatedById"] = new SelectList(await _accountService.GetAllAccountsAsync(), "AccountId", "AccountName", newsArticleVM.CreatedById);
                return View(newsArticleVM);
            }

            // Map the ViewModel to the DAL model
            var newsArticle = new FUNewsManagementSystem.DAL.Models.NewsArticle
            {
                NewsArticleId = newsArticleVM.NewsArticleId,
                NewsTitle = newsArticleVM.NewsTitle,
                Headline = newsArticleVM.Headline,
                CreatedDate = newsArticleVM.CreatedDate,
                NewsContent = newsArticleVM.NewsContent,
                NewsSource = newsArticleVM.NewsSource,
                CategoryId = newsArticleVM.CategoryId,
                NewsStatus = newsArticleVM.NewsStatus,
                CreatedById = newsArticleVM.CreatedById,
                UpdatedById = newsArticleVM.UpdatedById,
                ModifiedDate = newsArticleVM.ModifiedDate
            };

            await _newsArticleService.AddNewsArticleAsync(newsArticle);

            TempData["SuccessMessage"] = "NewsArticle created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(string id)
        {
            var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (newsArticle == null)
                return NotFound();

            var viewModel = new NewsArticleViewModel
            {
                NewsArticleId = newsArticle.NewsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CategoryId = newsArticle.CategoryId,
                NewsStatus = newsArticle.NewsStatus,
                CreatedById = newsArticle.CreatedById,
                UpdatedById = newsArticle.UpdatedById,
                ModifiedDate = newsArticle.ModifiedDate
            };

            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", newsArticle.CategoryId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(string id, NewsArticleViewModel newsArticleVM)
        {
            if (id != newsArticleVM.NewsArticleId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", newsArticleVM.CategoryId);
                return View(newsArticleVM);
            }

            // Map the ViewModel to the DAL model
            var newsArticle = new FUNewsManagementSystem.DAL.Models.NewsArticle
            {
                NewsArticleId = newsArticleVM.NewsArticleId,
                NewsTitle = newsArticleVM.NewsTitle,
                Headline = newsArticleVM.Headline,
                CreatedDate = newsArticleVM.CreatedDate,
                NewsContent = newsArticleVM.NewsContent,
                NewsSource = newsArticleVM.NewsSource,
                CategoryId = newsArticleVM.CategoryId,
                NewsStatus = newsArticleVM.NewsStatus,
                CreatedById = newsArticleVM.CreatedById,
                UpdatedById = newsArticleVM.UpdatedById,
                ModifiedDate = newsArticleVM.ModifiedDate
            };

            await _newsArticleService.UpdateNewsArticleAsync(newsArticle);

            TempData["SuccessMessage"] = "News article updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (newsArticle == null)
                return NotFound();

            var viewModel = new NewsArticleViewModel
            {
                NewsArticleId = newsArticle.NewsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CategoryId = newsArticle.CategoryId,
                NewsStatus = newsArticle.NewsStatus,
                CreatedById = newsArticle.CreatedById,
                UpdatedById = newsArticle.UpdatedById,
                ModifiedDate = newsArticle.ModifiedDate
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _newsArticleService.DeleteNewsArticleAsync(id);

            TempData["SuccessMessage"] = "News article deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
