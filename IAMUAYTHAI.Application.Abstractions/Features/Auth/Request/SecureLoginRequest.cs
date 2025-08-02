using System.ComponentModel.DataAnnotations;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Request
{
    public class SecureLoginRequest : PasswordRequest
    {
        private Memory<char> _passwordMemory;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public new void SetPassword(ReadOnlySpan<char> password)
        {
            _passwordMemory = new char[password.Length];
            password.CopyTo(_passwordMemory.Span);
        }

        public ReadOnlySpan<char> GetPassword() => _passwordMemory.Span;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _passwordMemory.Span.Clear();
            }
            base.Dispose(disposing);
        }
    }
}
