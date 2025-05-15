using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DataType? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
