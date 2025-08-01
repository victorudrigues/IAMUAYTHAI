using IAMUAYTHAI.Application.Abstractions.Features.Student.Repository;

namespace IAMUAYTHAI.Infra.Features.Student.Repository
{
    public class StudentRepository(Context context) : Repository<Domain.Aggregates.StudentAggregate.Student>(context), IStudentRepository
    {
    }
}
