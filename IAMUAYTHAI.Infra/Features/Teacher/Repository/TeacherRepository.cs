using IAMUAYTHAI.Application.Abstractions.Features.Teacher.Repository;


namespace IAMUAYTHAI.Infra.Features.Teacher.Repository
{
    public class TeacherRepository(Context context) : Repository<Domain.Aggregates.TeacherAggregate.Teacher>(context), ITeacherRepository
    {
    }
}
