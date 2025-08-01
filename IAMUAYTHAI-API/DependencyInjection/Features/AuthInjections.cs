using IAMUAYTHAI.Application.Abstractions.Features.Auth;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Infra.Auth;
using IAMUAYTHAI.Infra.Auth.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class AuthInjections
    {
        public static IServiceCollection AddAuthInjections(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHashService, PasswordHashService>();
            services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

            // Para HttpContextAccessor
            services.AddHttpContextAccessor();

            // Para cache distribuído
            services.AddMemoryCache();
            services.AddSingleton<IDistributedCache, MemoryDistributedCache>();

            return services;
        }
    }
}