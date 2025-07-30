using IAMUAYTHAI.Domain.Enumerations;

namespace IAMUAYTHAI.Domain.Aggregates.UserAggregate
{
    public class User : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserProfile Profile { get; set; }
    }
}