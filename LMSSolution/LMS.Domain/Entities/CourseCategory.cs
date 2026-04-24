namespace LMS.Domain.Entities
{
    public class CourseCategory
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
