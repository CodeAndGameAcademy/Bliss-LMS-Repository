using LMS.AdminPanel.Common.Constants;
using LMS.AdminPanel.Exceptions;
using LMS.AdminPanel.Services;
using LMS.AdminPanel.ViewModels.Instructor;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.AdminPanel.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public InstructorController(IFileService fileService, ApplicationDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _context.Instructors.ToList();
            return View(model);
        }

        public async Task<IActionResult> DeletedListAsync()
        {
            var model = await _context.Instructors
                        .IgnoreQueryFilters()
                        .Where(x => x.DeletedAt != null).ToListAsync();

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInstructorViewModel model)
        {
            string? uploadedPath = null;

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Validate OrderIndex
                var exists = await _context.Instructors
                    .AnyAsync(x => x.DisplayName == model.DisplayName);

                if (exists)
                    throw new BadRequestException("Display Name already exists");

                string imagePath;
                string baseUrl = "";

                if (model.ImageFile != null)
                {
                    var uploadResult = await _fileService.UploadAsync(
                        model.ImageFile,
                        "instructors",
                        FileConstants.Image.AllowedExtensions,
                        FileConstants.Image.MaxSize);

                    uploadedPath = uploadResult.FilePath;
                    imagePath = uploadedPath;
                    baseUrl = uploadResult.BaseUrl;
                }
                else
                {
                    imagePath = FileConstants.Defaults.InstructorImage; // 👈 default image
                }

                var instructor = new Instructor
                {
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                    About = model.About,
                    Degree = model.Degree,
                    CertificationSkill = model.CertificationSkill,
                    BaseUrl = baseUrl,
                    Image = imagePath,
                };

                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Cleanup if file was uploaded
                if (!string.IsNullOrEmpty(uploadedPath))
                {
                    await _fileService.DeleteAsync(uploadedPath);
                }

                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var entity = await _context.Instructors.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Instructor not found");

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
                var entity = await _context.Instructors
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null);

                if (entity == null)
                    throw new NotFoundException("Instructor not found or not deleted");

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
                var entity = await _context.Instructors
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Instructor Not Found");

                var model = new UpdateInstructorViewModel
                {
                    Name = entity.Name,
                    DisplayName = entity.DisplayName,
                    Degree = entity.Degree,
                    CertificationSkill = entity.CertificationSkill,
                    About = entity.About,
                    ExistingImage = entity.Image
                };

                ViewBag.InstructorId = entity.Id;

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong while loading Instructor. - " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateInstructorViewModel model)
        {
            string? newImagePath = null;

            try
            {
                var entity = await _context.Instructors
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Instructor Not Found");

                // Reload UI data if invalid
                if (!ModelState.IsValid)
                {
                    model.ExistingImage = entity.Image;
                    ViewBag.InstructorId = id;
                    return View(model);
                }

                // Check duplicate Display Name
                var exists = await _context.Instructors
                    .AnyAsync(x => x.DisplayName == model.DisplayName && x.Id != id);

                if (exists)
                {
                    ModelState.AddModelError("", "Display Name already exists");
                    model.ExistingImage = entity.Image;
                    ViewBag.InstructorId = id;
                    return View(model);
                }

                // IMAGE REPLACEMENT (optional)
                if (model.ImageFile != null)
                {
                    var uploadResult = await _fileService.UploadAsync(
                        model.ImageFile,
                        "instructors",
                        FileConstants.Image.AllowedExtensions,
                        FileConstants.Image.MaxSize
                    );

                    newImagePath = uploadResult.FilePath;

                    // delete old image
                    if (!string.IsNullOrEmpty(entity.Image))
                    {
                        await _fileService.DeleteAsync(entity.Image);
                    }

                    entity.Image = newImagePath;
                }

                // update fields
                entity.Name = model.Name;
                entity.DisplayName = model.DisplayName;
                entity.About = model.About;
                entity.Degree = model.Degree;
                entity.CertificationSkill = model.CertificationSkill;
                entity.UpdatedAt = DateTime.UtcNow;

                _context.Instructors.Update(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // rollback uploaded file if error happens
                if (!string.IsNullOrEmpty(newImagePath))
                {
                    await _fileService.DeleteAsync(newImagePath);
                }

                ModelState.AddModelError("", ex.Message);

                // reload existing image for UI
                var existing = await _context.Instructors.FirstOrDefaultAsync(x => x.Id == id);
                if (existing != null)
                {
                    model.ExistingImage = existing.Image;
                }
                ViewBag.InstructorId = id;

                return View(model);
            }
        }
    }
}
