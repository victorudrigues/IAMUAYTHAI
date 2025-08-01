using StudentDomain = IAMUAYTHAI.Domain.Aggregates.StudentAggregate.Student;

namespace IAMUAYTHAI.Application.Abstractions.Features.Student.Service
{
    public interface IStudentService
    {
        Task<StudentDomain> CreateStudentAsync(string name, string email, string password, DateTime birthDate);
        Task<StudentDomain> GetStudentByIdAsync(int id);
        Task<IEnumerable<StudentDomain>> GetAllStudentsAsync();
        Task CheckinAsync(int studentId);
    }
}