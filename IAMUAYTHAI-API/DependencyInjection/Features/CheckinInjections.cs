using IAMUAYTHAI.Application.Abstractions.Features.Checkin.Repository;
using IAMUAYTHAI.Infra.Features.Checkin.Repository;

namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class CheckinInjections
    {
        public static IServiceCollection AddCheckinInjections(this IServiceCollection services)
        {
            services.AddScoped<ICheckinRepository, CheckinRepository>();
            return services;
        }
    }
}
