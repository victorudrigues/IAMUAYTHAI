using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.User.Request;
using IAMUAYTHAI.Application.Abstractions.Features.User.Service;
using IAMUAYTHAI.Domain.Aggregates.UserAggregate;
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
            var response = await _service.RegisterAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = response.Id}, response);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var users = await _userRepository.GetAllAsync();
        //    return Ok(users);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}