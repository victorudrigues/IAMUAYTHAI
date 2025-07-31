using IAMUAYTHAI.Domain.Enumerations;

namespace IAMUAYTHAI.Domain.Aggregates.UserAggregate
{
    public class User : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserProfileType Profile { get; set; }


        public static User FromRequest(string name, string email, string passwordHash, int profile)
        {
            return new User
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                Profile = (UserProfileType)profile
            };
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(PasswordHash) &&
                   Enum.IsDefined(typeof(UserProfileType), Profile);
        }
    }
}