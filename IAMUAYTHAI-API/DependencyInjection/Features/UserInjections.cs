using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.User.Service;
using IAMUAYTHAI.Application.Features.User.Service;
using IAMUAYTHAI.Infra.Features.User.Repository;

namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class UserInjections
    {
        public static IServiceCollection AddUserInjections(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
