namespace IAMUAYTHAI.Domain.Aggregates.CheckinAggregate
{
    public class Checkin
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime DateTime { get; set; }
    }
}