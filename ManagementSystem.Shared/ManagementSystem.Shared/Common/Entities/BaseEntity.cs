namespace ManagementSystem.Shared.Common.Entities
{
    public abstract class BaseEntity<TKey> : IBaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
