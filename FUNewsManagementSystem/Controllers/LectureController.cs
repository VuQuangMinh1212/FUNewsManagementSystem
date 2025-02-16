using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    public class LectureController : Controller
    {

        public IActionResult ViewNewsArticles()
        {
            return View();
        }
    }
}
