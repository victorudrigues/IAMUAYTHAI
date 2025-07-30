namespace IAMUAYTHAI.Domain.Aggregates.UserAggregate
{
    public enum UserProfile
    {
        Student = 1,
        Teacher = 2
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserProfile Profile { get; set; }
    }
}