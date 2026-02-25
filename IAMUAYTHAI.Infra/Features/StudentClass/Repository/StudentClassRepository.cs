using IAMUAYTHAI.Application.Abstractions.Features.StudentClass.Repository;
using StudentClassDomain = IAMUAYTHAI.Domain.Aggregates.StudentClassAggregate.StudentClass;
using Microsoft.EntityFrameworkCore;

namespace IAMUAYTHAI.Infra.Features.StudentClass.Repository
{
    public class StudentClassRepository(Context context) : Repository<StudentClassDomain>(context), IStudentClassRepository
    {
        public async Task<IEnumerable<StudentClassDomain>> GetByClassIdsAsync(IEnumerable<int> classIds)
        {
            var idList = classIds.ToList();
            if (idList.Count == 0)
                return [];

            return await _dbSet
                .Where(sc => idList.Contains(sc.ClassId))
                .Include(sc => sc.Student)
                .Include(sc => sc.Class)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentClassDomain>> GetByStudentIdAsync(int studentId)
        {
            return await _dbSet
                .Where(sc => sc.StudentId == studentId)
                .Include(sc => sc.Class)
                .ToListAsync();
        }
    }
}
