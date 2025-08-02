using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Request
{
    public class PasswordRequest : IDisposable
    {
        private Memory<char> _passwordMemory;
        private bool _disposed;

        /// <summary>
        /// Define a senha de forma segura usando Memory<char>
        /// </summary>
        public void SetPassword(ReadOnlySpan<char> password)
        {
            // Limpa memória anterior se existir
            ClearPassword();

            // Aloca nova memória e copia a senha
            _passwordMemory = new char[password.Length];
            password.CopyTo(_passwordMemory.Span);
        }

        /// <summary>
        /// Obtém a senha como ReadOnlySpan<char> (mais seguro)
        /// </summary>
        public ReadOnlySpan<char> GetPasswordSpan()
        {
            return _passwordMemory.Span;
        }

        /// <summary>
        /// Limpa a senha da memória
        /// </summary>
        public void ClearPassword()
        {
            if (!_passwordMemory.IsEmpty)
            {
                _passwordMemory.Span.Clear();
                _passwordMemory = Memory<char>.Empty;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                ClearPassword();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PasswordRequest()
        {
            Dispose(false);
        }

    }
}
