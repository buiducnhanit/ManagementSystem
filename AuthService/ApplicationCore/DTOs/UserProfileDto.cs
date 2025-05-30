namespace ApplicationCore.DTOs
{
    public class UserProfileDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
