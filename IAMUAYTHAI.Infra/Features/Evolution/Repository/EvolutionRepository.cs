using IAMUAYTHAI.Application.Abstractions.Features.Evolution.Repository;
using EvolutionDomain = IAMUAYTHAI.Domain.Aggregates.EvolutionAggregate.Evolution;
using Microsoft.EntityFrameworkCore;

namespace IAMUAYTHAI.Infra.Features.Evolution.Repository
{
    public class EvolutionRepository(Context context) : Repository<EvolutionDomain>(context), IEvolutionRepository
    {
        public async Task<EvolutionDomain?> GetCurrentByStudentIdAsync(int studentId)
        {
            return await _dbSet
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.Id)
                .FirstOrDefaultAsync();
        }
    }
}
