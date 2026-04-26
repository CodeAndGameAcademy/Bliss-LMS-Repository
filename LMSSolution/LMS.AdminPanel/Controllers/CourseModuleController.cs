using LMS.AdminPanel.ViewModels.CourseModule;
using LMS.Application.Exceptions;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LMS.AdminPanel.Controllers
{
    public class CourseModuleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseModuleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= GET =================
        public IActionResult Index(Guid? courseId)
        {
            var model = new CourseModulePageViewModel();

            // Load courses
            ViewBag.Courses = _context.Courses
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                }).ToList();

            // Filter modules
            if (courseId.HasValue)
            {
                model.CourseModules = _context.CourseModules
                    .IgnoreQueryFilters()
                    .Where(x => x.CourseId == courseId)
                    .OrderBy(x => x.OrderIndex)
                    .ToList();

                model.CreateCourseModule.CourseId = courseId.Value;
            }

            return View("CourseModule", model);
        }

        // ================= CREATE =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseModulePageViewModel model)
        {
            try
            {
                // Reload dropdown (important)
                ViewBag.Courses = _context.Courses
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    }).ToList();

                if (!ModelState.IsValid)
                {
                    // Reload modules if validation fails
                    model.CourseModules = await _context.CourseModules
                        .Where(x => x.CourseId == model.CreateCourseModule.CourseId)
                        .OrderBy(x => x.OrderIndex)
                        .ToListAsync();

                    return View("CourseModule", model);
                }

                // Validate OrderIndex
                bool exists = await _context.CourseModules
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.CourseId == model.CreateCourseModule.CourseId && x.OrderIndex == model.CreateCourseModule.OrderIndex);

                if (exists)
                    throw new BadRequestException("OrderIndex already exists.");




                var module = new CourseModule
                {
                    Id = Guid.NewGuid(),
                    Title = model.CreateCourseModule.Title,
                    OrderIndex = model.CreateCourseModule.OrderIndex,
                    CourseId = model.CreateCourseModule.CourseId
                };

                _context.CourseModules.Add(module);
                await _context.SaveChangesAsync();

                // Redirect with selected course
                return RedirectToAction("Index", new { courseId = model.CreateCourseModule.CourseId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Reload modules if validation fails
                model.CourseModules = await _context.CourseModules
                    .Where(x => x.CourseId == model.CreateCourseModule.CourseId)
                    .OrderBy(x => x.OrderIndex)
                    .ToListAsync();

                return View("CourseModule", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid courseId)
        {
            try
            {
                var entity = await _context.CourseModules
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Course Module not found");

                entity.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { courseId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                // Redirect with selected course
                return RedirectToAction("Index", new { courseId = courseId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id, Guid courseId)
        {
            try
            {
                var entity = await _context.CourseModules
                     .IgnoreQueryFilters()
                     .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null);

                if (entity == null)
                    throw new NotFoundException("Course Module not found or not deleted");

                entity.DeletedAt = null;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { courseId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                // Redirect with selected course
                return RedirectToAction("Index", new { courseId = courseId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, Guid courseId)
        {
            try
            {
                var entity = await _context.CourseModules
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Course Module not found");

                var model = new UpdateCourseModuleViewModel
                {
                    CourseId = courseId,
                    OrderIndex = entity.OrderIndex,
                    Title = entity.Title,
                };

                ViewBag.CourseModuleId = entity.Id;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                // Redirect with selected course
                return RedirectToAction("Index", new { courseId = courseId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateCourseModuleViewModel model)
        {
            try
            {
                var module = await _context.CourseModules
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (module == null)
                    throw new NotFoundException("Module not found.");


                if (!ModelState.IsValid)
                {
                    ViewBag.CourseModuleId = id;
                    return View(model);
                }

                // Validate OrderIndex
                bool exists = await _context.CourseModules
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.CourseId == module.CourseId &&
                                   x.OrderIndex == model.OrderIndex &&
                                   x.Id != id);

                if (exists)
                    throw new BadRequestException("OrderIndex already exists.");


                // Update
                module.Title = model.Title;
                module.OrderIndex = model.OrderIndex;
                module.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { courseId = model.CourseId });

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", new { courseId = model.CourseId });
            }
        }
    }
}
