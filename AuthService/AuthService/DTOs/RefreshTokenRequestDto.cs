using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
