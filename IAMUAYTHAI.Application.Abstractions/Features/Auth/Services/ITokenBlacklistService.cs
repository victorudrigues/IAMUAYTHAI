using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Services
{
    public interface ITokenBlacklistService
    {
        Task AddToBlacklistAsync(string jti, DateTime expiration);
        Task<bool> IsTokenBlacklistedAsync(string jti);
        Task RemoveExpiredTokensAsync();
    }
}