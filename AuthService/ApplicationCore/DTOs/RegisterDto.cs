using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MinLength(3)]
        public string UserName { get; set; } = string.Empty;

        [MinLength(3)]
        public string? FirstName { get; set; } = null;

        [MinLength(3)]
        public string? LastName { get; set; } = null;

        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,20}$", ErrorMessage = "Password must be 6-20 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; } = null;

        [MaxLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.'-]{3,}$", ErrorMessage = "Address must be at least 3 characters long and can include letters, numbers, spaces, and certain punctuation marks.")]
        public string? Address { get; set; } = null;

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [RegularExpression(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$", ErrorMessage = "Date of Birth must be in the format dd/MM/yyyy.")]
        public DateTime? dateOfBirth { get; set; } = null;

        [Required]
        [Display(Name = "Gender")]
        public bool gender { get; set; } = false;
    }
}
