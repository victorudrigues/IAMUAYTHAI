using IAMUAYTHAI.Application.Abstractions.Features.StudentClass.Repository;
using IAMUAYTHAI.Infra.Features.StudentClass.Repository;

namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class StudentClassInjections
    {
        public static IServiceCollection AddStudentClassInjections(this IServiceCollection services)
        {
            services.AddScoped<IStudentClassRepository, StudentClassRepository>();
            return services;
        }
    }
}
