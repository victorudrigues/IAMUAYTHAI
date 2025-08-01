using IAMUAYTHAI.Application.Abstractions.Features.Student.Repository;
using StudentDomain = IAMUAYTHAI.Domain.Aggregates.StudentAggregate.Student;

namespace IAMUAYTHAI.Infra.Features.Student.Repository
{
    public class StudentRepository(Context context) : Repository<StudentDomain>(context), IStudentRepository
    {
    }
}
