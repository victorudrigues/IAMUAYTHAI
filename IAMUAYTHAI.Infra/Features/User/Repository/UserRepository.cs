using IAMUAYTHAI.Application.Abstractions;
using IAMUAYTHAI.Application.Abstractions.Features.User.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Infra.Features.User.Repository
{
    public class UserRepository(Context context) : Repository<Domain.Aggregates.UserAggregate.User>(context), IUserRepository
    {
    }
}
