using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Application.Abstractions.Features.Checkin.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Student.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Student.Service;
using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using IAMUAYTHAI.Domain.Aggregates.CheckinAggregate;
using IAMUAYTHAI.Domain.Enumerations;
using StudentDomain = IAMUAYTHAI.Domain.Aggregates.StudentAggregate.Student;

namespace IAMUAYTHAI.Application.Features.Student.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICheckinRepository _checkinRepository;
        private readonly IPasswordHashService _passwordHashService;

        public StudentService(
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            ICheckinRepository checkinRepository,
            IPasswordHashService passwordHashService)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _checkinRepository = checkinRepository;
            _passwordHashService = passwordHashService;
        }

        public async Task<StudentDomain> CreateStudentAsync(string name, string email, string password, DateTime birthDate)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (await _userRepository.EmailExistsAsync(normalizedEmail))
            {
                throw new ArgumentException("Já existe um usuário cadastrado com este e-mail.");
            }

            var passwordHash = _passwordHashService.HashPassword(password.AsSpan());

            var student = new StudentDomain
            {
                Name = name.Trim(),
                Email = normalizedEmail,
                PasswordHash = passwordHash,
                Profile = UserProfileType.Student,
                BirthDate = birthDate.Date
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();

            return student;
        }

        public async Task<StudentDomain> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null)
            {
                throw new InvalidOperationException($"Estudante com ID {id} não encontrado.");
            }

            return student;
        }

        public async Task<IEnumerable<StudentDomain>> GetAllStudentsAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task CheckinAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
            {
                throw new InvalidOperationException($"Estudante com ID {studentId} não encontrado.");
            }

            var checkin = new Checkin
            {
                StudentId = studentId,
                DateTime = DateTime.UtcNow
            };

            await _checkinRepository.AddAsync(checkin);
            await _checkinRepository.SaveChangesAsync();
        }
    }
}
