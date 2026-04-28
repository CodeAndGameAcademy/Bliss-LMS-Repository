using LMS.StudentAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace LMS.StudentAPI.DTOs.WishList
{
    public class CreateWishlistDto
    {
        [Required(ErrorMessage = "CourseId is required")]
        [NotEmptyGuid(ErrorMessage = "CourseId cannot be empty")]
        public Guid CourseId { get; set; }
    }
}
