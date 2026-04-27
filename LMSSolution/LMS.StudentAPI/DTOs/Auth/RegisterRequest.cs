using System.ComponentModel.DataAnnotations;

namespace LMS.StudentAPI.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(150, ErrorMessage = "Full name cannot exceed 150 characters")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]+$", ErrorMessage = "Password must contain at least one letter and one number")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "DeviceId is required")]
        [MaxLength(100, ErrorMessage = "DeviceId cannot exceed 100 characters")]
        public string? DeviceId { get; set; } = null!;

        [Required(ErrorMessage = "DeviceInfo is required")]
        [MaxLength(255, ErrorMessage = "DeviceInfo cannot exceed 255 characters")]
        public string? DeviceInfo { get; set; } = null!;
    }
}
