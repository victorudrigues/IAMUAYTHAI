using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IAMUAYTHAI_API.Controllers
{
    [Authorize(Roles = "Student,Teacher,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        [HttpPost("checkin")]
        public async Task<IActionResult> SelfCheckin()
        {
            try
            {
                var studentId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();
                
                if (userRole != "Student")
                {
                    return Forbid("Apenas estudantes podem fazer self check-in");
                }

                // Lógica para self checkin
                return Ok(new { message = "Check-in realizado com sucesso" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("my-classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            try
            {
                var studentId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();
                
                // Estudantes veem suas aulas, professores e admins podem ver de qualquer estudante
                if (userRole == "Student")
                {
                    // Lógica para buscar aulas do estudante logado
                    return Ok(new { message = "Minhas aulas como estudante" });
                }
                else
                {
                    // Professores e Admins podem especificar o studentId via query parameter
                    var targetStudentId = HttpContext.Request.Query["studentId"].FirstOrDefault();
                    if (string.IsNullOrEmpty(targetStudentId))
                    {
                        return BadRequest("studentId é obrigatório para professores e admins");
                    }
                    
                    return Ok(new { message = $"Aulas do estudante {targetStudentId}" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("my-evolution")]
        public async Task<IActionResult> GetMyEvolution()
        {
            try
            {
                var studentId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();
                
                if (userRole == "Student")
                {
                    // Lógica para buscar evolução do estudante logado
                    return Ok(new { message = "Minha evolução como estudante" });
                }
                else
                {
                    // Professores e Admins podem especificar o studentId
                    var targetStudentId = HttpContext.Request.Query["studentId"].FirstOrDefault();
                    if (string.IsNullOrEmpty(targetStudentId))
                    {
                        return BadRequest("studentId é obrigatório para professores e admins");
                    }
                    
                    return Ok(new { message = $"Evolução do estudante {targetStudentId}" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("my-checkins")]
        public async Task<IActionResult> GetMyCheckins()
        {
            try
            {
                var studentId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();
                
                if (userRole == "Student")
                {
                    // Lógica para buscar check-ins do estudante logado
                    return Ok(new { message = "Meus check-ins como estudante" });
                }
                else
                {
                    // Professores e Admins podem especificar o studentId
                    var targetStudentId = HttpContext.Request.Query["studentId"].FirstOrDefault();
                    if (string.IsNullOrEmpty(targetStudentId))
                    {
                        return BadRequest("studentId é obrigatório para professores e admins");
                    }
                    
                    return Ok(new { message = $"Check-ins do estudante {targetStudentId}" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }
    }
}