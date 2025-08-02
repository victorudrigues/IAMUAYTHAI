using System.ComponentModel.DataAnnotations;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Request
{
    public class ChangePasswordRequest 
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
