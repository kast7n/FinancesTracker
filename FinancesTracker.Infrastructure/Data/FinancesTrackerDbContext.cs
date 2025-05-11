using FinancesTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancesTracker.Infrastructure.Data
{
    public class FinancesTrackerDbContext(DbContextOptions<FinancesTrackerDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any custom configuration here if needed
        }
    }
}
