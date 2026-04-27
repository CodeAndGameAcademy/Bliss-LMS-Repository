using LMS.StudentAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.StudentAPI.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET ALL
        [HttpGet("category")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        // GET All SubCategories
        [HttpGet("subcategory/{parentId}")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetAllSubCategories(Guid parentId)
        {
            var result = await _categoryService.GetAllSubCategoriesAsync(parentId);
            return Ok(result);
        }
    }
}
