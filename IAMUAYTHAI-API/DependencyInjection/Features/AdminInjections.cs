using IAMUAYTHAI.Application.Abstractions.Features.Admin.Service;
using IAMUAYTHAI.Application.Features.Admin.Service;


namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class AdminInjections
    {
        public static IServiceCollection AddAdminInjections(this IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
            
            return services;
        }
    }
}
