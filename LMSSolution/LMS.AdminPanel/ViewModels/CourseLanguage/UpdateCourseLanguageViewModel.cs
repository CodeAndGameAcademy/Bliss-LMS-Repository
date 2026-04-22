using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels.CourseLanguage
{
    public class UpdateCourseLanguageViewModel
    {
        [Required(ErrorMessage = "Course language name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string CourseLanguageName { get; set; } = string.Empty;
    }
}
