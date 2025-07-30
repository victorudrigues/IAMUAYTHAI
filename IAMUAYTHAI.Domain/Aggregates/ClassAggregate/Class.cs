using IAMUAYTHAI.Domain.Aggregates.StudentAggregate;
using IAMUAYTHAI.Domain.Aggregates.TeacherAggregate;

namespace IAMUAYTHAI.Domain.Aggregates.ClassAggregate
{
    public class Class
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<Student> PresentStudents { get; set; } = new();
        public List<Student> AbsentStudents { get; set; } = new();
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
    }
}