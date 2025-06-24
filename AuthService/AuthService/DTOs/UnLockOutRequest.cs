using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class UnLockOutRequest
    {
        [Required]
        public string Id { get; set; } = string.Empty;
    }
}
