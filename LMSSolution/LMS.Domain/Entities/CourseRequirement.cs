using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class CourseRequirement : BaseEntity
    {
        public string Value { get; set; } = string.Empty;

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
