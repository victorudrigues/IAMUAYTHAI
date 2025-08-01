using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;

namespace IAMUAYTHAI_API.DependencyInjection
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenBlacklistMiddleware> _logger;

        public TokenBlacklistMiddleware(RequestDelegate next, ILogger<TokenBlacklistMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklistService, IJwtService jwtService)
        {
            var token = ExtractTokenFromRequest(context);

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var jti = jwtService.GetJtiFromToken(token);
                    
                    if (!string.IsNullOrEmpty(jti) && await blacklistService.IsTokenBlacklistedAsync(jti))
                    {
                        _logger.LogWarning("Requisição bloqueada com token na lista negra. JTI: {TokenId}", jti);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token foi revogado");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro crítico ao validar status do token na lista negra. Acesso negado por segurança.");
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync("Serviço temporariamente indisponível");
                    return;
                }
            }

            await _next(context);
        }

        private static string? ExtractTokenFromRequest(HttpContext context)
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            return authHeader["Bearer ".Length..].Trim();
        }
    }
}