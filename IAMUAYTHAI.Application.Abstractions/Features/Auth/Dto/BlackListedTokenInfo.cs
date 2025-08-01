using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Auth.Dto
{
    public class BlackListedTokenInfo
    {
        public string TokenId { get; set; } = string.Empty;
        public DateTime BlacklistedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}