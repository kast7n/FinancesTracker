using FinancesTracker.Domain.Entities;
using FinancesTracker.Domain.Repositories;
using FinancesTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancesTracker.Infrastructure.Repositories
{
    public class TransactionRepository(FinancesTrackerDbContext context) : Repository<Transaction>(context), ITransactionRepository
    {
        
        public new async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _dbSet.Include(t => t.Account).ToListAsync();
        }
    }
}
