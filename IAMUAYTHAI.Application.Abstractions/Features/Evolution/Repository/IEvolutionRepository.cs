using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Evolution.Repository
{
    public interface IEvolutionRepository : IRepository<Domain.Aggregates.EvolutionAggregate.Evolution>
    {
        Task<Domain.Aggregates.EvolutionAggregate.Evolution?> GetCurrentByStudentIdAsync(int studentId);
    }
}
