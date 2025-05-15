namespace ManagementSystem.Shared.Common.Entities
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
