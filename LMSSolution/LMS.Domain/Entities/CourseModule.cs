using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class CourseModule : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        // FK
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        // Navigation        
        public ICollection<CourseContent> CourseContents { get; set; } = new List<CourseContent>();
    }
}
