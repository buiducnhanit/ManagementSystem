using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(256)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedDate { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReasonRevoked { get; set; }
        public DateTime LastUsed { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [NotMapped]
        public bool IsRevoked => RevokedDate.HasValue;

        [NotMapped]
        public bool IsActive => !IsRevoked && ExpiryTime > DateTime.UtcNow;
    }
}
