using IAMUAYTHAI.Application.Abstractions.Features.Student.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Student.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.User.ViewModel;

namespace IAMUAYTHAI.Application.Abstractions.Features.Admin.Service
{
    public interface IAdminService
    {
        Task<TeacherViewModel> CreateTeacherAsync(CreateTeacherRequest request);
        Task<StudentViewModel> CreateStudentAsync(CreateStudentRequest request);
        Task<IEnumerable<UserViewModel>> GetAllUsersAsync();
        Task DeleteUserAsync(int userId);
        Task<UserViewModel> GetUserByIdAsync(int id);
    }
}