using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class CreateRoleRequest
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
