namespace ManagementSystem.Shared.Common.Entities
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
