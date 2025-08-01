using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Service;
using IAMUAYTHAI.Application.Features.Teacher.Service;
using IAMUAYTHAI.Infra.Features.Teacher.Repository;

namespace IAMUAYTHAI_API.DependencyInjection.Features
{
    public static class TeacherInjections
    {
        public static IServiceCollection AddTeacherInjections(this IServiceCollection services)
        {
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<ITeacherService, TeacherService>();

            return services;
        }
    }
}