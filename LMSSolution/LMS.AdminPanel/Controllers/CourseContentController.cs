using LMS.AdminPanel.Common.Constants;
using LMS.AdminPanel.Exceptions;
using LMS.AdminPanel.Services;
using LMS.AdminPanel.ViewModels.CourseContent;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LMS.AdminPanel.Controllers
{
    public class CourseContentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public CourseContentController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public IActionResult Index(Guid? courseId, Guid? courseModuleId)
        {
            var model = new CourseContentPageViewModel();

            // Load courses
            ViewBag.Courses = _context.Courses
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title
                }).ToList();

            ViewBag.CourseContentTypes = Enum.GetValues(typeof(CourseContentType))
                .Cast<CourseContentType>()
                .Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToString()
                }).ToList();

            ViewBag.CourseModules = new List<SelectListItem>();

            // Filter modules

            if (courseId.HasValue)
            {
                ViewBag.CourseModules = _context.CourseModules
                    .IgnoreQueryFilters()
                    .Where(x => x.CourseId == courseId)
                    .OrderBy(x => x.OrderIndex)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    }).ToList();

                model.CreateCourseContent.CourseId = courseId.Value;
            }


            if (courseModuleId.HasValue)
            {
                model.CourseContents = _context.CourseContents
                    .IgnoreQueryFilters()
                    .Where(x => x.CourseModuleId == courseModuleId)
                    .OrderBy(x => x.OrderIndex)
                    .ToList();

                model.CreateCourseContent.CourseModuleId = courseModuleId.Value;
            }

            return View("CourseContent", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseContentPageViewModel model)
        {
            string? uploadedPath = null;

            try
            {
                // Load courses
                ViewBag.Courses = _context.Courses
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title
                    }).ToList();

                if (model.CreateCourseContent.CourseId != Guid.Empty)
                {
                    ViewBag.CourseModules = _context.CourseModules
                       .IgnoreQueryFilters()
                       .Where(x => x.CourseId == model.CreateCourseContent.CourseId)
                       .OrderBy(x => x.OrderIndex)
                       .Select(x => new SelectListItem
                       {
                           Value = x.Id.ToString(),
                           Text = x.Title
                       }).ToList();
                }
                else
                {
                    ViewBag.CourseModules = new List<SelectListItem>();
                }

                ViewBag.CourseContentTypes = Enum.GetValues(typeof(CourseContentType))
                    .Cast<CourseContentType>()
                    .Select(x => new SelectListItem
                    {
                        Value = x.ToString(),
                        Text = x.ToString()
                    }).ToList();



                if (model.CreateCourseContent.CourseId == Guid.Empty)
                {
                    ModelState.AddModelError(
                        "CreateCourseContent.CourseId",
                        "Please select a course"
                    );
                }

                if (model.CreateCourseContent.CourseModuleId == Guid.Empty)
                {
                    ModelState.AddModelError(
                        "CreateCourseContent.CourseModuleId",
                        "Please select a module"
                    );
                }

                if (!ModelState.IsValid)
                {
                    // Reload modules if validation fails
                    model.CourseContents = await _context.CourseContents
                        .IgnoreQueryFilters()
                        .Where(x => x.CourseModuleId == model.CreateCourseContent.CourseModuleId)
                        .OrderBy(x => x.OrderIndex)
                        .ToListAsync();

                    return View("CourseContent", model);
                }

                // Validate Order Index
                var exists = await _context.CourseContents
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.CourseModuleId == model.CreateCourseContent.CourseModuleId && x.OrderIndex == model.CreateCourseContent.OrderIndex);

                if (exists)
                    throw new BadRequestException("OrderIndex already exists in this module.");

                // Upload PDF 
                string? pdfPath = null;
                string? youtubeVideoUrl = null;

                if (model.CreateCourseContent.CourseContentType == CourseContentType.Pdf)
                {
                    // ❗ PDF Required
                    if (model.CreateCourseContent.ContentFile == null)
                        throw new BadRequestException("PDF file is required.");

                    // Get Course Info (for dynamic folder)                
                    var courseData = await _context.CourseModules
                        .Where(m => m.Id == model.CreateCourseContent.CourseModuleId)
                        .Select(m => new
                        {
                            m.CourseId,
                            CourseName = m.Course.Title
                        })
                        .FirstOrDefaultAsync();

                    if (courseData == null || string.IsNullOrWhiteSpace(courseData.CourseName))
                        throw new NotFoundException("Course not found.");

                    var safeCourseName = courseData.CourseName;
                    var folder = $"course-contents/{safeCourseName}-{courseData.CourseId}";

                    var upload = await _fileService.UploadAsync(
                        model.CreateCourseContent.ContentFile,
                        folder,
                        FileConstants.Document.AllowedExtensions,
                        FileConstants.Document.MaxSize
                    );

                    uploadedPath = upload.FilePath;
                    pdfPath = uploadedPath;
                }
                else if (model.CreateCourseContent.CourseContentType == CourseContentType.Video)
                {
                    // ❗ YouTube URL Required
                    if (string.IsNullOrWhiteSpace(model.CreateCourseContent.YoutubeVideoURL))
                        throw new BadRequestException("YouTube URL is required.");

                    youtubeVideoUrl = model.CreateCourseContent.YoutubeVideoURL;
                }
                else
                {
                    throw new BadRequestException("Unsupported content type.");
                }


                var entity = new CourseContent
                {
                    Id = Guid.NewGuid(),
                    CourseModuleId = model.CreateCourseContent.CourseModuleId,
                    Title = model.CreateCourseContent.Title,
                    CourseContentType = model.CreateCourseContent.CourseContentType,
                    OrderIndex = model.CreateCourseContent.OrderIndex,
                    YoutubeVideoURL = youtubeVideoUrl,
                    ContentFile = pdfPath,
                    ContentLengthInMinutes = model.CreateCourseContent.ContentLengthInMinutes,
                    IsFreePreview = model.CreateCourseContent.IsFreePreview,
                };

                _context.CourseContents.Add(entity);
                await _context.SaveChangesAsync();

                // Redirect with selected course
                return RedirectToAction("Index", new { courseId = model.CreateCourseContent.CourseId, courseModuleId = model.CreateCourseContent.CourseModuleId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Reload modules if validation fails
                model.CourseContents = await _context.CourseContents
                    .Where(x => x.CourseModuleId == model.CreateCourseContent.CourseModuleId)
                    .OrderBy(x => x.OrderIndex)
                    .ToListAsync();

                return View("CourseContent", model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid courseId, Guid courseModuleId)
        {
            try
            {
                var entity = await _context.CourseContents
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Course Content not found");

                entity.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { courseId, courseModuleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                // Redirect with selected course
                return RedirectToAction("Index", new { courseId = courseId, courseModuleId = courseModuleId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id, Guid courseId, Guid courseModuleId)
        {
            try
            {
                var entity = await _context.CourseContents
                     .IgnoreQueryFilters()
                     .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null);

                if (entity == null)
                    throw new NotFoundException("Course Content not found or not deleted");

                entity.DeletedAt = null;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { courseId, courseModuleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                // Redirect with selected course
                return RedirectToAction("Index", new { courseId = courseId, courseModuleId = courseModuleId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, Guid courseId, Guid courseModuleId)
        {
            try
            {
                ViewBag.CourseContentTypes = Enum.GetValues(typeof(CourseContentType))
                   .Cast<CourseContentType>()
                   .Select(x => new SelectListItem
                   {
                       Value = x.ToString(),
                       Text = x.ToString()
                   }).ToList();

                var entity = await _context.CourseContents
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Course Content not found");

                var model = new UpdateCourseContentViewModel
                {
                    CourseId = courseId,
                    CourseModuleId = courseModuleId,
                    Title = entity.Title,
                    CourseContentType = entity.CourseContentType,
                    OrderIndex = entity.OrderIndex,
                    YoutubeVideoURL = entity.YoutubeVideoURL,
                    ExistingContentFile = entity.ContentFile,
                    ContentLengthInMinutes = entity.ContentLengthInMinutes,
                    IsFreePreview = entity.IsFreePreview,
                };

                ViewBag.CourseContentId = entity.Id;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                // Redirect with selected course
                return RedirectToAction("Index", new { courseId = courseId, courseModuleId = courseModuleId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateCourseContentViewModel model)
        {
            string? newUploadedPath = null;

            try
            {
                // 1. Get existing content
                var entity = await _context.CourseContents
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Content not found.");

                // 2. Validate OrderIndex (exclude current record)                
                var exists = await _context.CourseContents
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.CourseModuleId == entity.CourseModuleId && x.OrderIndex == model.OrderIndex && x.Id != id);

                if (exists)
                    throw new BadRequestException("OrderIndex already exists.");


                if (!ModelState.IsValid)
                {
                    ViewBag.CourseContentTypes = Enum.GetValues(typeof(CourseContentType))
                    .Cast<CourseContentType>()
                    .Select(x => new SelectListItem
                    {
                        Value = x.ToString(),
                        Text = x.ToString()
                    }).ToList();

                    ViewBag.CourseContentId = entity.Id;

                    return View("Edit", model);
                }

                string? oldContentfilePath = entity.ContentFile;
                string? youtubeVideoUrl = entity.YoutubeVideoURL;

                if (model.CourseContentType == CourseContentType.Pdf)
                {
                    // ❗ Must have file
                    if (model.ContentFile == null && string.IsNullOrEmpty(entity.ContentFile))
                        throw new BadRequestException("PDF file is required.");

                    // Replace if new file uploaded
                    if (model.ContentFile != null)
                    {
                        // Get Course Info for Dynamic Folder                
                        var courseData = await _context.CourseModules
                            .Where(m => m.Id == entity.CourseModuleId)
                            .Select(m => new
                            {
                                m.CourseId,
                                CourseName = m.Course.Title
                            })
                            .FirstOrDefaultAsync();

                        if (courseData == null || string.IsNullOrWhiteSpace(courseData.CourseName))
                            throw new NotFoundException("Course not found.");

                        var safeCourseName = courseData.CourseName;
                        var folder = $"course-contents/{safeCourseName}-{courseData.CourseId}";

                        var upload = await _fileService.UploadAsync(
                            model.ContentFile,
                            folder,
                            FileConstants.Document.AllowedExtensions,
                            FileConstants.Document.MaxSize
                        );

                        newUploadedPath = upload.FilePath;
                        entity.ContentFile = newUploadedPath;

                        if (model.ContentFile != null && !string.IsNullOrEmpty(oldContentfilePath))
                        {
                            await _fileService.DeleteAsync(oldContentfilePath);
                        }
                    }

                    // Remove YouTube URL if switching
                    youtubeVideoUrl = null;
                }
                else if (model.CourseContentType == CourseContentType.Video)
                {
                    // ❗ Must have YouTube URL
                    if (string.IsNullOrWhiteSpace(model.YoutubeVideoURL))
                        throw new BadRequestException("YouTube URL is required.");

                    youtubeVideoUrl = model.YoutubeVideoURL;

                    // If switching from PDF → delete old file
                    if (!string.IsNullOrEmpty(entity.ContentFile))
                    {
                        await _fileService.DeleteAsync(entity.ContentFile);
                        entity.ContentFile = null;
                    }
                }
                else
                {
                    throw new BadRequestException("Unsupported content type.");
                }

                entity.Title = model.Title;
                entity.OrderIndex = model.OrderIndex;
                entity.CourseContentType = model.CourseContentType;
                entity.YoutubeVideoURL = youtubeVideoUrl;
                entity.ContentLengthInMinutes = model.ContentLengthInMinutes;
                entity.IsFreePreview = model.IsFreePreview;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { courseId = model.CourseId, courseModuleId = model.CourseModuleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", new { courseId = model.CourseId, courseModuleId = model.CourseModuleId });
            }
        }
    }
}
