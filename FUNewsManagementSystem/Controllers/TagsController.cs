using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using FUNewsManagementSystem.DAL.ViewModel;
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

        [HttpGet("Tags/Index/page/{page:int?}")]
        public IActionResult Index(string searchString, int page = 1)
        {
            var tags = _tagService.GetAllTags();

            if (!string.IsNullOrEmpty(searchString))
            {
                tags = tags.Where(t => t.TagName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewData["CurrentFilter"] = searchString;

            // Pagination
            const int pageSize = 5;
            int resCount = tags.Count();
            var pager = new Pager(resCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var pagingTags = tags.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(pagingTags);
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
                    var existsTag = _tagService.GetTagById(tag.TagId);
                    if (existsTag != null)
                    {
                        ViewData["ErrorMessage"] = "Tag has exists in database";
                        return View();
                    }

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