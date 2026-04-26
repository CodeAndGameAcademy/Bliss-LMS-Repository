namespace LMS.AdminPanel.ViewModels.CourseContent
{
    public class CourseContentPageViewModel
    {
        public CreateCourseContentViewModel CreateCourseContent { get; set; } = new();

        public List<Domain.Entities.CourseContent> CourseContents { get; set; } = new();
    }
}