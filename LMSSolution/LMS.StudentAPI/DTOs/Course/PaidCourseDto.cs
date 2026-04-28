using LMS.Domain.Enums;

namespace LMS.StudentAPI.DTOs.Course
{
    public class PaidCourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DifficultyLevel DifficultyLevel { get; set; }

        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiscountPercentage { get; set; }

        public string InstructorName { get; set; } = string.Empty;
    }
}
