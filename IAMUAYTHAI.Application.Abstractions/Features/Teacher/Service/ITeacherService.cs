using ClassEntity = IAMUAYTHAI.Domain.Aggregates.ClassAggregate.Class;
using TeacherDomain = IAMUAYTHAI.Domain.Aggregates.TeacherAggregate.Teacher;
using StudentDomain = IAMUAYTHAI.Domain.Aggregates.StudentAggregate.Student;

namespace IAMUAYTHAI.Application.Abstractions.Features.Teacher.Service
{
    public interface ITeacherService
    {
        Task<TeacherDomain> CreateTeacherAsync(string name, string email, string password);
        Task<TeacherDomain> GetTeacherByIdAsync(int id);
        Task<IEnumerable<TeacherDomain>> GetAllTeachersAsync();
        Task<ClassEntity> CreateClassAsync(int teacherId, DateTime dateTime, string description);
        Task CheckinStudentAsync(int teacherId, int studentId);
        Task<IEnumerable<StudentDomain>> GetMyStudentsAsync(int teacherId);
        Task<IEnumerable<ClassEntity>> GetMyClassesAsync(int teacherId);
    }
}