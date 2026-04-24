using LMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels.Course
{
    public class UpdateCourseViewModel
    {
        // Basic Info
        [Required(ErrorMessage = "Course title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug is required")]
        [MaxLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Subtitle is required")]
        [MaxLength(300, ErrorMessage = "Subtitle cannot exceed 300 characters")]
        public string Subtitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Short description is required")]
        [MaxLength(1000, ErrorMessage = "Short description cannot exceed 1000 characters")]
        public string ShortDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Long description is required")]
        public string LongDescription { get; set; } = string.Empty;

        // Pricing

        [Required(ErrorMessage = "Course type is required")]
        public CourseType CourseType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Final price must be greater than or equal to 0")]
        public decimal FinalPrice { get; set; }


        // Media
        public IFormFile? Thumbnail { get; set; }
        public string? ExistingThumbnail { get; set; }


        // Metadata

        [Required(ErrorMessage = "Difficulty level is required")]
        public DifficultyLevel DifficultyLevel { get; set; }

        [Required(ErrorMessage = "Course language is required")]
        public Guid CourseLanguageId { get; set; }

        [Required(ErrorMessage = "Course status is required")]
        public CourseStatus CourseStatus { get; set; }

        public bool IsSequentialAccess { get; set; } = true;


        // Multi Values

        [MinLength(1,ErrorMessage = "At least one learning outcome is required")]
        public List<string> CourseLearningOutcomes { get; set; } = new();

        [MinLength(1,ErrorMessage = "At least one requirement is required")]
        public List<string> CourseRequirements { get; set; } = new();

        [MinLength(1,ErrorMessage = "At least one target audience is required")]
        public List<string> CourseTargetAudiences { get; set; } = new();


        // Categories
        [MinLength(1, ErrorMessage = "At least one category is required")]
        public List<Guid> CategoryIds { get; set; } = new();
    }
}
