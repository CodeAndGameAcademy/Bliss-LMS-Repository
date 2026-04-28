using LMS.Domain.Enums;
using LMS.StudentAPI.DTOs.CourseModule;

namespace LMS.StudentAPI.DTOs.Course
{
    public class CourseDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public DifficultyLevel DifficultyLevel { get; set; }
        public string CourseLanguage { get; set; } = string.Empty;
        public CourseType CourseType { get; set; }
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public List<string> CourseLanguageOutcomes { get; set; } = new();
        public List<string> CourseRequirements { get; set; } = new();
        public List<string> CourseTargetAudiences { get; set; } = new();
        public List<CourseModuleDto> Modules { get; set; } = new();
    }
}
