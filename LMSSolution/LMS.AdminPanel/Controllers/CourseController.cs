using LMS.AdminPanel.Common.Constants;
using LMS.AdminPanel.Helpers;
using LMS.AdminPanel.Services;
using LMS.AdminPanel.ViewModels.Course;
using LMS.AdminPanel.Exceptions;
using LMS.Domain.Common;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LMS.AdminPanel.Controllers
{
    public class CourseController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public CourseController(IFileService fileService, ApplicationDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _context.Courses.ToList();
            return View(model);
        }

        public async Task<IActionResult> DeletedListAsync()
        {
            var model = await _context.Courses
                        .IgnoreQueryFilters()
                        .Where(x => x.DeletedAt != null).ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var entity = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Course not found");

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
                var entity = await _context.Courses
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null);

                if (entity == null)
                    throw new NotFoundException("Course not found or not deleted");

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

        public IActionResult Create()
        {
            LoadDropdowns();

            var model = new CreateCourseViewModel();
            model.CourseLearningOutcomes = new List<string>() { "" };
            model.CourseRequirements = new List<string>() { "" };
            model.CourseTargetAudiences = new List<string>() { "" };

            return View("Create", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CreateCourseViewModel model)
        {
            string? uploadedPath = null;

            try
            {
                ValidateCollection(model);

                var text = Regex.Replace(model.LongDescription, "<.*?>", "").Trim();
                if (string.IsNullOrEmpty(text))
                {
                    ModelState.AddModelError("LongDescription", "Long Description is required");
                }


                if (!ModelState.IsValid)
                {
                    LoadDropdowns();
                    return View(model);
                }

                // Validate Course Title
                var exists = await _context.Courses.IgnoreQueryFilters().AnyAsync(x => x.Title == model.Title);
                if (exists) throw new BadRequestException("Course already exists");

                // Validate Course Slug
                exists = await _context.Courses.IgnoreQueryFilters().AnyAsync(x => x.Slug == model.Slug);
                if (exists) throw new BadRequestException("Slug already exists");

                string imagePath;

                if (model.Thumbnail != null)
                {
                    var uploadResult = await _fileService.UploadAsync(
                        model.Thumbnail,
                        "course_thumbnails",
                        FileConstants.Image.AllowedExtensions,
                        FileConstants.Image.MaxSize);

                    uploadedPath = uploadResult.FilePath;
                    imagePath = uploadedPath;
                }
                else
                {
                    imagePath = FileConstants.Defaults.CourseThumbnailImage; // 👈 default image
                }

                // Save DB
                var entity = new Course
                {
                    Title = model.Title,
                    Slug = string.IsNullOrWhiteSpace(model.Slug) ? SlugHelper.GenerateSlug(model.Title) : model.Slug,
                    Subtitle = model.Subtitle,
                    ShortDescription = model.ShortDescription,
                    LongDescription = model.LongDescription,

                    CourseType = model.CourseType,
                    DifficultyLevel = model.DifficultyLevel,
                    CourseLanguageId = model.CourseLanguageId,
                    CourseStatus = model.CourseStatus,

                    IsSequentialAccess = model.IsSequentialAccess ? true : false,
                    Thumbnail = imagePath
                };

                ApplyPricing(entity, model.Price, model.FinalPrice);

                // Categories
                entity.CourseCategories = model.CategoryIds
                    .Select(x => new CourseCategory { CategoryId = x })
                    .ToList();

                // Child collections
                entity.CourseRequirements = model.CourseRequirements
                    .Select(x => new CourseRequirement { Value = x }).ToList();

                entity.CourseTargetAudiences = model.CourseTargetAudiences
                    .Select(x => new CourseTargetAudience { Value = x }).ToList();

                entity.CourseLearningOutcomes = model.CourseLearningOutcomes
                    .Select(x => new CourseLearningOutcome { Value = x }).ToList();

                _context.Courses.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uploadedPath))
                {
                    await _fileService.DeleteAsync(uploadedPath);
                }

                ModelState.AddModelError("", ex.Message);
                LoadDropdowns();
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var entity = await _context.Courses
                .Include(x => x.CourseCategories).ThenInclude(x => x.Category)
                .Include(x => x.CourseRequirements)
                .Include(x => x.CourseTargetAudiences)
                .Include(x => x.CourseLearningOutcomes)
                .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Course Not Found");

                var model = new UpdateCourseViewModel
                {
                    Title = entity.Title,
                    Slug = entity.Slug,
                    Subtitle = entity.Subtitle,
                    ShortDescription = entity.ShortDescription,
                    LongDescription = entity.LongDescription,
                    CourseLanguageId = entity.CourseLanguageId,
                    DifficultyLevel = entity.DifficultyLevel,
                    CourseType = entity.CourseType,
                    Price = entity.Price,
                    FinalPrice = entity.FinalPrice,
                    CourseStatus = entity.CourseStatus,
                    CategoryIds = entity.CourseCategories.Select(x => x.CategoryId).ToList(),
                    CourseLearningOutcomes = entity.CourseLearningOutcomes.Select(x => x.Value).ToList(),
                    CourseRequirements = entity.CourseRequirements.Select(x => x.Value).ToList(),
                    CourseTargetAudiences = entity.CourseTargetAudiences.Select(x => x.Value).ToList(),
                    ExistingThumbnail = entity.Thumbnail,
                    IsSequentialAccess = entity.IsSequentialAccess,
                };

                // Ensure at least 1 input
                if (!model.CourseLearningOutcomes.Any()) model.CourseLearningOutcomes.Add("");
                if (!model.CourseRequirements.Any()) model.CourseRequirements.Add("");
                if (!model.CourseTargetAudiences.Any()) model.CourseTargetAudiences.Add("");

                ViewBag.CourseId = entity.Id;

                LoadDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong while loading Course. - " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateCourseViewModel model)
        {
            string? newImagePath = null;

            try
            {
                var entity = await _context.Courses
                    .Include(x => x.CourseCategories)
                    .Include(x => x.CourseRequirements)
                    .Include(x => x.CourseTargetAudiences)
                    .Include(x => x.CourseLearningOutcomes)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Courses Not Found");

                ValidateCollection(model);

                var text = Regex.Replace(model.LongDescription, "<.*?>", "").Trim();
                if (string.IsNullOrEmpty(text))
                {
                    ModelState.AddModelError("LongDescription", "Long Description is required");
                }

                // Reload UI data if invalid
                if (!ModelState.IsValid)
                {
                    model.ExistingThumbnail = entity.Thumbnail;
                    ViewBag.CourseId = id;
                    LoadDropdowns();
                    return View(model);
                }

                // Validate Course Title
                var exists = await _context.Courses.IgnoreQueryFilters().AnyAsync(x => x.Title == model.Title && x.Id != id);
                if (exists) throw new BadRequestException("Course already exists");

                // Validate Course Slug
                exists = await _context.Courses.IgnoreQueryFilters().AnyAsync(x => x.Slug == model.Slug && x.Id != id);
                if (exists) throw new BadRequestException("Slug already exists");


                // IMAGE REPLACEMENT (optional)
                if (model.Thumbnail != null)
                {
                    var uploadResult = await _fileService.UploadAsync(
                        model.Thumbnail,
                        "course_thumbnails",
                        FileConstants.Image.AllowedExtensions,
                        FileConstants.Image.MaxSize
                    );

                    newImagePath = uploadResult.FilePath;

                    // delete old image
                    if (!string.IsNullOrEmpty(entity.Thumbnail))
                    {
                        await _fileService.DeleteAsync(entity.Thumbnail);
                    }

                    entity.Thumbnail = newImagePath;
                }

                // update fields
                entity.Title = model.Title;
                entity.Slug = string.IsNullOrWhiteSpace(model.Slug)
                    ? SlugHelper.GenerateSlug(model.Title)
                    : model.Slug;

                entity.Subtitle = model.Subtitle;
                entity.ShortDescription = model.ShortDescription;
                entity.LongDescription = model.LongDescription;

                entity.CourseType = model.CourseType;
                entity.DifficultyLevel = model.DifficultyLevel;
                entity.CourseLanguageId = model.CourseLanguageId;
                entity.CourseStatus = model.CourseStatus;

                entity.IsSequentialAccess = model.IsSequentialAccess;

                ApplyPricing(entity, model.Price, model.FinalPrice);

                // Replace Categories
                _context.CourseCategories.RemoveRange(entity.CourseCategories);
                entity.CourseCategories = model.CategoryIds.Distinct()
                    .Select(x => new CourseCategory { CourseId = entity.Id, CategoryId = x })
                    .ToList();

                // Replace child collections
                await ReplaceChildCollectionAsync(_context.CourseRequirements, entity.Id, model.CourseRequirements, (v, id) => new CourseRequirement { Value = v, CourseId = id });
                await ReplaceChildCollectionAsync(_context.CourseTargetAudiences, entity.Id, model.CourseTargetAudiences, (v, id) => new CourseTargetAudience { Value = v, CourseId = id });
                await ReplaceChildCollectionAsync(_context.CourseLearningOutcomes, entity.Id, model.CourseLearningOutcomes, (v, id) => new CourseLearningOutcome { Value = v, CourseId = id });

                entity.UpdatedAt = DateTime.UtcNow;

                _context.Courses.Update(entity);
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
                var existing = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);
                if (existing != null)
                {
                    model.ExistingThumbnail = existing.Thumbnail;
                }
                ViewBag.CourseId = id;
                LoadDropdowns();
                return View(model);
            }
        }


        #region Private Methods

        private void LoadDropdowns()
        {
            ViewBag.CourseTypes = Enum.GetValues(typeof(CourseType))
                .Cast<CourseType>()
                .Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToString()
                }).ToList();

            ViewBag.DifficultyLevels = Enum.GetValues(typeof(DifficultyLevel))
                .Cast<DifficultyLevel>()
                .Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToString()
                }).ToList();

            ViewBag.CourseLanguages = _context.CourseLanguages
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CourseLanguageName
                }).ToList();

            ViewBag.CourseStatuses = Enum.GetValues(typeof(CourseStatus))
                .Cast<CourseStatus>()
                .Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToString()
                }).ToList();

            ViewBag.Categories = _context.Categories
                .OrderBy(x => x.DisplayName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.DisplayName
                }).ToList();
        }

        private void ApplyPricing(Course course, decimal price, decimal finalPrice)
        {
            if (course.CourseType == CourseType.Free)
            {
                course.Price = 0;
                course.FinalPrice = 0;
                course.DiscountPercentage = 0;
                return;
            }

            course.Price = price;
            course.FinalPrice = finalPrice;

            if (price > 0)
                course.DiscountPercentage = ((price - finalPrice) / price) * 100;
        }

        private async Task ReplaceChildCollectionAsync<TEntity>(DbSet<TEntity> dbSet, Guid courseId, List<string> values, Func<string, Guid, TEntity> factory) where TEntity : BaseEntity
        {
            // 1. DELETE directly from DB (no tracking issues)
            var existingItems = await dbSet
                .Where(x => EF.Property<Guid>(x, "CourseId") == courseId)
                .ToListAsync();

            if (existingItems.Any())
            {
                dbSet.RemoveRange(existingItems);
            }

            // 2. ADD new items
            if (values != null && values.Any())
            {
                var newItems = values.Select(v => factory(v, courseId)).ToList();
                await dbSet.AddRangeAsync(newItems);
            }
        }

        private void ValidateCollection(CreateCourseViewModel model)
        {
            model.CourseLearningOutcomes = CleanList(model.CourseLearningOutcomes, "CourseLearningOutcomes", "At least one learning outcome is required");
            model.CourseRequirements = CleanList(model.CourseRequirements, "CourseRequirements", "At least one requirement is required");
            model.CourseTargetAudiences = CleanList(model.CourseTargetAudiences, "CourseTargetAudiences", "At least one target audience is required");
        }

        private void ValidateCollection(UpdateCourseViewModel model)
        {
            model.CourseLearningOutcomes = CleanList(model.CourseLearningOutcomes, "CourseLearningOutcomes", "At least one learning outcome is required");
            model.CourseRequirements = CleanList(model.CourseRequirements, "CourseRequirements", "At least one requirement is required");
            model.CourseTargetAudiences = CleanList(model.CourseTargetAudiences, "CourseTargetAudiences", "At least one target audience is required");
        }

        private List<string> CleanList(List<string>? list, string fieldName, string errorMessage)
        {
            var cleaned = list?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList() ?? new List<string>();

            if (!cleaned.Any())
            {
                ModelState.AddModelError(fieldName, errorMessage);
            }

            return cleaned;
        }

        #endregion
    }
}
