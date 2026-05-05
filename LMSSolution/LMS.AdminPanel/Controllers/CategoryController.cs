using LMS.AdminPanel.Common.Constants;
using LMS.AdminPanel.Exceptions;
using LMS.AdminPanel.Helpers;
using LMS.AdminPanel.Services;
using LMS.AdminPanel.ViewModels.Category;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LMS.AdminPanel.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public CategoryController(IFileService fileService, ApplicationDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Categories.OrderBy(x => x.DisplayName).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> DeletedListAsync()
        {
            var model = await _context.Categories
                        .IgnoreQueryFilters()
                        .OrderBy(x => x.DisplayName)
                        .Where(x => x.DeletedAt != null).ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var entity = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Category not found");

                // Check active children
                var hasActiveChildren = await _context.Categories
                    .AnyAsync(x => x.ParentId == id);

                if (hasActiveChildren)
                    throw new BadRequestException("Cannot delete category because it has active child categories");


                entity.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var entity = await _context.Categories
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt != null);

                if (entity == null)
                    throw new NotFoundException("Category not found or not deleted");

                if (entity.DeletedAt == null)
                    throw new BadRequestException("Category is already active");

                // Check active parent
                if (entity.ParentId != null)
                {
                    var hasDeactivatedParent = await _context.Categories.IgnoreQueryFilters()
                        .AnyAsync(x => x.Id == entity.ParentId && x.DeletedAt != null);

                    if (hasDeactivatedParent)
                    {
                        throw new BadRequestException("Cannot proceed because parent category is deactivated");
                    }
                }

                entity.DeletedAt = null;
                entity.UpdatedAt = DateTime.UtcNow;

                // await RestoreChildrenAsync(id);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> CreateAsync()
        {
            var categories = await _context.Categories
               .OrderBy(x => x.DisplayName)
               .Select(x => new CategoryDropdownViewModel
               {
                   Id = x.Id,
                   Name = x.Name,
                   DisplayName = x.DisplayName
               })
               .ToListAsync();

            ViewBag.ParentCategories = new SelectList(categories, "Id", "DisplayName");

            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            string? uploadedPath = null;

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // VALIDATION                
                await ValidateDuplicateAsync(model.Name, model.ParentId);

                if (model.ParentId != null)
                {
                    var parentExists = await _context.Categories
                        .AnyAsync(x => x.Id == model.ParentId);

                    if (!parentExists)
                        throw new BadRequestException("Invalid parent category");
                }

                var orderExists = await _context.Categories
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.ParentId == model.ParentId && x.OrderIndex == model.OrderIndex);

                if (orderExists)
                    throw new BadRequestException("OrderIndex already exists in this category level.");

                // SLUG
                var slug = string.IsNullOrWhiteSpace(model.Slug)
                    ? SlugHelper.GenerateSlug(model.Name)
                    : model.Slug;

                var slugExists = await _context.Categories
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.Slug == slug);

                if (slugExists)
                    throw new BadRequestException("Slug already exists. Please use a different slug.");

                // DISPLAY NAME
                var displayName = await GenerateDisplayNameAsync(model.Name, model.ParentId);

                // IMAGE
                string imagePath;
                string baseUrl = "";

                if (model.ImageFile != null)
                {
                    var uploadResult = await _fileService.UploadAsync(
                        model.ImageFile,
                        "categories",
                        FileConstants.Image.AllowedExtensions,
                        FileConstants.Image.MaxSize);

                    uploadedPath = uploadResult.FilePath;
                    
                    imagePath = uploadedPath;
                    baseUrl = uploadResult.BaseUrl;
                }
                else
                {
                    imagePath = FileConstants.Defaults.CategoryImage; // 👈 default image
                }

                var entity = new Category
                {
                    Name = model.Name,
                    Slug = slug,
                    OrderIndex = model.OrderIndex,
                    DisplayName = displayName,
                    Description = model.Description,
                    BaseUrl = baseUrl,
                    Image = imagePath,
                    ParentId = model.ParentId
                };

                _context.Categories.Add(entity);
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

                var categories = await _context.Categories
               .OrderBy(x => x.DisplayName)
               .Select(x => new CategoryDropdownViewModel
               {
                   Id = x.Id,
                   Name = x.Name,
                   DisplayName = x.DisplayName
               })
               .ToListAsync();

                ViewBag.ParentCategories = new SelectList(categories, "Id", "DisplayName");


                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var entity = await _context.Categories
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Category Not Found");

                var model = new UpdateCategoryViewModel
                {
                    Name = entity.Name,
                    Slug = entity.Slug,
                    OrderIndex = entity.OrderIndex,
                    ParentId = entity.ParentId,
                    Description = entity.Description,
                    ExistingImage = entity.Image
                };

                var categories = await _context.Categories
                   .OrderBy(x => x.DisplayName)
                   .Select(x => new CategoryDropdownViewModel
                   {
                       Id = x.Id,
                       Name = x.Name,
                       DisplayName = x.DisplayName
                   })
                   .ToListAsync();

                ViewBag.ParentCategories = new SelectList(categories, "Id", "DisplayName");
                ViewBag.CategoryId = entity.Id;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong while loading Instructor. - " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateCategoryViewModel model)
        {
            string? newImagePath = null;

            try
            {
                var entity = await _context.Categories
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new NotFoundException("Category Not Found");

                // Reload UI data if invalid
                if (!ModelState.IsValid)
                {
                    model.ExistingImage = entity.Image;
                    ViewBag.CategoryId = id;
                    return View(model);
                }

                // Parent validation
                if (model.ParentId != null)
                {
                    if (model.ParentId == id)
                        throw new BadRequestException("Category cannot be its own parent");

                    var parentExists = await _context.Categories
                        .AnyAsync(x => x.Id == model.ParentId);

                    if (!parentExists)
                        throw new BadRequestException("Invalid parent category");
                }

                // Name duplicate (FIXED)
                await ValidateDuplicateAsync(model.Name, model.ParentId, id);

                // OrderIndex validation (NEW)
                var orderExists = await _context.Categories
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.ParentId == model.ParentId && x.OrderIndex == model.OrderIndex && x.Id != id);

                if (orderExists)
                    throw new BadRequestException("OrderIndex already exists in this category level.");

                // SLUG                
                var slug = string.IsNullOrWhiteSpace(model.Slug)
                    ? SlugHelper.GenerateSlug(model.Name)
                    : model.Slug;

                var slugExists = await _context.Categories
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.Slug == slug && x.Id != id);

                if (slugExists)
                    throw new BadRequestException("Slug already exists. Please use a different slug.");

                // DISPLAY NAME                
                var displayName = await GenerateDisplayNameAsync(model.Name, model.ParentId);


                // IMAGE REPLACEMENT (optional)
                if (model.ImageFile != null)
                {
                    var uploadResult = await _fileService.UploadAsync(
                        model.ImageFile,
                        "categories",
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
                entity.Slug = slug;
                entity.OrderIndex = model.OrderIndex;
                entity.DisplayName = displayName;
                entity.Description = model.Description;
                entity.ParentId = model.ParentId;
                entity.UpdatedAt = DateTime.UtcNow;

                _context.Categories.Update(entity);
                await _context.SaveChangesAsync();

                // Update children display names
                await UpdateChildrenDisplayNamesAsync(entity.Id);

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
                var existing = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (existing != null)
                {
                    model.ExistingImage = existing.Image;
                }

                ViewBag.CategoryId = id;
                return View(model);
            }
        }

        #region Private Methods

        private async Task<string> GenerateDisplayNameAsync(string name, Guid? parentId)
        {
            if (parentId == null)
                return name;

            var parent = await _context.Categories
                .FirstOrDefaultAsync(x => x.Id == parentId);

            if (parent == null)
                throw new BadRequestException("Parent category not found");

            return $"{parent.DisplayName} >> {name}";
        }


        private async Task ValidateDuplicateAsync(string name, Guid? parentId, Guid? excludeId = null)
        {
            var exists = await _context.Categories
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Name == name && x.ParentId == parentId && (!excludeId.HasValue || x.Id != excludeId));

            if (exists)
                throw new BadRequestException("Category name already exists in this parent");
        }

        private async Task UpdateChildrenDisplayNamesAsync(Guid parentId)
        {
            // GlobalQueryFilter already excludes deleted
            var allCategories = await _context.Categories.ToListAsync();

            // Create lookup (children mapping)
            var lookup = allCategories.ToLookup(x => x.ParentId);

            // Create dictionary for O(1) parent access
            var categoryDict = allCategories.ToDictionary(x => x.Id);

            UpdateChildrenRecursive(parentId, lookup, categoryDict);

            await _context.SaveChangesAsync();
        }

        private void UpdateChildrenRecursive(Guid parentId, ILookup<Guid?, Category> lookup, Dictionary<Guid, Category> categoryDict)
        {
            if (!categoryDict.TryGetValue(parentId, out var parent))
                return;

            var children = lookup[parentId];

            foreach (var child in children)
            {
                child.DisplayName = $"{parent.DisplayName} >> {child.Name}";
                child.UpdatedAt = DateTime.UtcNow;

                UpdateChildrenRecursive(child.Id, lookup, categoryDict);
            }
        }

        //private async Task RestoreChildrenAsync(Guid parentId)
        //{
        //    var allCategories = await _context.Categories
        //        .IgnoreQueryFilters()
        //        .ToListAsync();

        //    var lookup = allCategories.ToLookup(x => x.ParentId);

        //    RestoreRecursive(parentId, lookup);
        //}

        //private void RestoreRecursive(Guid parentId, ILookup<Guid?, Category> lookup)
        //{
        //    var children = lookup[parentId];

        //    foreach (var child in children)
        //    {
        //        if (child.DeletedAt != null)
        //        {
        //            child.DeletedAt = null;
        //            child.UpdatedAt = DateTime.UtcNow;
        //        }

        //        RestoreRecursive(child.Id, lookup);
        //    }
        //}

        #endregion

    }
}
