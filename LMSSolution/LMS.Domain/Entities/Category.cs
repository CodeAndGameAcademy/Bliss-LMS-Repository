using LMS.Domain.Common;

namespace LMS.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public int OrderIndex { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
        public string Image { get; set; } = "uploads/default/category.png";

        // Self Reference
        public Guid? ParentId { get; set; }

        public Category? Parent { get; set; }

        public ICollection<Category> Children { get; set; } = new List<Category>();

        // Many-to-Many with Course
        public ICollection<CourseCategory> CourseCategories { get; set; } = new List<CourseCategory>();
    }
}
