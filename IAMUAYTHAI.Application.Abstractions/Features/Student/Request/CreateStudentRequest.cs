using System.ComponentModel.DataAnnotations;

namespace IAMUAYTHAI.Application.Abstractions.Features.Student.Request
{
    public class CreateStudentRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
        public DateTime BirthDate { get; set; }
    }
}