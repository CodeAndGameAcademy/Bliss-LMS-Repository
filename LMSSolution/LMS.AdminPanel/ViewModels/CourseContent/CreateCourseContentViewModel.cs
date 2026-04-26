using LMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels.CourseContent
{
    public class CreateCourseContentViewModel
    {
        public Guid CourseId { get; set; }

        public Guid CourseModuleId { get; set; }

        [Required(ErrorMessage = "Content title is required")]
        [MaxLength(200, ErrorMessage = "Content title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order index is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Order index must be greater than 0")]
        public int OrderIndex { get; set; }

        [Required(ErrorMessage = "Content type is required")]
        public CourseContentType CourseContentType { get; set; }

        [Url(ErrorMessage = "Invalid YouTube URL")]
        public string? YoutubeVideoURL { get; set; }

        public IFormFile? ContentFile { get; set; }


        [Required(ErrorMessage = "Content length is required")]
        [Range(1, 10000, ErrorMessage = "Content length must be between 1 and 10000 minutes")]
        public int ContentLengthInMinutes { get; set; }

        public bool IsFreePreview { get; set; } = false;
    }
}
