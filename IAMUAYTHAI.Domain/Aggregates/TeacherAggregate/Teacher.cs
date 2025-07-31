using IAMUAYTHAI.Domain.Aggregates.ScheduleAggregate;
using IAMUAYTHAI.Domain.Aggregates.UserAggregate;
using System.Collections.Generic;

namespace IAMUAYTHAI.Domain.Aggregates.TeacherAggregate
{
    public class Teacher : User
    {
        public List<Schedule> Schedules { get; set; } = new();
    }
}