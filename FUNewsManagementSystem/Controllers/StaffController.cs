using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult ManageCategories()
        {
            return View();
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
