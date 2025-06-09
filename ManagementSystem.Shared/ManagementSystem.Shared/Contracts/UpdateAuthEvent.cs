namespace ManagementSystem.Shared.Contracts
{
    public class UpdateAuthEvent
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
