using IAMUAYTHAI.Application.Abstractions.Features.User.Request;
using UserDomain = IAMUAYTHAI.Domain.Aggregates.UserAggregate.User;

namespace IAMUAYTHAI.Application.Abstractions.Features.User.Service
{
    public interface IUserService
    {
        Task<UserDomain> GetByIdAsync(int id);
        Task<IEnumerable<UserDomain>> GetAllAsync();
        Task DeleteAsync(List<string> ids);
    }
}
