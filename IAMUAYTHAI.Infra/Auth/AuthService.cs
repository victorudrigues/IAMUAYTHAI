using IAMUAYTHAI.Application.Abstractions.DTOs.Auth;
using IAMUAYTHAI.Application.Abstractions.Features.Auth;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;

namespace IAMUAYTHAI.Infra.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // Busca usuário por email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return null;

            // Verifica senha
            if (!_passwordHashService.VerifyPassword(request.Password, user.PasswordHash))
                return null;

            // Gera tokens
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new UserInfo
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Profile = user.Profile.ToString()
                }
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Verifica senha atual
            if (!_passwordHashService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                return false;

            // Atualiza senha
            user.PasswordHash = _passwordHashService.HashPassword(request.NewPassword);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
        {
            // TODO: Implementar lógica de refresh token
            await Task.CompletedTask;
            return null;
        }

        public async Task LogoutAsync(int userId)
        {
            // TODO: Implementar blacklist de tokens
            await Task.CompletedTask;
        }
    }
}