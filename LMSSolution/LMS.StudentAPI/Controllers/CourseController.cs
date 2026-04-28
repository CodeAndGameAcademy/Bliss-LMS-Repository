using LMS.StudentAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.StudentAPI.Controllers
{
    [Route("api/v1/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("free")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetAllFreeCourses()
        {
            var result = await _courseService.GetAllFreeCoursesAsync();
            return Ok(result);
        }

        [HttpGet("paid")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetAllPaidCourses()
        {
            var result = await _courseService.GetAllPaidCoursesAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _courseService.GetByIdAsync(id);
            return Ok(result);
        }
    }
}
