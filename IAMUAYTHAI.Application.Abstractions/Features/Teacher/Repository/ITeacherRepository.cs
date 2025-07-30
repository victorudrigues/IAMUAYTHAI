using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Teacher.Repository
{
    public interface ITeacherRepository : IRepository<Domain.Aggregates.TeacherAggregate.Teacher>
    {
    }
}
