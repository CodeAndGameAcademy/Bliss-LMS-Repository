using LMS.Infrastructure.Data;
using LMS.StudentAPI.DTOs.Category;
using LMS.StudentAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.StudentAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL (Active Only)       
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(x => x.ParentId == null)
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                    Image = x.BaseUrl + "/" + x.Image,
                    ParentId = x.ParentId
                })
                .ToListAsync();
        }

        public async Task<List<CategoryDto>> GetAllSubCategoriesAsync(Guid ParentId)
        {
            return await _context.Categories
               .Where(x => x.ParentId == ParentId)
               .Select(x => new CategoryDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   DisplayName = x.DisplayName,
                   Image = x.BaseUrl + "/" + x.Image,
                   ParentId = x.ParentId
               })
               .ToListAsync();
        }
    }
}
