using LMS.AdminPanel.Common.Constants;
using LMS.AdminPanel.Services;
using LMS.AdminPanel.ViewModels.Slider;
using LMS.AdminPanel.Exceptions;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.AdminPanel.Controllers
{
    public class SliderController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public SliderController(IFileService fileService, ApplicationDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new SliderPageViewModel
            {
                Sliders = _context.Sliders.OrderBy(x => x.OrderIndex).ToList()
            };

            return View("Slider", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderPageViewModel model)
        {
            string? uploadedPath = null;

            try
            {
                if (!ModelState.IsValid)
                {
                    model.Sliders = await _context.Sliders
                        .OrderBy(x => x.OrderIndex)
                        .ToListAsync();

                    return View("Slider", model);
                }


                // Validate OrderIndex
                var exists = await _context.Sliders
                    .AnyAsync(x => x.OrderIndex == model.CreateSlider.OrderIndex);

                if (exists)
                    throw new BadRequestException("OrderIndex already exists");

                var uploadResult = await _fileService.UploadAsync(
                    model.CreateSlider.ImageFile,
                    "sliders",
                    FileConstants.Image.AllowedExtensions,
                    FileConstants.Image.MaxSize
                );

                uploadedPath = uploadResult.FilePath;

                var slider = new Slider
                {
                    Image = uploadedPath,
                    OrderIndex = model.CreateSlider.OrderIndex
                };

                _context.Sliders.Add(slider);
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

                model.Sliders = await _context.Sliders
                       .OrderBy(x => x.OrderIndex)
                       .ToListAsync();

                ModelState.AddModelError("", ex.Message);
                return View("Slider", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);

                if (slider == null)
                    throw new NotFoundException("Slider not found");

                // delete image from server
                if (!string.IsNullOrEmpty(slider.Image))
                {
                    await _fileService.DeleteAsync(slider.Image);
                }

                _context.Sliders.Remove(slider);
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
                var slider = await _context.Sliders
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (slider == null)
                    throw new NotFoundException("Slider Not Found");

                var model = new UpdateSliderViewModel
                {
                    OrderIndex = slider.OrderIndex,
                    ExistingImage = slider.Image
                };

                ViewBag.SliderId = slider.Id;

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
        public async Task<IActionResult> Edit(Guid id, UpdateSliderViewModel model)
        {
            string? newImagePath = null;

            try
            {
                var slider = await _context.Sliders
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (slider == null)
                    throw new NotFoundException("Slider Not Found");

                // Reload UI data if invalid
                if (!ModelState.IsValid)
                {
                    model.ExistingImage = slider.Image;
                    ViewBag.SliderId = id;
                    return View(model);
                }

                // Check duplicate OrderIndex
                var exists = await _context.Sliders
                    .AnyAsync(x => x.OrderIndex == model.OrderIndex && x.Id != id);

                if (exists)
                {
                    ModelState.AddModelError("", "OrderIndex already exists");
                    model.ExistingImage = slider.Image;
                    ViewBag.SliderId = id;
                    return View(model);
                }

                // IMAGE REPLACEMENT (optional)
                if (model.ImageFile != null)
                {
                    var uploadResult = await _fileService.UploadAsync(
                        model.ImageFile,
                        "sliders",
                        FileConstants.Image.AllowedExtensions,
                        FileConstants.Image.MaxSize
                    );

                    newImagePath = uploadResult.FilePath;

                    // delete old image
                    if (!string.IsNullOrEmpty(slider.Image))
                    {
                        await _fileService.DeleteAsync(slider.Image);
                    }

                    slider.Image = newImagePath;
                }

                // update fields
                slider.OrderIndex = model.OrderIndex;
                slider.UpdatedAt = DateTime.UtcNow;

                _context.Sliders.Update(slider);
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
                var existing = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);
                if (existing != null)
                {
                    model.ExistingImage = existing.Image;
                }
                ViewBag.SliderId = id;

                return View(model);
            }
        }
    }
}