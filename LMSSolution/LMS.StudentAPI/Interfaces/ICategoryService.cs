using LMS.StudentAPI.DTOs.Category;

namespace LMS.StudentAPI.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<List<CategoryDto>> GetAllSubCategoriesAsync(Guid ParentId);       
    }
}
