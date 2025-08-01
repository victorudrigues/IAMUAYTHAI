using UserDomain = IAMUAYTHAI.Domain.Aggregates.UserAggregate.User;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserDomain user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        int GetUserIdFromToken(string token);
    }
}