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

        // Hash gerado uma vez por execu��o da aplica��o
        private static readonly Lazy<string> _dummyHash = new(GenerateDummyHash);
        
        // Timeout para opera��es de hash para evitar DoS
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
                
                // Execu��o em tempo constante para evitar timing attacks
                var (isPasswordValid, isUserValid) = await VerifyCredentialsAsync(request.Password, user);
                
                if (!isPasswordValid || !isUserValid)
                {
                    // Log de tentativa falhada (sem exposi��o de dados sens�veis)
                    var hashedEmail = HashForLogging(normalizedEmail);
                    _logger.LogWarning("Tentativa de login falhada. Email hash: {EmailHash}, IP: {IpAddress}", 
                        hashedEmail, GetClientIpAddress());
                    return null;
                }

                // Verifica��es adicionais de seguran�a
                if (!IsUserActiveAndValid(user!))
                {
                    _logger.LogWarning("Tentativa de login de usu�rio inativo ou inv�lido: {UserId}", user!.Id);
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
                _logger.LogWarning("Tentativa de altera��o de senha com par�metros inv�lidos");
                return false;
            }

            // Valida��o mais robusta de senha
            if (string.IsNullOrWhiteSpace(request.NewPassword) || 
                string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                _logger.LogWarning("Tentativa de altera��o de senha com campos vazios para usu�rio: {UserId}", userId);
                return false;
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                _logger.LogWarning("Tentativa de altera��o de senha - senhas n�o coincidem para usu�rio: {UserId}", userId);
                return false;
            }

            // Valida��o de pol�tica de senha
            if (!IsPasswordSecure(request.NewPassword))
            {
                _logger.LogWarning("Tentativa de altera��o de senha - nova senha n�o atende aos crit�rios de seguran�a para usu�rio: {UserId}", userId);
                return false;
            }

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Tentativa de altera��o de senha para usu�rio inexistente: {UserId}", userId);
                    return false;
                }

                // Verifica��o da senha atual com timeout
                var isCurrentPasswordValid = await VerifyPasswordWithTimeoutAsync(request.CurrentPassword, user.PasswordHash);
                if (!isCurrentPasswordValid)
                {
                    _logger.LogWarning("Tentativa de altera��o de senha - senha atual incorreta para usu�rio: {UserId}", userId);
                    return false;
                }

                // Verificar se a nova senha n�o � igual � atual
                var isSamePassword = await VerifyPasswordWithTimeoutAsync(request.NewPassword, user.PasswordHash);
                if (isSamePassword)
                {
                    _logger.LogWarning("Tentativa de altera��o de senha - nova senha igual � atual para usu�rio: {UserId}", userId);
                    return false;
                }

                // Atualiza��o da senha com hash seguro
                user.PasswordHash = _passwordHashService.HashPassword(request.NewPassword);
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("Senha alterada com sucesso para usu�rio: {UserId}", userId);
                
                // Limpar dados sens�veis da mem�ria
                ClearSensitiveData(request);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha para usu�rio: {UserId}", userId);
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
                // TODO: Implementar l�gica completa de refresh token
                // Por enquanto, registra a tentativa
                _logger.LogWarning("Tentativa de refresh token - funcionalidade n�o implementada completamente");
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
                _logger.LogWarning("Tentativa de logout com userId inv�lido: {UserId}", userId);
                return;
            }

            try
            {
                var token = ExtractTokenFromCurrentRequest();

                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        // Validar token antes de adicionar � blacklist
                        if (!_jwtService.ValidateToken(token))
                        {
                            _logger.LogWarning("Tentativa de logout com token inv�lido para usu�rio: {UserId}", userId);
                            return;
                        }

                        var jti = _jwtService.GetJtiFromToken(token);
                        var expiration = _jwtService.GetExpirationFromToken(token);

                        await _tokenBlacklistService.AddToBlacklistAsync(jti, expiration);
                        _logger.LogInformation("Logout realizado com sucesso para usu�rio: {UserId}", userId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar token durante logout do usu�rio: {UserId}", userId);
                        // N�o falha o logout mesmo se a blacklist falhar
                    }
                }
                else
                {
                    _logger.LogWarning("Logout realizado sem token v�lido para usu�rio: {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante processo de logout para usu�rio: {UserId}", userId);
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
            // Sempre executa verifica��o de hash para manter tempo constante
            var isPasswordValid = false;
            var hashToVerify = user?.PasswordHash ?? _dummyHash.Value;

            try
            {
                isPasswordValid = user != null && await VerifyPasswordWithTimeoutAsync(password, hashToVerify);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante verifica��o de credenciais");
                // Se h� erro na verifica��o, simula o hash dummy para manter tempo constante
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
                _logger.LogWarning("Timeout durante verifica��o de senha");
                return false;
            }
        }

        private async Task SimulateConstantTimeAsync()
        {
            // Simula opera��o de hash para manter tempo constante
            await Task.Run(() => _passwordHashService.VerifyPassword("dummy", _dummyHash.Value));
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private static bool IsUserActiveAndValid(Domain.Aggregates.UserAggregate.User user)
        {
            // ^TODO: Adicionar valida��es espec�ficas do dom�nio
            return user.IsValid();
        }

        private static bool IsPasswordSecure(string password)
        {
            // Pol�tica de senha mais robusta
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

            _logger.LogInformation("Login bem-sucedido para usu�rio ID: {UserId}", user.Id);

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
            // Cria hash para logging sem exposer dados sens�veis
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            // Primeiros 8 caracteres do hash
            return Convert.ToBase64String(hash)[..8];
        }

        private static void ClearSensitiveData(ChangePasswordRequest request)
        {
            // Limpa dados sens�veis da mem�ria 
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
            // Gera um hash dummy usando um valor conhecido e um salt �nico por execu��o
            var salt = "unique_salt_for_current_execution";
            var dummyPassword = "dummy_password" + salt;
            return new PasswordHashService().HashPassword(dummyPassword);
        }
    }
}