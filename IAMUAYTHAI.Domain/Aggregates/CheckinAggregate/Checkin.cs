using System;

namespace IAMUAYTHAI.Domain.Aggregates.CheckinAggregate
{
    public class Checkin : Entity
    {
        public int StudentId { get; set; }
        public DateTime DateTime { get; set; }
    }
}