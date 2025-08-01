namespace IAMUAYTHAI.Application.Abstractions.Features.Teacher.Request
{
    public class CreateTeacherRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}