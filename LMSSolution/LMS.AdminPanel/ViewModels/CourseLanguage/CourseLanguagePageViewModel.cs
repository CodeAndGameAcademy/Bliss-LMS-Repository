using LMS.AdminPanel.ViewModels.Slider;

namespace LMS.AdminPanel.ViewModels.CourseLanguage
{
    public class CourseLanguagePageViewModel
    {
        public CreateCourseLanguageViewModel CreateLanguage { get; set; } = new();
        public List<Domain.Entities.CourseLanguage> CourseLanguages { get; set; } = new();
    }
}
