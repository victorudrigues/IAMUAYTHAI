using ClassDomain = IAMUAYTHAI.Domain.Aggregates.ClassAggregate.Class;
using IAMUAYTHAI.Application.Abstractions.Features.Class.Repository;

namespace IAMUAYTHAI.Infra.Features.Class.Repository
{
    public class ClassRepository(Context context) : Repository<ClassDomain>(context), IClassRepository
    {
    }
}
