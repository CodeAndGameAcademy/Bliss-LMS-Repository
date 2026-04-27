using System.ComponentModel.DataAnnotations;

namespace LMS.StudentAPI.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        [MinLength(20, ErrorMessage = "Invalid refresh token")]
        [MaxLength(500, ErrorMessage = "Invalid refresh token")]
        public string RefreshToken { get; set; } = null!;

        [Required(ErrorMessage = "DeviceId is required")]
        [MaxLength(100, ErrorMessage = "DeviceId cannot exceed 100 characters")]
        public string DeviceId { get; set; } = null!;
    }
}
