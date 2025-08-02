using IAMUAYTHAI.Application.Abstractions.Features.Auth;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace IAMUAYTHAI.Infra.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IJwtService _jwtService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        private readonly Lazy<string> _dummyHash;
        
        // Timeout para operações de hash para evitar DoS
        private static readonly TimeSpan HashTimeout = TimeSpan.FromSeconds(5);

        public AuthService(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            IJwtService jwtService,
            ITokenBlacklistService tokenBlacklistService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _jwtService = jwtService;
            _tokenBlacklistService = tokenBlacklistService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            
            _dummyHash = new Lazy<string>(() => GenerateDummyHash());
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            using var secureRequest = new SecureLoginRequest { Email = request.Email };
            secureRequest.SetPassword(request.Password.AsSpan());

            // Delega para o método seguro
            return await SecureLoginAsync(secureRequest);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            using var secureRequest = new SecureChangePasswordRequest();
            secureRequest.SetCurrentPassword(request.CurrentPassword.AsSpan());
            secureRequest.SetNewPassword(request.NewPassword.AsSpan());
            secureRequest.SetConfirmPassword(request.ConfirmPassword.AsSpan());
            
            return await SecureChangePasswordAsync(userId, secureRequest);
        }

        #region (Memory<char>)

        /// <summary>
        /// Método seguro para login usando LoginRequest com Memory<char>
        /// </summary>
        public async Task<LoginResponse?> SecureLoginAsync(SecureLoginRequest request)
        {
            using (request)
            {
                if (request == null)
                {
                    _logger.LogWarning("Tentativa de login com request nulo");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(request.Email) || request.GetPassword().IsEmpty)
                {
                    _logger.LogWarning("Tentativa de login com email ou senha vazios");
                    await SimulateConstantTimeAsync();
                    return null;
                }

                var normalizedEmail = NormalizeEmail(request.Email);

                try
                {
                    var user = await _userRepository.GetByEmailAsync(normalizedEmail);

                    // Usa método síncrono para verificação com ReadOnlySpan
                    var (isPasswordValid, isUserValid) = VerifyCredentialsSecure(request.GetPassword(), user);

                    if (!isPasswordValid || !isUserValid)
                    {
                        var hashedEmail = HashForLogging(normalizedEmail);
                        _logger.LogWarning("Tentativa de login falhada. Email hash: {EmailHash}, IP: {IpAddress}",
                            hashedEmail, GetClientIpAddress());
                        return null;
                    }

                    if (!IsUserActiveAndValid(user!))
                    {
                        _logger.LogWarning("Tentativa de login de usuário inativo ou inválido: {UserId}", user!.Id);
                        return null;
                    }

                    return GenerateLoginResponse(user!);
                }
                catch (Exception ex)
                {
                    var hashedEmail = HashForLogging(normalizedEmail);
                    _logger.LogError(ex, "Erro durante processo de login. Email hash: {EmailHash}", hashedEmail);
                    return null;
                }
            }
        }

        /// <summary>
        /// Método seguro para mudança de senha usando ChangePasswordRequest com Memory<char>
        /// </summary>
        public async Task<bool> SecureChangePasswordAsync(int userId, SecureChangePasswordRequest request)
        {
            using (request)
            {
                if (request == null || userId <= 0)
                {
                    _logger.LogWarning("Tentativa de alteração de senha com parâmetros inválidos");
                    return false;
                }

                if (request.GetCurrentPassword().IsEmpty ||
                    request.GetNewPassword().IsEmpty ||
                    request.GetConfirmPassword().IsEmpty)
                {
                    _logger.LogWarning("Tentativa de alteração de senha com campos vazios para usuário: {UserId}", userId);
                    return false;
                }

                if (!request.PasswordsMatch())
                {
                    _logger.LogWarning("Tentativa de alteração de senha - senhas não coincidem para usuário: {UserId}", userId);
                    return false;
                }

                if (!IsPasswordSecureSpan(request.GetNewPassword()))
                {
                    _logger.LogWarning("Tentativa de alteração de senha - nova senha não atende aos critérios de segurança para usuário: {UserId}", userId);
                    return false;
                }

                try
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user == null)
                    {
                        _logger.LogWarning("Tentativa de alteração de senha para usuário inexistente: {UserId}", userId);
                        return false;
                    }

                    var isCurrentPasswordValid = VerifyPasswordSecure(request.GetCurrentPassword(), user.PasswordHash);
                    if (!isCurrentPasswordValid)
                    {
                        _logger.LogWarning("Tentativa de alteração de senha - senha atual incorreta para usuário: {UserId}", userId);
                        return false;
                    }

                    var isSamePassword = VerifyPasswordSecure(request.GetNewPassword(), user.PasswordHash);
                    if (isSamePassword)
                    {
                        _logger.LogWarning("Tentativa de alteração de senha - nova senha igual à atual para usuário: {UserId}", userId);
                        return false;
                    }

                    user.PasswordHash = _passwordHashService.HashPassword(request.GetNewPassword());
                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();

                    _logger.LogInformation("Senha alterada com sucesso para usuário: {UserId}", userId);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao alterar senha para usuário: {UserId}", userId);
                    return false;
                }
            }
        }

        #endregion

        #region Métodos Públicos Restantes

        public async Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _logger.LogWarning("Tentativa de refresh token com token vazio");
                return null;
            }

            try
            {
                // TODO: Implementar lógica completa de refresh token
                _logger.LogWarning("Tentativa de refresh token - funcionalidade não implementada completamente");
                await Task.CompletedTask;
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante refresh token");
                return null;
            }
        }

        public async Task LogoutAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Tentativa de logout com userId inválido: {UserId}", userId);
                return;
            }

            try
            {
                var token = ExtractTokenFromCurrentRequest();

                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        if (!_jwtService.ValidateToken(token))
                        {
                            _logger.LogWarning("Tentativa de logout com token inválido para usuário: {UserId}", userId);
                            return;
                        }

                        var jti = _jwtService.GetJtiFromToken(token);
                        var expiration = _jwtService.GetExpirationFromToken(token);

                        await _tokenBlacklistService.AddToBlacklistAsync(jti, expiration);
                        _logger.LogInformation("Logout realizado com sucesso para usuário: {UserId}", userId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar token durante logout do usuário: {UserId}", userId);
                    }
                }
                else
                {
                    _logger.LogWarning("Logout realizado sem token válido para usuário: {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante processo de logout para usuário: {UserId}", userId);
            }
        }

        #endregion

        #region Métodos Privados Seguros

        private (bool isPasswordValid, bool isUserValid) VerifyCredentialsSecure(
            ReadOnlySpan<char> password, 
            Domain.Aggregates.UserAggregate.User? user)
        {
            var isPasswordValid = false;
            var hashToVerify = user?.PasswordHash ?? _dummyHash.Value;

            try
            {
                isPasswordValid = user != null && VerifyPasswordSecure(password, hashToVerify);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante verificação de credenciais");
                if (user == null)
                {
                    _passwordHashService.VerifyPassword(password, _dummyHash.Value);
                }
            }

            return (isPasswordValid, user != null);
        }

        private bool VerifyPasswordSecure(ReadOnlySpan<char> password, string hash)
        {
            try
            {
                return _passwordHashService.VerifyPassword(password, hash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante verificação de senha");
                return false;
            }
        }

        private static bool IsPasswordSecureSpan(ReadOnlySpan<char> password)
        {
            const int MinLength = 8;
            const int MaxLength = 128;

            if (password.Length < MinLength || password.Length > MaxLength)
                return false;

            var hasUpper = false;
            var hasLower = false;
            var hasDigit = false;
            var hasSpecial = false;

            foreach (var c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            var criteriaCount = (hasUpper ? 1 : 0) + (hasLower ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSpecial ? 1 : 0);
            return criteriaCount >= 3;
        }

        #endregion

        #region Métodos Privados (compatibilidade)

        private string? ExtractTokenFromCurrentRequest()
        {
            try
            {
                var headers = _httpContextAccessor.HttpContext?.Request.Headers;
                if (headers == null)
                    return null;

                if (headers.TryGetValue("Authorization", out var authHeader))
                {
                    var authHeaderValue = authHeader.ToString();
                    if (!string.IsNullOrEmpty(authHeaderValue) && authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        return authHeaderValue["Bearer ".Length..].Trim();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair token do request");
                return null;
            }
        }

        private async Task SimulateConstantTimeAsync()
        {
            await Task.Run(() => _passwordHashService.VerifyPassword("dummy", _dummyHash.Value));
        }

        private string GenerateDummyHash()
        {
            var uniqueData = $"dummy_password_{Environment.MachineName}_{Environment.ProcessId}_{DateTime.UtcNow:yyyyMMdd}";
            return _passwordHashService.HashPassword(uniqueData.AsSpan());
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > 254)
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            var normalized = email.Trim().ToLowerInvariant();
            
            if (!IsValidEmail(normalized))
                throw new ArgumentException("Email inválido", nameof(email));

            return normalized;
        }

        private LoginResponse GenerateLoginResponse(Domain.Aggregates.UserAggregate.User user)
        {
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            _logger.LogInformation("Login bem-sucedido para usuário ID: {UserId}", user.Id);

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

        private string GetClientIpAddress()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return "Unknown";

                var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp;
                }

                return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private static string HashForLogging(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash)[..8];
        }

        private static bool IsUserActiveAndValid(Domain.Aggregates.UserAggregate.User user)
        {
            return user.IsValid();
        }

        #endregion
    }
}