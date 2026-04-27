using LMS.Domain.Entities;

namespace LMS.StudentAPI.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
