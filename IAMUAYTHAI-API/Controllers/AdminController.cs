using IAMUAYTHAI.Application.Abstractions.Features.Admin.Service;
using IAMUAYTHAI.Application.Abstractions.Features.Admin.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Student.Request;
using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Request;
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
                return Ok(teacher);
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

        [HttpPost("students")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
        {
            try
            {
                var student = await _adminService.CreateStudentAsync(request);
                return Ok((student));
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

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorViewModel { Message = "Erro interno do servidor", Details = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _adminService.GetUserByIdAsync(id);
                return Ok((user));
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

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _adminService.DeleteUserAsync(id);
                return Ok(new MessageResultViewModel { Message = $"Usu?rio {id} removido com sucesso" });
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

        //[HttpGet("statistics")]
        //public async Task<IActionResult> GetStatistics()
        //{
        //    try
        //    {
        //        var users = await _adminService.GetAllUsersAsync();
        //        var stats = new StatisticsViewModel
        //        {
        //            TotalUsers = users.Count(),
        //            TotalStudents = users.Count(u => u.Profile == UserProfileType.Student),
        //            TotalTeachers = users.Count(u => u.Profile == UserProfileType.Teacher),
        //            TotalAdmins = users.Count(u => u.Profile == UserProfileType.Admin)
        //        };
        //        return Ok(stats);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiErrorViewModel { Message = "Erro interno do servidor", Details = ex.Message });
        //    }
        //}
    }
}
