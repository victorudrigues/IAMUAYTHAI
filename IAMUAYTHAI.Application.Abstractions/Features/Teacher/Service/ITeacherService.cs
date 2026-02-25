using IAMUAYTHAI.Application.Abstractions.Features.Class.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Student.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.User.ViewModel;

namespace IAMUAYTHAI.Application.Abstractions.Features.Teacher.Service
{
    public interface ITeacherService
    {
        Task CreateTeacherAsync(string name, string email, string password);
        Task<UserViewModel> GetTeacherByIdAsync(int id);
        Task<IEnumerable<TeacherViewModel>> GetAllTeachersAsync();
        Task CreateClassAsync(int teacherId, DateTime dateTime, string description);
        Task CheckinStudentAsync(int teacherId, int studentId);
        Task<IEnumerable<StudentViewModel>> GetMyStudentsAsync(int teacherId);
        Task<IEnumerable<ClassViewModel>> GetMyClassesAsync(int teacherId);
        Task<StudentDetailViewModel> GetStudentByIdAsync(int teacherId, int studentId);
    }
}