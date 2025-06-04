using ManagementSystem.Shared.Common.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DataType? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
