using IAMUAYTHAI.Application.Abstractions.Features.Checkin.Repository;
using CheckinDomain = IAMUAYTHAI.Domain.Aggregates.CheckinAggregate.Checkin;
namespace IAMUAYTHAI.Infra.Features.Checkin.Repository
{
    public class CheckinRepository(Context context) : Repository<CheckinDomain>(context), ICheckinRepository
    {
    }
}
