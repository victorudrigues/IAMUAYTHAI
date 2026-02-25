using IAMUAYTHAI.Application.Abstractions;

namespace IAMUAYTHAI.Application.Abstractions.Features.StudentClass.Repository
{
    public interface IStudentClassRepository : IRepository<Domain.Aggregates.StudentClassAggregate.StudentClass>
    {
        Task<IEnumerable<Domain.Aggregates.StudentClassAggregate.StudentClass>> GetByClassIdsAsync(IEnumerable<int> classIds);
        Task<IEnumerable<Domain.Aggregates.StudentClassAggregate.StudentClass>> GetByStudentIdAsync(int studentId);
    }
}
