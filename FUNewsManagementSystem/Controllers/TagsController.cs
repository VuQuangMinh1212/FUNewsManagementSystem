using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public IActionResult Index(string searchString)
        {
            var tags = string.IsNullOrEmpty(searchString)
                ? _tagService.GetAllTags()
                : _tagService.GetAllTags().Where(t => t.TagName.Contains(searchString));

            ViewData["CurrentFilter"] = searchString;
            return View(tags);
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var tag = _tagService.GetTagById(id.Value);
            return tag == null ? NotFound() : View(tag);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("TagId,TagName,Note")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _tagService.AddTag(tag);
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

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var tag = _tagService.GetTagById(id.Value);
            return tag == null ? NotFound() : View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("TagId,TagName,Note")] Tag tag)
        {
            if (id != tag.TagId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _tagService.UpdateTag(tag);
                    ViewData["SuccessMessage"] = "Edit tag successfully.";
                    return View(tag);
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = "An error occurred while editing the tag. Please try again.";
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(tag);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var tag = _tagService.GetTagById(id.Value);
            return tag == null ? NotFound() : View(tag);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _tagService.DeleteTag(id);
                ViewData["SuccessMessage"] = "Tag deleted successfully.";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while deleting the tag. Please try again.";
                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}