using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Request
{
    public class SecureChangePasswordRequest : PasswordRequest
    {
        private Memory<char> _currentPasswordMemory;
        private Memory<char> _newPasswordMemory;
        private Memory<char> _confirmPasswordMemory;

        public void SetCurrentPassword(ReadOnlySpan<char> password)
        {
            _currentPasswordMemory = new char[password.Length];
            password.CopyTo(_currentPasswordMemory.Span);
        }

        public void SetNewPassword(ReadOnlySpan<char> password)
        {
            _newPasswordMemory = new char[password.Length];
            password.CopyTo(_newPasswordMemory.Span);
        }

        public void SetConfirmPassword(ReadOnlySpan<char> password)
        {
            _confirmPasswordMemory = new char[password.Length];
            password.CopyTo(_confirmPasswordMemory.Span);
        }

        public ReadOnlySpan<char> GetCurrentPassword() => _currentPasswordMemory.Span;
        public ReadOnlySpan<char> GetNewPassword() => _newPasswordMemory.Span;
        public ReadOnlySpan<char> GetConfirmPassword() => _confirmPasswordMemory.Span;

        /// <summary>
        /// Verifica se as senhas coincidem de forma segura
        /// </summary>
        public bool PasswordsMatch()
        {
            return GetNewPassword().SequenceEqual(GetConfirmPassword());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _currentPasswordMemory.Span.Clear();
                _newPasswordMemory.Span.Clear();
                _confirmPasswordMemory.Span.Clear();
            }
            base.Dispose(disposing);
        }
    }
}
