using IAMUAYTHAI.Application.Abstractions;
using IAMUAYTHAI.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;


namespace IAMUAYTHAI.Infra.Features
{
    public class Repository<T>(Context context, DbSet<T> dbSet) : IRepository<T> where T : Entity
    {
        protected readonly Context _context = context;
        protected readonly DbSet<T> _dbSet = dbSet;

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        //public virtual async Task SaveChangesAsync()
        //{
        //    await _context.SaveEntitiesAsync();
        //}
    }
}
