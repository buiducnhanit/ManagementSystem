namespace ManagementSystem.Shared.Common.Entities
{
    public interface IBaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        bool IsDeleted { get; set; }
    }
}
