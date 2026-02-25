namespace IAMUAYTHAI.Application.Abstractions.Features.Class.ViewModel
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
