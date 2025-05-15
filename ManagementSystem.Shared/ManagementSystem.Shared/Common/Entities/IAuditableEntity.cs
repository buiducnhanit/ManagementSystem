namespace ManagementSystem.Shared.Common.Entities
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
