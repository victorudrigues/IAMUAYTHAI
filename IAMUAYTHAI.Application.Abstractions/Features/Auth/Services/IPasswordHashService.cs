namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Services
{
    public interface IPasswordHashService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}