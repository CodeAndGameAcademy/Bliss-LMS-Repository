using LMS.Domain.Enums;

namespace LMS.StudentAPI.DTOs.CourseContent
{
    public class CourseContentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public CourseContentType CourseContentType { get; set; }
        public string? YoutubeVideoURL { get; set; }
        public string? ContentFile { get; set; }
        public int ContentLengthInMinutes { get; set; }

        public bool IsFreePreview { get; set; }
    }
}
