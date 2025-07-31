using IAMUAYTHAI_API.DependencyInjection.Features;

namespace IAMUAYTHAI_API.DependencyInjection
{
    public static class ConfigureFeaturesServices
    {
        public static IServiceCollection AddFeaturesServices(this IServiceCollection services)
        {
            services.AddUserInjections();

            return services;
        }
    }
}
