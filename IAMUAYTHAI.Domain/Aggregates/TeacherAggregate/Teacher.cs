using IAMUAYTHAI.Domain.Aggregates.ScheduleAggregate;
using IAMUAYTHAI.Domain.Aggregates.UserAggregate;

namespace IAMUAYTHAI.Domain.Aggregates.TeacherAggregate
{
    public class Teacher : User
    {
        public List<Schedule> Schedules { get; set; } = new();
    }
}