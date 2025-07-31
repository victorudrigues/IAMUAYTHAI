using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Student.Repository
{
    public interface IStudentRepository : IRepository<Domain.Aggregates.StudentAggregate.Student>
    {
    }
}
