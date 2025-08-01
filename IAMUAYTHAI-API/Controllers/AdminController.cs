using IAMUAYTHAI.Application.Abstractions.Features.Admin.Service;
using IAMUAYTHAI.Application.Abstractions.Features.Student.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Request;
using IAMUAYTHAI.Domain.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAMUAYTHAI_API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminService adminService) : ControllerBase
    {
        private readonly IAdminService _adminService = adminService;

        [HttpPost("teachers")]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherRequest request)
        {
            try
            {
                var teacher = await _adminService.CreateTeacherAsync(request);
                return Ok(new { 
                    message = "Professor criado com sucesso",
                    teacherId = teacher.Id,
                    name = teacher.Name,
                    email = teacher.Email
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

        [HttpPost("students")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
        {
            try
            {
                var student = await _adminService.CreateStudentAsync(request);
                return Ok(new { 
                    message = "Estudante criado com sucesso",
                    studentId = student.Id,
                    name = student.Name,
                    email = student.Email,
                    birthDate = student.BirthDate
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

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(new { 
                    message = "Lista de usuários",
                    data = users.Select(u => new {
                        id = u.Id,
                        name = u.Name,
                        email = u.Email,
                        profile = u.Profile.ToString()
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _adminService.GetUserByIdAsync(id);
                return Ok(new {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    profile = user.Profile.ToString()
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

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _adminService.DeleteUserAsync(id);
                return Ok(new { message = $"Usuário {id} removido com sucesso" });
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

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                var stats = new
                {
                    totalUsers = users.Count(),
                    totalStudents = users.Count(u => u.Profile == UserProfileType.Student),
                    totalTeachers = users.Count(u => u.Profile == UserProfileType.Teacher),
                    totalAdmins = users.Count(u => u.Profile == UserProfileType.Admin)
                };
                
                return Ok(new { 
                    message = "Estatísticas do sistema",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }
    }  
}