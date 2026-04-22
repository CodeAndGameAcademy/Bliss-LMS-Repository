using LMS.Domain.Common;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public Role Role { get; set; }

        public string? Image { get; set; }

        // Device Restriction
        public string? PrimaryDeviceId { get; set; }
        public string? PrimaryDeviceInfo { get; set; }

        public string? SecondaryDeviceId { get; set; }
        public string? SecondaryDeviceInfo { get; set; }

        public bool IsActive { get; set; } = true;

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockoutEnd { get; set; }
    }
}