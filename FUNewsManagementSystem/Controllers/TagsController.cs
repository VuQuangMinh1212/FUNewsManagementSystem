using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Controllers
{
    public class TagsController : Controller
    {
        private readonly FunewsManagementContext _context;

        public TagsController(FunewsManagementContext context)
        {
            _context = context;
        }

        // GET: Tags
        public async Task<IActionResult> Index(string searchString)
        {
            var tags = from t in _context.Tags select t;

            if (!string.IsNullOrEmpty(searchString))
            {
                tags = tags.Where(t => t.TagName.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await tags.ToListAsync());
        }


        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TagId,TagName,Note")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tag.TagId = (short)(_context.Tags.Max(a => (int?)a.TagId) + 1 ?? 1);
                    _context.Add(tag);
                    await _context.SaveChangesAsync();
                    ViewData["SuccessMessage"] = "Create tag successfully.";
                    return View(tag);
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = "An error occurred while creating the tag. Please try again.";
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(tag);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TagId,TagName,Note")] Tag tag)
        {
            if (id != tag.TagId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tag);
                    ViewData["SuccessMessage"] = "Edit tag successfully.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.TagId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "An error occurred while editing the tag. Please try again.";
                        throw;
                    }
                }
                return View(tag);
            }
            return View(tag);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var tag = await _context.Tags.FindAsync(id);
                if (tag == null)
                {
                    ViewData["ErrorMessage"] = "Tag not found.";
                    return View(tag);
                }

                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
                ViewData["SuccessMessage"] = "Tag deleted successfully.";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while deleting the tag. Please try again.";
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }
    }
}
