using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.User.Repository
{
    public interface IUserRepository : IRepository<Domain.Aggregates.UserAggregate.User>
    {
    }
}
