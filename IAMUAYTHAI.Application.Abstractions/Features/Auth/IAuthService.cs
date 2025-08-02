using IAMUAYTHAI.Application.Abstractions.Features.Auth.Request;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request);
        Task LogoutAsync(int userId);

        // Metodos de segurança
        Task<LoginResponse?> SecureLoginAsync(SecureLoginRequest request);
        Task<bool> SecureChangePasswordAsync(int userId, SecureChangePasswordRequest request);
    }
}