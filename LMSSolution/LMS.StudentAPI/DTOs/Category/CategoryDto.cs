namespace LMS.StudentAPI.DTOs.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string Image { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
    }
}
