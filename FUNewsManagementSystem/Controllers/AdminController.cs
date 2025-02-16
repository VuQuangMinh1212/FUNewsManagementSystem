using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ManageAccounts()
        {
            return View();
        }
        public IActionResult Report()
        {
            return View();
        }
        public IActionResult EditAccount()
        {
            return View();
        }
    }
}
