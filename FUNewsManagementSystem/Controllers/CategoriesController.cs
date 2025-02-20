using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly FunewsManagementContext _context;

        public CategoriesController(FunewsManagementContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string searchString)
        {
            var categories = _context.Categories.Include(c => c.ParentCategory).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c => c.CategoryName.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    ViewData["SuccessMessage"] = "Create category successfully.";
                    return View(category);
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = "An error occurred while creating the category. Please try again.";
                    ModelState.AddModelError("", ex.Message);
                }
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", category.ParentCategoryId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("CategoryId,CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    ViewData["SuccessMessage"] = "Update category successfully.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "An error occurred while editing the tag. Please try again.";
                        throw;
                    }
                }
                return View(category);
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var category = await _context.Categories
                .Include(c => c.NewsArticles) // Include related NewsArticles
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            // Check if the category has any associated NewsArticles
            if (category.NewsArticles.Any())
            {
                ViewData["ErrorMessage"] = "Cannot delete this category because it is linked to existing news articles.";
                return View(category);
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                ViewData["SuccessMessage"] = "Category deleted successfully.";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while deleting the category.";
            }

            return View(category);
        }


        private bool CategoryExists(short id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}

