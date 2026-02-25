using IAMUAYTHAI.Application.Abstractions.Features.Evolution.Repository;
using IAMUAYTHAI.Infra.Features.Evolution.Repository;

namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class EvolutionInjections
    {
        public static IServiceCollection AddEvolutionInjections(this IServiceCollection services)
        {
            services.AddScoped<IEvolutionRepository, EvolutionRepository>();
            return services;
        }
    }
}
