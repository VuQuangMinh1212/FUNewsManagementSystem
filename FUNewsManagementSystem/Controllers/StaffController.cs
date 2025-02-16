using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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


            _context.Categories.Add(category);
            _context.SaveChanges();

            var categories = _context.Categories.AsEnumerable().ToList();

            return PartialView("_CategoryList", categories);
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
