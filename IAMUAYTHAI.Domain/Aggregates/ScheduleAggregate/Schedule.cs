using System;

namespace IAMUAYTHAI.Domain.Aggregates.ScheduleAggregate
{
    public class Schedule : Entity
    {
        public int TeacherId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}