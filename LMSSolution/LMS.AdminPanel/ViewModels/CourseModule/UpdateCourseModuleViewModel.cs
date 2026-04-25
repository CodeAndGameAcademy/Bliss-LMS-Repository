using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels.CourseModule
{
    public class UpdateCourseModuleViewModel
    {
        [Required(ErrorMessage = "Module title is required")]
        [MaxLength(200, ErrorMessage = "Module title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order index is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Order index must be greater than 0")]
        public int OrderIndex { get; set; }

        public Guid CourseId { get; set; }
    }
}
