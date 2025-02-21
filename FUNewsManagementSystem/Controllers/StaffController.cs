using System.Text.Json;
using System.Text.Json.Serialization;
using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Controllers
{
    public class StaffController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly INewsArticleService _newsArticleService;

        public StaffController(ICategoryService categoryService, INewsArticleService newsArticleService)
        {
            _categoryService = categoryService;
            _newsArticleService = newsArticleService;
        }

        public async Task<IActionResult> ManageCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _categoryService.AddCategoryAsync(category);
                var categories = await _categoryService.GetAllCategoriesAsync();

                return PartialView("_CategoryList", categories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the category. Please try again.";
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        public async Task<IActionResult> GetCategoryById(short id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return Json(new { success = false, message = "Category not found" });
            }

            return Json(category, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _categoryService.UpdateCategoryAsync(category);
            var categories = await _categoryService.GetAllCategoriesAsync();

            return PartialView("_CategoryList", categories);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(short id)
        {
            var haveNews = await _newsArticleService.HasNewsInCategoryAsync(id);
            if (haveNews)
            {
                return Json(new { success = false, message = "Không thể xóa danh mục vì nó đang chứa bài viết!" });
            }

            await _categoryService.DeleteCategoryAsync(id);
            return Json(new { success = true });
        }

        public IActionResult ManageNews()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
