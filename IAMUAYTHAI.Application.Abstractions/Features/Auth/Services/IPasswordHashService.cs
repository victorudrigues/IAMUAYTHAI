namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Services
{
    public interface IPasswordHashService
    {
        string HashPassword(ReadOnlySpan<char> password);
        bool VerifyPassword(ReadOnlySpan<char> password, string hash);
    }
}