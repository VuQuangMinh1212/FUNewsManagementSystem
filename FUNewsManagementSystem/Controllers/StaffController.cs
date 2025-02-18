using System.Text.Json;
using System.Text.Json.Serialization;
using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    public class StaffController : Controller
    {
        private readonly FunewsManagementContext _context;

        public StaffController(FunewsManagementContext context)
        {
            _context = context;
        }

        public IActionResult ManageCategories()
        {
            List<Category> categories = new List<Category>();
            categories = _context.Categories.AsEnumerable().ToList();
            return View(categories);
        }

        public IActionResult AddCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                _context.Categories.Add(category);
                _context.SaveChanges();

                var categories = _context.Categories.AsEnumerable().ToList();

                return PartialView("_CategoryList", categories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the account. Please try again.";
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        public IActionResult GetCategoryById(int id)
        {
            var category = _context.Categories.SingleOrDefault(cate => cate.CategoryId == id);

            if (category == null)
            {
                return Json(new { success = false, message = "Category not found" });
            }

            return Json(category, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
        }

        public IActionResult UpdateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.Categories.Update(category);
            _context.SaveChanges();

            var categories = _context.Categories.AsEnumerable().ToList();

            return PartialView("_CategoryList", categories);
        }

        public IActionResult DeleteCategory(int id)
        {
            var haveNews = _context.NewsArticles.Any(article => article.CategoryId == id);
            if (haveNews)
            {
                return Json(new { success = false, message = "Không thể xóa danh mục vì nó đang chứa bài viết!" });
            }

            var category = _context.Categories.SingleOrDefault(cate => cate.CategoryId == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }

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
