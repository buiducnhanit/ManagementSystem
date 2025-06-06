using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
