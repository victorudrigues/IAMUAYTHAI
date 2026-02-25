using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using IAMUAYTHAI.Application.Abstractions.Features.Class.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Class.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Checkin.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Checkin.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Evolution.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Evolution.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Student.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Student.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.StudentClass.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Service;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.User.ViewModel;
using IAMUAYTHAI.Domain.Aggregates.CheckinAggregate;
using IAMUAYTHAI.Domain.Aggregates.ClassAggregate;
using IAMUAYTHAI.Domain.Enumerations;
using TeacherDomain = IAMUAYTHAI.Domain.Aggregates.TeacherAggregate.Teacher;

namespace IAMUAYTHAI.Application.Features.Teacher.Service
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClassRepository _classRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly ICheckinRepository _checkinRepository;
        private readonly IEvolutionRepository _evolutionRepository;
        private readonly IPasswordHashService _passwordHashService;

        public TeacherService(
            ITeacherRepository teacherRepository,
            IUserRepository userRepository,
            IClassRepository classRepository,
            IStudentRepository studentRepository,
            IStudentClassRepository studentClassRepository,
            ICheckinRepository checkinRepository,
            IEvolutionRepository evolutionRepository,
            IPasswordHashService passwordHashService)
        {
            _teacherRepository = teacherRepository;
            _userRepository = userRepository;
            _classRepository = classRepository;
            _studentRepository = studentRepository;
            _studentClassRepository = studentClassRepository;
            _checkinRepository = checkinRepository;
            _evolutionRepository = evolutionRepository;
            _passwordHashService = passwordHashService;
        }

        public async Task CreateTeacherAsync(string name, string email, string password)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (await _userRepository.EmailExistsAsync(normalizedEmail))
            {
                throw new ArgumentException("Já existe um usuário cadastrado com este e-mail.");
            }

            var passwordHash = _passwordHashService.HashPassword(password.AsSpan());

            var teacher = new TeacherDomain
            {
                Name = name.Trim(),
                Email = normalizedEmail,
                PasswordHash = passwordHash,
                Profile = UserProfileType.Teacher
            };

            await _teacherRepository.AddAsync(teacher);
            await _teacherRepository.SaveChangesAsync();
        }

        public async Task<UserViewModel> GetTeacherByIdAsync(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);

            if (teacher == null)
            {
                throw new InvalidOperationException($"Professor com ID {id} não encontrado.");
            }

            return new UserViewModel
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Email = teacher.Email,
                Profile = teacher.Profile.ToString()
            };
        }

        public async Task<IEnumerable<TeacherViewModel>> GetAllTeachersAsync()
        {
            var teachers = await _teacherRepository.GetAllAsync();

            return teachers.Select(t => new TeacherViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email
            });
        }

        public async Task<ClassViewModel> CreateClassAsync(int teacherId, DateTime dateTime, string description)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);

            if (teacher == null)
            {
                throw new InvalidOperationException($"Professor com ID {teacherId} não encontrado.");
            }

            var classEntity = new Class
            {
                TeacherId = teacherId,
                DateTime = dateTime,
                Description = description?.Trim() ?? string.Empty
            };

            await _classRepository.AddAsync(classEntity);
            await _classRepository.SaveChangesAsync();

            return new ClassViewModel
            {
                Id = classEntity.Id,
                TeacherId = classEntity.TeacherId,
                DateTime = classEntity.DateTime,
                Description = classEntity.Description
            };
        }

        public async Task CheckinStudentAsync(int teacherId, int studentId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);

            if (teacher == null)
                throw new InvalidOperationException($"Professor com ID {teacherId} não encontrado.");
            
            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                throw new InvalidOperationException($"Estudante com ID {studentId} não encontrado.");
            
            var teacherClasses = await _classRepository.GetByTeacherIdAsync(teacherId);
            var classIds = teacherClasses.Select(c => c.Id).ToList();

            if (classIds.Count == 0)
                throw new InvalidOperationException("Professor não possui turmas cadastradas.");

            var studentClasses = await _studentClassRepository.GetByClassIdsAsync(classIds);
            var studentIsInTeacherClass = studentClasses.Any(sc => sc.StudentId == studentId);

            if (!studentIsInTeacherClass)
                throw new InvalidOperationException("Estudante não pertence a nenhuma turma deste professor.");

            var checkin = new Checkin
            {
                StudentId = studentId,
                DateTime = DateTime.UtcNow
            };

            await _checkinRepository.AddAsync(checkin);
            await _checkinRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentViewModel>> GetMyStudentsAsync(int teacherId)
        {
            var teacherClasses = await _classRepository.GetByTeacherIdAsync(teacherId);
            var classIds = teacherClasses.Select(c => c.Id).ToList();

            if (classIds.Count == 0)
                return [];

            var studentClasses = await _studentClassRepository.GetByClassIdsAsync(classIds);
            var studentIds = studentClasses.Select(sc => sc.StudentId).Distinct().ToList();

            if (studentIds.Count == 0)
                return [];

            var students = await _studentRepository.GetByIdsAsync(studentIds);

            return students.Select(s => new StudentViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                BirthDate = s.BirthDate
            });
        }

        public async Task<IEnumerable<ClassViewModel>> GetMyClassesAsync(int teacherId)
        {
            var classes = await _classRepository.GetByTeacherIdAsync(teacherId);

            return classes.Select(c => new ClassViewModel
            {
                Id = c.Id,
                TeacherId = c.TeacherId,
                DateTime = c.DateTime,
                Description = c.Description
            });
        }

        public async Task<StudentDetailViewModel> GetStudentByIdAsync(int teacherId, int studentId)
        {
            var teacherClasses = await _classRepository.GetByTeacherIdAsync(teacherId);
            var classIds = teacherClasses.Select(c => c.Id).ToList();

            if (classIds.Count > 0)
            {
                var studentClasses = await _studentClassRepository.GetByClassIdsAsync(classIds);
                var studentIsInTeacherClass = studentClasses.Any(sc => sc.StudentId == studentId);

                if (!studentIsInTeacherClass)
                    throw new InvalidOperationException("Estudante não pertence a nenhuma turma deste professor.");
            }

            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
                throw new InvalidOperationException($"Estudante com ID {studentId} não encontrado.");

            return await PopuleDetails(studentId, student);
        }

        private async Task<StudentDetailViewModel> PopuleDetails(int studentId, Domain.Aggregates.StudentAggregate.Student student)
        {
            var checkins = await _checkinRepository.GetByStudentIdAsync(studentId);
            var evolution = await _evolutionRepository.GetCurrentByStudentIdAsync(studentId);
            var studentClassRecords = await _studentClassRepository.GetByStudentIdAsync(studentId);

            return new StudentDetailViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                BirthDate = student.BirthDate,
                Checkins = checkins.Select(c => new CheckinViewModel
                {
                    Id = c.Id,
                    StudentId = c.StudentId,
                    DateTime = c.DateTime
                }).ToList(),
                CurrentEvolution = evolution != null ? new EvolutionViewModel
                {
                    Id = evolution.Id,
                    StudentId = evolution.StudentId,
                    CurrentLevel = evolution.CurrentLevel,
                    NextLevel = evolution.NextLevel,
                    NextKruangExpectedDate = evolution.NextKruangExpectedDate,
                    EligibleForNextLevel = evolution.EligibleForNextLevel
                } : null,
                StudentClasses = studentClassRecords.Select(sc => new StudentClassViewModel
                {
                    Id = sc.Id,
                    ClassId = sc.ClassId,
                    Class = new ClassViewModel
                    {
                        Id = sc.Class.Id,
                        TeacherId = sc.Class.TeacherId,
                        DateTime = sc.Class.DateTime,
                        Description = sc.Class.Description
                    },
                    WasPresent = sc.WasPresent,
                    Justification = sc.Justification
                }).ToList()
            };
        }
    }
}
