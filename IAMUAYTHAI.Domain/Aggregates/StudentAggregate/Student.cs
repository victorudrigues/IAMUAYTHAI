using IAMUAYTHAI.Domain.Aggregates.CheckinAggregate;
using IAMUAYTHAI.Domain.Aggregates.ClassAggregate;
using IAMUAYTHAI.Domain.Aggregates.EvolutionAggregate;
using IAMUAYTHAI.Domain.Aggregates.UserAggregate;

namespace IAMUAYTHAI.Domain.Aggregates.StudentAggregate
{
    public class Student : User
    {
        public DateTime BirthDate { get; set; }
        public List<Checkin> Checkins { get; set; } = new();
        public Evolution? CurrentEvolution { get; set; }
        public List<Class> Classes { get; set; } = new();
    }
}