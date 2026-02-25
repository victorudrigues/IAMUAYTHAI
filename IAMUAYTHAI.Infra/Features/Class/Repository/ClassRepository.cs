using ClassDomain = IAMUAYTHAI.Domain.Aggregates.ClassAggregate.Class;
using IAMUAYTHAI.Application.Abstractions.Features.Class.Repository;
using Microsoft.EntityFrameworkCore;

namespace IAMUAYTHAI.Infra.Features.Class.Repository
{
    public class ClassRepository(Context context) : Repository<ClassDomain>(context), IClassRepository
    {
        public async Task<IEnumerable<ClassDomain>> GetByTeacherIdAsync(int teacherId)
        {
            return await _dbSet
                .Where(c => c.TeacherId == teacherId)
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();
        }
    }
}
