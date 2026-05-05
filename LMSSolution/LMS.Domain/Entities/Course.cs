using LMS.Domain.Common;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    public class Course : BaseEntity
    {
        // Basic Info
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;


        // Pricing
        public CourseType CourseType { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }


        // Media
        public string BaseUrl { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = "uploads/default/course_thumbnail.png";


        // Metadata        
        public DifficultyLevel DifficultyLevel { get; set; }

        public Guid CourseLanguageId { get; set; }
        public CourseLanguage CourseLanguage { get; set; } = null!;

        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;

        public CourseStatus CourseStatus { get; set; }

        public bool IsSequentialAccess { get; set; } = true;


        // Navigation Properties        
        public ICollection<CourseLearningOutcome> CourseLearningOutcomes { get; set; } = new List<CourseLearningOutcome>();
        public ICollection<CourseRequirement> CourseRequirements { get; set; } = new List<CourseRequirement>();
        public ICollection<CourseTargetAudience> CourseTargetAudiences { get; set; } = new List<CourseTargetAudience>();
        public ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();
        public ICollection<CourseCategory> CourseCategories { get; set; } = new List<CourseCategory>();
    }
}
