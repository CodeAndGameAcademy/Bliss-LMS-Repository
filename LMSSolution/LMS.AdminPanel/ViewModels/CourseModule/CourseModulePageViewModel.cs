namespace LMS.AdminPanel.ViewModels.CourseModule
{
    public class CourseModulePageViewModel
    {
        public CreateCourseModuleViewModel CreateCourseModule { get; set; } = new();
        public List<Domain.Entities.CourseModule> CourseModules { get; set; } = new();
    }
}