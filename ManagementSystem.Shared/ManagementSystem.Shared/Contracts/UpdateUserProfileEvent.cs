using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Contracts
{
    public class UpdateUserProfileEvent
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public List<string> Roles { get; set; } = [];
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
