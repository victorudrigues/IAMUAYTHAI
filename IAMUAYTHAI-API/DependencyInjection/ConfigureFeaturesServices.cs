using IAMUAYTHAI_API.DependencyInjection.Features;

namespace IAMUAYTHAI_API.DependencyInjection
{
    public static class ConfigureFeaturesServices
    {
        public static IServiceCollection AddFeaturesServices(this IServiceCollection services)
        {
            services.AddUserInjections();
            services.AddAuthInjections();
            services.AddAdminInjections();
            services.AddCheckinInjections();
            services.AddClassInjections();
            services.AddTeacherInjections();
            services.AddStudentInjections();

            return services;
        }
    }
}
