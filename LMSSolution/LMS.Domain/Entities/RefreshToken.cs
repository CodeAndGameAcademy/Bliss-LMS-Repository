namespace LMS.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string Token { get; set; } = null!;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public string DeviceId { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }
    }
}
