using System.ComponentModel.DataAnnotations;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Request
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}