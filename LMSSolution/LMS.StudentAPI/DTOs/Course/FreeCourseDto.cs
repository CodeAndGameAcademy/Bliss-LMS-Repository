using LMS.Domain.Enums;

namespace LMS.StudentAPI.DTOs.Course
{
    public class FreeCourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DifficultyLevel DifficultyLevel { get; set; }
        public string InstructorName { get; set; } = string.Empty;
    }
}
