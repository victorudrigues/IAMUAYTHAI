using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using Microsoft.EntityFrameworkCore;
using IAMUAYTHAI.Domain.Enumerations;
using UserDomain = IAMUAYTHAI.Domain.Aggregates.UserAggregate.User;

namespace IAMUAYTHAI.Infra.Features.User.Repository
{
    public class UserRepository(Context context) : Repository<UserDomain>(context), IUserRepository
    {
        public async Task<UserDomain?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<UserDomain>> GetByProfileAsync(UserProfileType profile)
        {
            return await _dbSet.Where(u => u.Profile == profile).ToListAsync();
        }

        public async Task<UserDomain?> GetWithDetailsAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<UserDomain>> GetByIdsAsync(List<string> ids)
        {
            return await _dbSet.Where(u => ids.Contains(u.Id.ToString())).ToListAsync();
        }
    }
}   
