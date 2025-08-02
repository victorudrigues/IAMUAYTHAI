using IAMUAYTHAI.Application.Abstractions.Features.Auth.Services;
using System.Security.Cryptography;
using System.Text;

namespace IAMUAYTHAI.Infra.Auth.Services
{
    public class PasswordHashService : IPasswordHashService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;
        
        public string HashPassword(ReadOnlySpan<char> password)
        {
            // Convertendo para UTF8 de forma segura
            var maxByteCount = Encoding.UTF8.GetMaxByteCount(password.Length);
            Span<byte> passwordBytes = maxByteCount <= 1024 
                ? stackalloc byte[maxByteCount] 
                : new byte[maxByteCount];

            try
            {
                var actualByteCount = Encoding.UTF8.GetBytes(password, passwordBytes);
                var actualPasswordBytes = passwordBytes[..actualByteCount];

                // Gera salt aleatório
                Span<byte> salt = stackalloc byte[SaltSize];
                RandomNumberGenerator.Fill(salt);

                // Gera hash usando PBKDF2
                Span<byte> hash = stackalloc byte[HashSize];
                Rfc2898DeriveBytes.Pbkdf2(actualPasswordBytes, salt, hash, Iterations, HashAlgorithmName.SHA256);

                // Combina salt + hash
                Span<byte> combined = stackalloc byte[SaltSize + HashSize];
                salt.CopyTo(combined);
                hash.CopyTo(combined[SaltSize..]);

                return Convert.ToBase64String(combined);
            }
            finally
            {
                // Limpando dados sensíveis da memória
                passwordBytes.Clear();
            }
        }

        public bool VerifyPassword(ReadOnlySpan<char> password, string hash)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(hash);
                if (hashBytes.Length != SaltSize + HashSize)
                    return false;

                // Extraindo salt e hash armazenados
                ReadOnlySpan<byte> salt = hashBytes.AsSpan(0, SaltSize);
                ReadOnlySpan<byte> storedHash = hashBytes.AsSpan(SaltSize);

                // Convertendo senha para bytes de forma segura
                var maxByteCount = Encoding.UTF8.GetMaxByteCount(password.Length);
                Span<byte> passwordBytes = maxByteCount <= 1024 
                    ? stackalloc byte[maxByteCount] 
                    : new byte[maxByteCount];

                try
                {
                    var actualByteCount = Encoding.UTF8.GetBytes(password, passwordBytes);
                    var actualPasswordBytes = passwordBytes[..actualByteCount];

                    // Calculando hash da senha
                    Span<byte> computedHash = stackalloc byte[HashSize];
                    Rfc2898DeriveBytes.Pbkdf2(actualPasswordBytes, salt, computedHash, Iterations, HashAlgorithmName.SHA256);

                    // Comparação segura usando SequenceEqual
                    return storedHash.SequenceEqual(computedHash);
                }
                finally
                {
                    // Limpa dados sensíveis
                    passwordBytes.Clear();
                }
            }
            catch
            {
                return false;
            }
        }
    }
}