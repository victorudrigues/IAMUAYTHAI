using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using IAMUAYTHAI.Application.Abstractions.Features.User.Service;
using UserDomain = IAMUAYTHAI.Domain.Aggregates.UserAggregate.User;

namespace IAMUAYTHAI.Application.Features.User.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDomain> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new InvalidOperationException($"Usuário com ID {id} não encontrado.");
            
            return user;
        }

        public async Task<IEnumerable<UserDomain>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task DeleteAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                return;
            
            var users = await _userRepository.GetByIdsAsync(ids);

            if (!users.Any())
                return;
            
            _userRepository.DeleteRange(users);
            await _userRepository.SaveChangesAsync();
        }
    }
}
