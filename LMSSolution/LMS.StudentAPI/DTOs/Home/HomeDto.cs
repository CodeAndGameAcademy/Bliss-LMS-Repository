using LMS.StudentAPI.DTOs.Category;
using LMS.StudentAPI.DTOs.Course;
using LMS.StudentAPI.DTOs.Instructor;
using LMS.StudentAPI.DTOs.Slider;

namespace LMS.StudentAPI.DTOs.Home
{
    public class HomeDto
    {
        public List<SliderDto> Sliders { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<InstructorDto> Instructors { get; set; } = new();        
        public List<FreeCourseDto> FreeCourses { get; set; } = new();
        public List<PaidCourseDto> PaidCourses { get; set; } = new();
    }
}
