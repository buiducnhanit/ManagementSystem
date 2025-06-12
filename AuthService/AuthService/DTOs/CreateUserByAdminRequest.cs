namespace AuthService.DTOs
{
    public class CreateUserByAdminRequest
    {
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
