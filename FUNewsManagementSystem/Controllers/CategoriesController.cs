using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.BLL.Services;
using FUNewsManagementSystem.DAL.Models;
using FUNewsManagementSystem.DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FUNewsManagementSystem.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        [HttpGet("Categories/Index/page/{page:int?}")]
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {

            var categories = await _categoryService.GetAllCategoriesAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c => c.CategoryName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewData["CurrentFilter"] = searchString;

            //Pagination
            const int pageSize = 5;
            int resCount = categories.Count();
            var pager = new Pager(resCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var pagingCategories = categories.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(pagingCategories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ParentCategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddCategoryAsync(category);

                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ParentCategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            ViewData["ParentCategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", category.ParentCategoryId);
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("CategoryId,CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            if (id != category.CategoryId)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);

                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ParentCategoryId"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "CategoryId", "CategoryName", category.ParentCategoryId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            await _categoryService.DeleteCategoryAsync(id);

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
