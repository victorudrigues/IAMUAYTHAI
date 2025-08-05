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
                    if (!jwtService.ValidateToken(token))
                    {
                        _logger.LogWarning("Token inválido detectado - Path: {Path}", context.Request.Path);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token inválido");
                        return;
                    }

                    var jti = jwtService.GetJtiFromToken(token);
                    
                    if (string.IsNullOrEmpty(jti))
                    {
                        _logger.LogWarning("Token sem JTI detectado - Path: {Path} | Rejeitando por segurança", context.Request.Path);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token sem identificador válido");
                        return;
                    }
                    
                    var isBlacklisted = await blacklistService.IsTokenBlacklistedAsync(jti);
                    if (isBlacklisted)
                    {
                        _logger.LogWarning("Token BLACKLISTED detectado - JTI: {TokenId} | Path: {Path}", jti, context.Request.Path);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token Inválido");
                        return;
                    }

                    _logger.LogDebug("  Token válido - JTI: {TokenId} | Path: {Path}", jti, context.Request.Path);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro crítico ao validar token - Path: {Path}", context.Request.Path);
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync("Serviço temporariamente indisponível");
                    return;
                }
            }
            else
            {
                _logger.LogDebug("Nenhum token fornecido - Path: {Path}", context.Request.Path);
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