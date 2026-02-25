namespace IAMUAYTHAI.Application.Abstractions.Features.Checkin.ViewModel
{
    public class CheckinResultViewModel
    {
        public string Message { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
