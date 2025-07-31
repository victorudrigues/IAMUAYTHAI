using IAMUAYTHAI.Domain.Aggregates.ClassAggregate;
using IAMUAYTHAI.Domain.Aggregates.StudentAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Domain.Aggregates.StudentClassAggregate
{
    public class StudentClass : Entity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int ClassId { get; set; }
        public Class Class { get; set; } = null!;

        public bool WasPresent { get; set; }
        public string? Justification { get; set; }
    }
}
