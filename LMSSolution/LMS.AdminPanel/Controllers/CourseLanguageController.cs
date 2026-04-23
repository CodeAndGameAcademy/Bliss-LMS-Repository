using LMS.AdminPanel.ViewModels.CourseLanguage;
using LMS.Application.Exceptions;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.AdminPanel.Controllers
{
    public class CourseLanguageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseLanguageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new CourseLanguagePageViewModel
            {
                CourseLanguages = _context.CourseLanguages.IgnoreQueryFilters().ToList()
            };

            return View("CourseLanguage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseLanguagePageViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.CourseLanguages = await _context.CourseLanguages.ToListAsync();
                    return View("CourseLanguage", model);
                }


                // Validate CourseLanguage Name
                var exists = await _context.CourseLanguages
                    .AnyAsync(x => x.CourseLanguageName == model.CreateLanguage.CourseLanguageName);

                if (exists)
                    throw new BadRequestException("Language already exists");

                var entity = new CourseLanguage
                {
                    CourseLanguageName = model.CreateLanguage.CourseLanguageName,
                };

                _context.CourseLanguages.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                model.CourseLanguages = await _context.CourseLanguages.ToListAsync();
                ModelState.AddModelError("", ex.Message);
                return View("CourseLanguage", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var entity = await _context.CourseLanguages.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Language not found");

                entity.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var entity = await _context.CourseLanguages
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null);

                if (entity == null)
                    throw new NotFoundException("Language not found or not deleted");

                entity.DeletedAt = null;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var entity = await _context.CourseLanguages
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Language Not Found");

                var model = new UpdateCourseLanguageViewModel
                {
                    CourseLanguageName = entity.CourseLanguageName,
                };

                ViewBag.CourseLanguageId = entity.Id;

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong while loading slider. - " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateCourseLanguageViewModel model)
        {
            try
            {
                var entity = await _context.CourseLanguages
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Language Not Found");

                // Reload UI data if invalid
                if (!ModelState.IsValid)
                {
                    ViewBag.CourseLanguageId = id;
                    return View(model);
                }

                // Check duplicate OrderIndex
                var exists = await _context.CourseLanguages
                    .AnyAsync(x => x.CourseLanguageName == model.CourseLanguageName && x.Id != id);

                if (exists)
                {
                    ModelState.AddModelError("", "Language already exists");
                    ViewBag.CourseLanguageId = id;
                    return View(model);
                }

                // update fields
                entity.CourseLanguageName = model.CourseLanguageName;
                entity.UpdatedAt = DateTime.UtcNow;

                _context.CourseLanguages.Update(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.CourseLanguageId = id;

                return View(model);
            }
        }
    }
}