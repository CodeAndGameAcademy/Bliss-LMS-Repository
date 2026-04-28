using LMS.StudentAPI.DTOs.Course;

namespace LMS.StudentAPI.Interfaces
{
    public interface ICourseService
    {
        Task<List<FreeCourseDto>> GetAllFreeCoursesAsync();
        Task<List<PaidCourseDto>> GetAllPaidCoursesAsync();
        Task<CourseDetailDto> GetByIdAsync(Guid id);
    }
}
