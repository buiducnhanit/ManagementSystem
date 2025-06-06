using ManagementSystem.Shared.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManagementSystem.Shared.Common.Interfaces
{
    public interface IGenericInterface<TEntity, TKey, TDbContext>
        where TEntity : IBaseEntity<TKey>
        where TKey : IEquatable<TKey>
        where TDbContext : DbContext
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteByIdAsync(TKey id);
        Task SoftDeleteAsync(TEntity entity);
        Task SoftDeleteByIdAsync(TKey id);
        Task<int> SaveChangesAsync();
    }
}
