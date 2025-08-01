using IAMUAYTHAI.Application.Abstractions.DTOs.Auth;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(int userId);
    }
}   