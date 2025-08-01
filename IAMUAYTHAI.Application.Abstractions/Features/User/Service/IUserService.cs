using IAMUAYTHAI.Application.Abstractions.Features.User.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain = IAMUAYTHAI.Domain.Aggregates.UserAggregate.User;

namespace IAMUAYTHAI.Application.Abstractions.Features.User.Service
{
    public interface IUserService
    {
        Task RegisterAsync(UserResquest request);
        Task<UserDomain> GetByIdAsync(int id);
    }
}
