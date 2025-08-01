using IAMUAYTHAI.Application.Abstractions.Features.Auth;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Infra.Auth.Services;
using IAMUAYTHAI.Infra.AuthService;


namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class AuthInjections
    {
        public static IServiceCollection AddAuthInjections(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHashService, PasswordHashService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}