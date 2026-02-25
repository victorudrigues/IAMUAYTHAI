using IAMUAYTHAI.Application.Abstractions.Features.Class.ViewModel;

namespace IAMUAYTHAI.Application.Abstractions.Features.Student.ViewModel
{
    public class StudentClassViewModel
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public ClassViewModel Class { get; set; } = null!;
        public bool WasPresent { get; set; }
        public string? Justification { get; set; }
    }
}
