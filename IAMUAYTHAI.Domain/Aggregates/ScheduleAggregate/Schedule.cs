namespace IAMUAYTHAI.Domain.Aggregates.ScheduleAggregate
{
    public class Schedule
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}