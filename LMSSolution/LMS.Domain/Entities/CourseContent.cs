using LMS.Domain.Common;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    public class CourseContent : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public CourseContentType CourseContentType { get; set; }

        public string? YoutubeVideoURL { get; set; }
        public string? ContentFile { get; set; }

        public int ContentLengthInMinutes { get; set; }

        public bool IsFreePreview { get; set; } = false;

        // FK
        public Guid CourseModuleId { get; set; }
        public CourseModule CourseModule { get; set; } = null!;
    }
}
