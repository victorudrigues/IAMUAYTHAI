using IAMUAYTHAI.Application.Abstractions;
using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using Microsoft.EntityFrameworkCore;
using IAMUAYTHAI.Domain.Enumerations;

namespace IAMUAYTHAI.Infra.Features.User.Repository
{
    public class UserRepository(Context context) : Repository<Domain.Aggregates.UserAggregate.User>(context), IUserRepository
    {
        public async Task<Domain.Aggregates.UserAggregate.User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Domain.Aggregates.UserAggregate.User>> GetByProfileAsync(UserProfileType profile)
        {
            return await _dbSet.Where(u => u.Profile == profile).ToListAsync();
        }

        public async Task<Domain.Aggregates.UserAggregate.User?> GetWithDetailsAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}   
