using IAMUAYTHAI.Application.Abstractions.Features.Student.Repository;
using StudentDomain = IAMUAYTHAI.Domain.Aggregates.StudentAggregate.Student;
using Microsoft.EntityFrameworkCore;

namespace IAMUAYTHAI.Infra.Features.Student.Repository
{
    public class StudentRepository(Context context) : Repository<StudentDomain>(context), IStudentRepository
    {
        public async Task<IEnumerable<StudentDomain>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.ToList();
            if (idList.Count == 0)
                return [];

            return await _dbSet
                .Where(s => idList.Contains(s.Id))
                .ToListAsync();
        }
    }
}
