using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class ManageUserRolesRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one role name is required.")]
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}
