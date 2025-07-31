using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Class.Repository
{
    public interface IClassRepository : IRepository<Domain.Aggregates.ClassAggregate.Class>
    {
    }
}