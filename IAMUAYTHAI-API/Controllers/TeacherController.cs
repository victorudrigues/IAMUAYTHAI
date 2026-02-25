using IAMUAYTHAI.Application.Abstractions.Features.Admin.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Class.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Class.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Checkin.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Evolution.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Student.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Service;
using IAMUAYTHAI.Application.Abstractions.Features.User.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassEntity = IAMUAYTHAI.Domain.Aggregates.ClassAggregate.Class;
using CheckinEntity = IAMUAYTHAI.Domain.Aggregates.CheckinAggregate.Checkin;
using StudentClassEntity = IAMUAYTHAI.Domain.Aggregates.StudentClassAggregate.StudentClass;
using StudentDomain = IAMUAYTHAI.Domain.Aggregates.StudentAggregate.Student;

namespace IAMUAYTHAI_API.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpPost("classes")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request)
        {
            try
            {
                var teacherId = GetCurrentUserId();

                var createdClass = await _teacherService.CreateClassAsync(
                    teacherId,
                    request.DateTime,
                    request.Description);

                return Ok(createdClass);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiErrorViewModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorViewModel { Message = "Erro interno do servidor", Details = ex.Message });
            }
        }

        [HttpPost("checkin/{studentId}")]
        public async Task<IActionResult> CheckinStudent(int studentId)
        {
            try
            {
                var teacherId = GetCurrentUserId();

                await _teacherService.CheckinStudentAsync(teacherId, studentId);

                return Ok(new CheckinResultViewModel
                {
                    Message = "Check-in realizado com sucesso",
                    StudentId = studentId,
                    TeacherId = teacherId,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiErrorViewModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorViewModel { Message = "Erro interno do servidor", Details = ex.Message });
            }
        }

        [HttpGet("my-students")]
        public async Task<IActionResult> GetMyStudents()
        {
            try
            {
                var teacherId = GetCurrentUserId();

                var students = await _teacherService.GetMyStudentsAsync(teacherId);

                return Ok(students);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiErrorViewModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorViewModel { Message = "Erro interno do servidor", Details = ex.Message });
            }
        }

        [HttpGet("my-students/{studentId}")]
        public async Task<IActionResult> GetStudentById(int studentId)
        {
            try
            {
                var teacherId = GetCurrentUserId();

                var student = await _teacherService.GetStudentByIdAsync(teacherId, studentId);

                if (student == null)
                    return NotFound(new ApiErrorViewModel { Message = "Estudante não encontrado ou não pertence a este professor." });

                return Ok(student);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorViewModel { Message = "Erro interno do servidor", Details = ex.Message });
            }
        }

        [HttpGet("my-classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            try
            {
                var teacherId = GetCurrentUserId();

                var classes = await _teacherService.GetMyClassesAsync(teacherId);

                return Ok(classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorViewModel { Message = "Erro interno do servidor", Details = ex.Message });
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var teacherId = GetCurrentUserId();

                var teacher = await _teacherService.GetTeacherByIdAsync(teacherId);

                return Ok(teacher);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiErrorViewModel { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorViewModel { Message = "Erro interno do servidor", Details = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}
