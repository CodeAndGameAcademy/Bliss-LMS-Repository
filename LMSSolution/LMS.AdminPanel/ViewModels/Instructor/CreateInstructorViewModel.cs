using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels.Instructor
{
    public class CreateInstructorViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display name is required")]
        [MaxLength(150, ErrorMessage = "Display name cannot exceed 150 characters")]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Degree cannot exceed 200 characters")]
        public string? Degree { get; set; }

        [Required(ErrorMessage = "About is required")]
        [MaxLength(2000, ErrorMessage = "About cannot exceed 2000 characters")]
        public string About { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Certification/Skill cannot exceed 500 characters")]
        public string? CertificationSkill { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
