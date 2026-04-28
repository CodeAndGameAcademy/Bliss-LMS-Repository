using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class Wishlist : BaseEntity
    {
        // Required FKs
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }

        // Navigation (Unidirectional)
        public User User { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
