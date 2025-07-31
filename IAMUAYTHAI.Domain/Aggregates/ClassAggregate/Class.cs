using IAMUAYTHAI.Domain.Aggregates.StudentClassAggregate;
using IAMUAYTHAI.Domain.Aggregates.TeacherAggregate;
using System;
using System.Collections.Generic;

namespace IAMUAYTHAI.Domain.Aggregates.ClassAggregate
{
    public class Class : Entity
    {
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;        
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        public List<StudentClass> StudentClasses { get; set; } = new();
    }
}