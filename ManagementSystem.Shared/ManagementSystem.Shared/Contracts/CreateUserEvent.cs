using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Contracts
{
    public class CreateUserEvent
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; } = null;
        public bool Gender { get; set; } = false;
    }
}
