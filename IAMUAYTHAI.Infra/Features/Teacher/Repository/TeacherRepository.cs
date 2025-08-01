using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Repository;
using TeacherDomain = IAMUAYTHAI.Domain.Aggregates.TeacherAggregate.Teacher;


namespace IAMUAYTHAI.Infra.Features.Teacher.Repository
{
    public class TeacherRepository(Context context) : Repository<TeacherDomain>(context), ITeacherRepository
    {
    }
}
