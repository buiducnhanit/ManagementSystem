namespace WebAPI.DTOs
{
    public class CreateUserRequest
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public List<string>? Roles { get; set; }
    }
}
