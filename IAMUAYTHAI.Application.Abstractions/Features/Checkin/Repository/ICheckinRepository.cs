using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Checkin.Repository
{
    public interface ICheckinRepository : IRepository<Domain.Aggregates.CheckinAggregate.Checkin>
    {
    }
}