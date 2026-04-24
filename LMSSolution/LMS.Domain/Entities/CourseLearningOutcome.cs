using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class CourseLearningOutcome : BaseEntity
    {
        public string Value { get; set; } = string.Empty;

        // FK
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
