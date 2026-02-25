using IAMUAYTHAI.Application.Abstractions.Features.Admin.Service;
using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Application.Abstractions.Features.Student.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Student.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Student.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.User.ViewModel;
using IAMUAYTHAI.Domain.Aggregates.StudentAggregate;
using IAMUAYTHAI.Domain.Aggregates.TeacherAggregate;
using IAMUAYTHAI.Domain.Enumerations;

namespace IAMUAYTHAI.Application.Features.Admin.Service
{
    public class AdminService : IAdminService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;

        public AdminService(
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            IPasswordHashService passwordHashService)
        {
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
        }

        public async Task<TeacherViewModel> CreateTeacherAsync(CreateTeacherRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new ArgumentException("Já existe um usuário cadastrado com este e-mail.");
            }

            var passwordHash = _passwordHashService.HashPassword(request.Password.AsSpan());

            var teacher = new Teacher
            {
                Name = request.Name.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                Profile = UserProfileType.Teacher
            };

            await _teacherRepository.AddAsync(teacher);
            await _teacherRepository.SaveChangesAsync();

            return new TeacherViewModel
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Email = teacher.Email
            };
        }

        public async Task<StudentViewModel> CreateStudentAsync(CreateStudentRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new ArgumentException("Já existe um usuário cadastrado com este e-mail.");
            }

            var passwordHash = _passwordHashService.HashPassword(request.Password.AsSpan());

            var student = new Student
            {
                Name = request.Name.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                Profile = UserProfileType.Student,
                BirthDate = request.BirthDate.Date
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();

            return new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                BirthDate = student.BirthDate
            };
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Profile = u.Profile.ToString()
            });
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException($"Usuário com ID {userId} não encontrado.");
            }

            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<UserViewModel> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new InvalidOperationException($"Usuário com ID {id} não encontrado.");
            }

            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Profile = user.Profile.ToString()
            };
        }
    }
}
