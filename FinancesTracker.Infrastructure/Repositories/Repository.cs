using FinancesTracker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinancesTracker.Infrastructure.Repositories
{
    public class Repository<T>(DbContext context) : IRepository<T> where T : class
    {
        protected readonly DbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public IQueryable<T> Query() => _dbSet;

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
