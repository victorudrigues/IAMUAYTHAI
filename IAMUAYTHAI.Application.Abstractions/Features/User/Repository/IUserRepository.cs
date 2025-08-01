using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAMUAYTHAI.Domain.Enumerations;

namespace IAMUAYTHAI.Application.Abstractions.Features.User.Repository
{
    public interface IUserRepository : IRepository<Domain.Aggregates.UserAggregate.User>
    {
        Task<Domain.Aggregates.UserAggregate.User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<Domain.Aggregates.UserAggregate.User>> GetByProfileAsync(UserProfileType profile);
        Task<Domain.Aggregates.UserAggregate.User?> GetWithDetailsAsync(int id);
        Task<IEnumerable<Domain.Aggregates.UserAggregate.User>> GetByIdsAsync(List<string> ids);
    }
}
