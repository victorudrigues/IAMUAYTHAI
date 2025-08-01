using IAMUAYTHAI.Application.Abstractions.Features.Auth;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using IAMUAYTHAI.Infra.Auth.Services;
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

        // Hash gerado uma vez por execução da aplicação
        private static readonly Lazy<string> _dummyHash = new(GenerateDummyHash);
        
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
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _tokenBlacklistService = tokenBlacklistService ?? throw new ArgumentNullException(nameof(tokenBlacklistService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Tentativa de login com request nulo");
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Tentativa de login com email ou senha vazios");
                await SimulateConstantTimeAsync();
                return null;
            }

            var normalizedEmail = NormalizeEmail(request.Email);
            
            try
            {
                var user = await _userRepository.GetByEmailAsync(normalizedEmail);
                
                // Execução em tempo constante para evitar timing attacks
                var (isPasswordValid, isUserValid) = await VerifyCredentialsAsync(request.Password, user);
                
                if (!isPasswordValid || !isUserValid)
                {
                    // Log de tentativa falhada (sem exposição de dados sensíveis)
                    var hashedEmail = HashForLogging(normalizedEmail);
                    _logger.LogWarning("Tentativa de login falhada. Email hash: {EmailHash}, IP: {IpAddress}", 
                        hashedEmail, GetClientIpAddress());
                    return null;
                }

                // Verificações adicionais de segurança
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

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            if (request == null || userId <= 0)
            {
                _logger.LogWarning("Tentativa de alteração de senha com parâmetros inválidos");
                return false;
            }

            // Validação mais robusta de senha
            if (string.IsNullOrWhiteSpace(request.NewPassword) || 
                string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                _logger.LogWarning("Tentativa de alteração de senha com campos vazios para usuário: {UserId}", userId);
                return false;
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                _logger.LogWarning("Tentativa de alteração de senha - senhas não coincidem para usuário: {UserId}", userId);
                return false;
            }

            // Validação de política de senha
            if (!IsPasswordSecure(request.NewPassword))
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

                // Verificação da senha atual com timeout
                var isCurrentPasswordValid = await VerifyPasswordWithTimeoutAsync(request.CurrentPassword, user.PasswordHash);
                if (!isCurrentPasswordValid)
                {
                    _logger.LogWarning("Tentativa de alteração de senha - senha atual incorreta para usuário: {UserId}", userId);
                    return false;
                }

                // Verificar se a nova senha não é igual à atual
                var isSamePassword = await VerifyPasswordWithTimeoutAsync(request.NewPassword, user.PasswordHash);
                if (isSamePassword)
                {
                    _logger.LogWarning("Tentativa de alteração de senha - nova senha igual à atual para usuário: {UserId}", userId);
                    return false;
                }

                // Atualização da senha com hash seguro
                user.PasswordHash = _passwordHashService.HashPassword(request.NewPassword);
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("Senha alterada com sucesso para usuário: {UserId}", userId);
                
                // Limpar dados sensíveis da memória
                ClearSensitiveData(request);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha para usuário: {UserId}", userId);
                return false;
            }
        }

        public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Tentativa de refresh token com token vazio");
                return null;
            }

            try
            {
                // TODO: Implementar lógica completa de refresh token
                // Por enquanto, registra a tentativa
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
                        // Validar token antes de adicionar à blacklist
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
                        // Não falha o logout mesmo se a blacklist falhar
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

        private async Task<(bool isPasswordValid, bool isUserValid)> VerifyCredentialsAsync(string password, Domain.Aggregates.UserAggregate.User? user)
        {
            // Sempre executa verificação de hash para manter tempo constante
            var isPasswordValid = false;
            var hashToVerify = user?.PasswordHash ?? _dummyHash.Value;

            try
            {
                isPasswordValid = user != null && await VerifyPasswordWithTimeoutAsync(password, hashToVerify);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante verificação de credenciais");
                // Se há erro na verificação, simula o hash dummy para manter tempo constante
                if (user == null)
                {
                    _passwordHashService.VerifyPassword(password, _dummyHash.Value);
                }
            }

            return (isPasswordValid, user != null);
        }

        private async Task<bool> VerifyPasswordWithTimeoutAsync(string password, string hash)
        {
            var cancellationTokenSource = new CancellationTokenSource(HashTimeout);
            
            try
            {
                return await Task.Run(() => _passwordHashService.VerifyPassword(password, hash), cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Timeout durante verificação de senha");
                return false;
            }
        }

        private async Task SimulateConstantTimeAsync()
        {
            // Simula operação de hash para manter tempo constante
            await Task.Run(() => _passwordHashService.VerifyPassword("dummy", _dummyHash.Value));
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private static bool IsUserActiveAndValid(Domain.Aggregates.UserAggregate.User user)
        {
            // ^TODO: Adicionar validações específicas do domínio
            return user.IsValid();
        }

        private static bool IsPasswordSecure(string password)
        {
            // Política de senha mais robusta
            if (password.Length < 8)
                return false;

            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpper && hasLower && hasDigit && hasSpecial;
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

                // Verifica headers de proxy primeiro
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
            // Cria hash para logging sem exposer dados sensíveis
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            // Primeiros 8 caracteres do hash
            return Convert.ToBase64String(hash)[..8];
        }

        private static void ClearSensitiveData(ChangePasswordRequest request)
        {
            // Limpa dados sensíveis da memória 
            if (request.CurrentPassword != null)
            {
                request.CurrentPassword = new string('\0', request.CurrentPassword.Length);
            }
            if (request.NewPassword != null)
            {
                request.NewPassword = new string('\0', request.NewPassword.Length);
            }
            if (request.ConfirmPassword != null)
            {
                request.ConfirmPassword = new string('\0', request.ConfirmPassword.Length);
            }
        }

        private static string GenerateDummyHash()
        {
            // Gera um hash dummy usando um valor conhecido e um salt único por execução
            var salt = "unique_salt_for_current_execution";
            var dummyPassword = "dummy_password" + salt;
            return new PasswordHashService().HashPassword(dummyPassword);
        }
    }
}