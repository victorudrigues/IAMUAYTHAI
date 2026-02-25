using IAMUAYTHAI.Application.Abstractions.Features.Checkin.ViewModel;
using IAMUAYTHAI.Application.Abstractions.Features.Evolution.ViewModel;

namespace IAMUAYTHAI.Application.Abstractions.Features.Student.ViewModel
{
    public class StudentDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public List<CheckinViewModel> Checkins { get; set; } = new();
        public EvolutionViewModel? CurrentEvolution { get; set; }
        public List<StudentClassViewModel> StudentClasses { get; set; } = new();
    }
}
