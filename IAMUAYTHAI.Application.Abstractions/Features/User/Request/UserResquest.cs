using IAMUAYTHAI.Domain.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.User.Request
{
    public class UserResquest
    {
        [FromQuery(Name = "n")]
        public string Name { get; set; } = string.Empty;

        [FromQuery(Name = "e")]
        public string Email { get; set; } = string.Empty;

        [FromQuery(Name = "p")]
        public string PasswordHash { get; set; } = string.Empty;

        [FromQuery(Name = "p")]
        required public int Profile { get; set; }

    }
}