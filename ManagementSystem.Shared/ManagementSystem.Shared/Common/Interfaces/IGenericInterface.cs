using ManagementSystem.Shared.Common.Entities;

namespace ManagementSystem.Shared.Common.Interfaces
{
    public interface IGenericInterface<T> where T : class, IEntity<Guid>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
