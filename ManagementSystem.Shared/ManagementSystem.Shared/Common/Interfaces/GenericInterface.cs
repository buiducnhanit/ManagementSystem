using ManagementSystem.Shared.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManagementSystem.Shared.Common.Interfaces
{
    public class GenericInterface<TEntity, TKey> : IGenericInterface<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericInterface(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id) && !e.IsDeleted);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).Where(e => !e.IsDeleted).ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteByIdAsync(TKey id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Entity with ID {id} not found.");
            await DeleteAsync(entity);
        }

        public virtual async Task SoftDeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entity.IsDeleted = true;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task SoftDeleteByIdAsync(TKey id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Entity with ID {id} not found.");
            await SoftDeleteAsync(entity);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
