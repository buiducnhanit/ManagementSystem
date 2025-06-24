using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class UpdateRoleRequest
    {
        [Required]
        public string OldRoleName { get; set; } = string.Empty;

        [Required]
        public string NewRoleName { get; set; } = string.Empty;
    }
}
