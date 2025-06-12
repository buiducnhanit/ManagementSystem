using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage ="Email is invalid.")]
        public string Email { get; set; } = string.Empty;
    }
}
