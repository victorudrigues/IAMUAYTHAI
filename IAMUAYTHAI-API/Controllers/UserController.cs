using IAMUAYTHAI.Application.Abstractions.Features.User.Request;
using IAMUAYTHAI.Application.Abstractions.Features.User.Service;
using Microsoft.AspNetCore.Mvc;

namespace IAMUAYTHAI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service;

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserResquest user)
        {
            try
            {
                await _service.RegisterAsync(user);
                return Ok(new { message = "Usuário criado com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _service.GetByIdAsync(id);
                return Ok(user);
            }
            catch (InvalidOperationException)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}