using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class Instructor : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? Degree { get; set; }

        public string About { get; set; } = string.Empty;

        public string? CertificationSkill { get; set; }

        public string Image { get; set; } = string.Empty;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
