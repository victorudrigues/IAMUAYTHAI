using IAMUAYTHAI.Application.Abstractions.Features.Class.Repository;
using IAMUAYTHAI.Infra.Features.Class.Repository;

namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class ClassInjections
    {
        public static IServiceCollection AddClassInjections(this IServiceCollection services)
        {
            services.AddScoped<IClassRepository, ClassRepository>();
            return services;
        }
    }
}
