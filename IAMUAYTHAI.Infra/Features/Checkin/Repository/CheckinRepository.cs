using IAMUAYTHAI.Application.Abstractions.Features.Checkin.Repository;
using CheckinDomain = IAMUAYTHAI.Domain.Aggregates.CheckinAggregate.Checkin;
using Microsoft.EntityFrameworkCore;

namespace IAMUAYTHAI.Infra.Features.Checkin.Repository
{
    public class CheckinRepository(Context context) : Repository<CheckinDomain>(context), ICheckinRepository
    {
        public async Task<IEnumerable<CheckinDomain>> GetByStudentIdAsync(int studentId)
        {
            return await _dbSet
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();
        }
    }
}
