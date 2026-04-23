using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels.Category
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(160, ErrorMessage = "Slug cannot exceed 160 characters")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug must be lowercase and hyphen-separated")]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "OrderIndex is required")]
        [Range(1, int.MaxValue, ErrorMessage = "OrderIndex must be greater than 0")]
        public int OrderIndex { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        public Guid? ParentId { get; set; }
    }
}
