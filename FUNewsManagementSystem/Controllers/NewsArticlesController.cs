using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly FunewsManagementContext _context;

        public NewsArticlesController(FunewsManagementContext context)
        {
            _context = context;
        }

        // GET: NewsArticles
        public async Task<IActionResult> Index(string searchTitle, int? categoryFilter)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTitle))
            {
                query = query.Where(n => n.NewsTitle.Contains(searchTitle));
            }

            if (categoryFilter.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryFilter);
            }

            ViewData["CurrentTitleFilter"] = searchTitle;
            ViewData["CurrentCategoryFilter"] = categoryFilter;
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(await query.ToListAsync());
        }



        // GET: NewsArticles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .FirstOrDefaultAsync(m => m.NewsArticleId == id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            return View(newsArticle);
        }

        [Authorize(Roles = "Staff")]
        // GET: NewsArticles/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["CreatedById"] = new SelectList(_context.SystemAccounts, "AccountId", "AccountName");
            return View();
        }

        [Authorize(Roles = "Staff")]
        // POST: NewsArticles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById,UpdatedById,ModifiedDate")] NewsArticle newsArticle)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(newsArticle);
                    await _context.SaveChangesAsync();
                    ViewData["SuccessMessage"] = "NewsArticle created successfully!";
                    return View(newsArticle);
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = "An error occurred while creating the article. Please try again.";
                    ModelState.AddModelError("", ex.Message);
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(_context.SystemAccounts, "AccountId", "AccountName", newsArticle.CreatedById);
            return View(newsArticle);
        }

        [Authorize(Roles = "Staff")]
        // GET: NewsArticles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = await _context.NewsArticles.FindAsync(id);
            if (newsArticle == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(_context.SystemAccounts, "AccountId", "AccountName", newsArticle.CreatedById);
            return View(newsArticle);
        }

        [Authorize(Roles = "Staff")]
        // POST: NewsArticles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById,UpdatedById,ModifiedDate")] NewsArticle newsArticle)
        {
            if (id != newsArticle.NewsArticleId)
            {
                ViewData["ErrorMessage"] = "Invalid article ID.";
                return View(newsArticle);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsArticle);
                    await _context.SaveChangesAsync();
                    ViewData["SuccessMessage"] = "NewsArticle updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsArticleExists(newsArticle.NewsArticleId))
                    {
                        ViewData["ErrorMessage"] = "Article not found.";
                        return View(newsArticle);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "An error occurred while updating the article.";
                    }
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "Update failed. Please check the input.";
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["CreatedById"] = new SelectList(_context.SystemAccounts, "AccountId", "AccountName", newsArticle.CreatedById);
            return View(newsArticle);
        }

        [Authorize(Roles = "Staff")]
        // GET: NewsArticles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .FirstOrDefaultAsync(m => m.NewsArticleId == id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            return View(newsArticle);
        }

        [Authorize(Roles = "Staff")]
        // POST: NewsArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var newsArticle = await _context.NewsArticles.FindAsync(id);
            if (newsArticle == null)
            {
                ViewData["ErrorMessage"] = "News article not found.";
                return View("Delete"); // Giữ nguyên trang Delete
            }

            try
            {
                _context.NewsArticles.Remove(newsArticle);
                await _context.SaveChangesAsync();

                ViewData["SuccessMessage"] = "News article deleted successfully!";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while deleting the article. Please try again.";
            }

            return View("Delete", newsArticle);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate <= endDate.Value);
            }

            var reportData = await query.OrderByDescending(n => n.CreatedDate).ToListAsync();

            return View(reportData);
        }


        private bool NewsArticleExists(string id)
        {
            return _context.NewsArticles.Any(e => e.NewsArticleId == id);
        }


        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> History()
        {
            // Get logged-in user's ID
            var userId = Convert.ToInt16(User.FindFirst("AccountId")?.Value);

            if (userId == null)
            {
                return Unauthorized();
            }

            // Retrieve news articles created by the logged-in staff
            var staffNews = await _context.NewsArticles
                .Include(n => n.Category)
                .Where(n => n.CreatedById == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return View(staffNews);
        }
    }
}