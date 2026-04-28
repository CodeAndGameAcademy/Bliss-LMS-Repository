using LMS.StudentAPI.DTOs.CourseContent;

namespace LMS.StudentAPI.DTOs.CourseModule
{
    public class CourseModuleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public List<CourseContentDto> Contents { get; set; } = new();
    }
}
