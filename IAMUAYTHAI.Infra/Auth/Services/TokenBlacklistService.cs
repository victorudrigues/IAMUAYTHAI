using IAMUAYTHAI.Application.Abstractions.Features.Auth.Dto;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IAMUAYTHAI.Infra.Auth.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<TokenBlacklistService> _logger;
        private const string BLACKLIST_PREFIX = "blacklisted_token:";

        public TokenBlacklistService(
            IDistributedCache cache,
            ILogger<TokenBlacklistService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task AddToBlacklistAsync(string jti, DateTime expiration)
        {
            try
            {
                var key = $"{BLACKLIST_PREFIX}{jti}";
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = expiration
                };

                var tokenInfo = new BlackListedTokenInfo
                {
                    TokenId = jti,
                    BlacklistedAt = DateTime.UtcNow,
                    ExpiresAt = expiration
                };

                var serializedToken = JsonSerializer.Serialize(tokenInfo);
                await _cache.SetStringAsync(key, serializedToken, options);

                _logger.LogInformation("Token {TokenId} adicionado à lista negra", jti);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar token {TokenId} à lista negra", jti);
                throw;
            }
        }

        public async Task<bool> IsTokenBlacklistedAsync(string jti)
        {
            try
            {
                var key = $"{BLACKLIST_PREFIX}{jti}";
                var result = await _cache.GetStringAsync(key);
                return !string.IsNullOrEmpty(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se token {TokenId} está na lista negra", jti);
                // Em caso de erro, considerando como não blacklisted para não bloquear usuários válidos
                return false;
            }
        }

        public async Task RemoveExpiredTokensAsync()
        {
            //É automático
            //TODO: Pensar em uma estratégia de limpeza
            await Task.CompletedTask;
            _logger.LogInformation("Limpeza de tokens expirados concluída");
        }

    }
}