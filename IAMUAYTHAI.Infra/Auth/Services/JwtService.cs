using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Application.Abstractions.Options;
using IAMUAYTHAI.Domain.Aggregates.UserAggregate;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IAMUAYTHAI.Infra.Auth.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _options;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IOptions<JwtOptions> options, ILogger<JwtService> logger)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;

            if (!_options.IsValid())
            {
                _logger.LogError("Configurações JWT inválidas detectadas");
                throw new InvalidOperationException("Configurações JWT são inválidas ou estão faltando");
            }
        }

        public string GenerateToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_options.SecretKey);
                
                var now = DateTime.UtcNow;
                var expiry = now.AddHours(_options.ExpirationHours);

                // Gerar JTI único e seguro
                var jti = GenerateSecureJti();
                
                _logger.LogDebug("Gerando token para usuário {UserId} com JTI: {JTI}. Valido de {NotBefore} até {Expires}", 
                    user.Id, jti, now, expiry);

                // Claims incluindo JTI e IAT na ClaimsIdentity corretamente
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Name),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Profile.ToString()),
                    new("profile", user.Profile.ToString()),
                    new(JwtRegisteredClaimNames.Jti, jti),
                    new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    new(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims, "jwt"),
                    Issuer = _options.Issuer,
                    Audience = _options.Audience,
                    NotBefore = now,
                    Expires = expiry,
                    IssuedAt = now,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), 
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                _logger.LogInformation("Token gerado com sucesso para usuário {UserId}. Expira em: {ExpirationTime}", 
                    user.Id, expiry);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token para usuário {UserId}", user.Id);
                throw new InvalidOperationException("Falha na geração do token JWT", ex);
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomBytes = new byte[64]; 
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                
                var refreshToken = Convert.ToBase64String(randomBytes);
                _logger.LogDebug("Refresh token gerado com sucesso");
                
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar refresh token");
                throw new InvalidOperationException("Falha na geração do refresh token", ex);
            }
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Tentativa de validação com token vazio");
                return false;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_options.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = true,
                    // Adicionar tolerância de 5 minutos para diferenças de relógio
                    ClockSkew = TimeSpan.FromMinutes(5),
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.HmacSha256 },
                    TryAllIssuerSigningKeys = true
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                var jwtToken = validatedToken as JwtSecurityToken;
                var hasJti = jwtToken?.Claims?.Any(c => c.Type == JwtRegisteredClaimNames.Jti) == true;
                
                if (!hasJti)
                {
                    _logger.LogWarning("Token válido mas sem JTI detectado");
                    return false;
                }

                _logger.LogDebug("Token validado com sucesso. Expira em: {TokenExpiry}", jwtToken?.ValidTo);
                return true;
            }
            catch (SecurityTokenSignatureKeyNotFoundException ex)
            {
                _logger.LogWarning("Erro de chave de assinatura: {Message}. Verifique se a SecretKey está configurada corretamente", ex.Message);
                return false;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token inválido: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado durante validação de token");
                return false;
            }
        }

        public int GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);
                
                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("Token sem claim de NameIdentifier");
                    return 0;
                }

                if (!int.TryParse(userIdClaim.Value, out var userId))
                {
                    _logger.LogWarning("NameIdentifier inválido no token: {Value}", userIdClaim.Value);
                    return 0;
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair UserId do token");
                return 0;
            }
        }

        public string GetJtiFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);
                
                // Busca otimizada por JTI
                var jtiClaim = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);
                
                if (jtiClaim == null)
                {
                    _logger.LogWarning("Token sem JTI claim detectado");     
                    return string.Empty;
                }

                var jti = jtiClaim.Value;
                if (string.IsNullOrWhiteSpace(jti))
                {
                    _logger.LogWarning("JTI claim vazio detectado");
                    return string.Empty;
                }

                _logger.LogDebug("JTI extraído com sucesso: {JTI}", jti);
                return jti;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair JTI do token");
                return string.Empty;
            }
        }

        public DateTime GetExpirationFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);
                
                if (jwt.ValidTo == DateTime.MinValue)
                {
                    _logger.LogWarning("Token sem data de expiração válida");
                    return DateTime.UtcNow;
                }

                return jwt.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair data de expiração do token");
                return DateTime.UtcNow;
            }
        }

        private static string GenerateSecureJti()
        {
            var guid = Guid.NewGuid().ToString("N"); 
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return $"{guid}_{timestamp}";
        }
    }
}