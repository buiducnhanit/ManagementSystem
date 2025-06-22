namespace ManagementSystem.Shared.Contracts
{
    public class UserDeletedEvent
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
    }
}
