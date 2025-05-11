using FinancesTracker.Domain.Entities;
using FinancesTracker.Domain.Repositories;
using FinancesTracker.Infrastructure.Data;

namespace FinancesTracker.Infrastructure.Repositories
{
    public class AccountRepository(FinancesTrackerDbContext context) : Repository<Account>(context), IAccountRepository
    {
    }
}
