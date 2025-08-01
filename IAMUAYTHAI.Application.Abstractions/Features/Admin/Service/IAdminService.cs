using IAMUAYTHAI.Application.Abstractions.Features.Student.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Request;
using StudentDomain = IAMUAYTHAI.Domain.Aggregates.StudentAggregate.Student;
using TeacherDomain = IAMUAYTHAI.Domain.Aggregates.TeacherAggregate.Teacher;
using UserDomain = IAMUAYTHAI.Domain.Aggregates.UserAggregate.User;

namespace IAMUAYTHAI.Application.Abstractions.Features.Admin.Service
{
    public interface IAdminService
    {
        Task<TeacherDomain> CreateTeacherAsync(CreateTeacherRequest request);
        Task<StudentDomain> CreateStudentAsync(CreateStudentRequest request);
        Task<IEnumerable<UserDomain>> GetAllUsersAsync();
        Task DeleteUserAsync(int userId);
        Task<UserDomain> GetUserByIdAsync(int id);
    }
}