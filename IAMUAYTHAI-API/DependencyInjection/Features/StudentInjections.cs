using IAMUAYTHAI.Application.Abstractions.Features.Student.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Student.Service;
using IAMUAYTHAI.Application.Features.Student.Service;
using IAMUAYTHAI.Infra.Features.Student.Repository;

namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class StudentInjections
    {
        public static IServiceCollection AddStudentInjections(this IServiceCollection services)
        {
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentService, StudentService>();

            return services;
        }
    }
}