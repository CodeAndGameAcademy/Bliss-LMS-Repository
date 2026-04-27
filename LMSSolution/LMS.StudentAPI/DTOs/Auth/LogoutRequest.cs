using System.ComponentModel.DataAnnotations;

namespace LMS.StudentAPI.DTOs.Auth
{
    public class LogoutRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        [MinLength(20, ErrorMessage = "Invalid refresh token")]
        [MaxLength(500, ErrorMessage = "Invalid refresh token")]
        public string RefreshToken { get; set; } = null!;
    }
}
