using Microsoft.AspNetCore.Identity;

namespace backend.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties will be added later

        public User()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}