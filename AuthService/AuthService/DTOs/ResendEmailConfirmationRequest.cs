using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class ResendEmailConfirmationRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;
    }
}
