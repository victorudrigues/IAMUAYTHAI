using IAMUAYTHAI.Application.Abstractions.Features.Class.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

                return Ok(new
                {
                    message = "Aula criada com sucesso",
                    classId = createdClass.Id,
                    teacherId,
                    dateTime = createdClass.DateTime,
                    description = createdClass.Description
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        [HttpPost("checkin/{studentId}")]
        public async Task<IActionResult> CheckinStudent(int studentId)
        {
            try
            {
                var teacherId = GetCurrentUserId();

                await _teacherService.CheckinStudentAsync(teacherId, studentId);

                return Ok(new
                {
                    message = $"Check-in realizado com sucesso",
                    studentId,
                    teacherId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        [HttpGet("my-students")]
        public async Task<IActionResult> GetMyStudents()
        {
            try
            {
                var teacherId = GetCurrentUserId();

                var students = await _teacherService.GetMyStudentsAsync(teacherId);

                return Ok(new
                {
                    message = "Lista de estudantes do professor",
                    teacherId,
                    students = students.Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        email = s.Email,
                        birthDate = s.BirthDate
                    })
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        [HttpGet("my-classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            try
            {
                var teacherId = GetCurrentUserId();

                var classes = await _teacherService.GetMyClassesAsync(teacherId);

                return Ok(new
                {
                    message = "Minhas aulas",
                    teacherId,
                    classes = classes.Select(c => new
                    {
                        id = c.Id,
                        dateTime = c.DateTime,
                        description = c.Description
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var teacherId = GetCurrentUserId();

                var teacher = await _teacherService.GetTeacherByIdAsync(teacherId);

                return Ok(new
                {
                    id = teacher.Id,
                    name = teacher.Name,
                    email = teacher.Email,
                    profile = teacher.Profile.ToString()
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}