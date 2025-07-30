using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Schedule.Repository
{
    public interface IScheduleRepository : IRepository<Domain.Aggregates.ScheduleAggregate.Schedule>
    {
    }
}
