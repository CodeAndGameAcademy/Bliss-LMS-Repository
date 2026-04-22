using System.ComponentModel.DataAnnotations;

namespace LMS.AdminPanel.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number format")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]+$", ErrorMessage = "Password must contain at least one letter and one number")]
        public string Password { get; set; } = string.Empty;
    }
}
