namespace IAMUAYTHAI.Application.Abstractions.Features.Student.Request
{
    public class CreateStudentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }
}