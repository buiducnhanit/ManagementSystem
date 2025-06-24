using System.ComponentModel.DataAnnotations;

namespace ManagementSystem.Shared.Contracts
{
    public class UnLockUserEvent
    {
        [Required]
        public string Id { get; set; } = string.Empty;
    }
}
