using FinancesTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancesTracker.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FinancesTrackerDbContext context)
        {
            context.Database.Migrate();

            // Accounts
            if (!context.Accounts.Any())
            {
                var accounts = new List<Account>
                {
                    new() { Name = "Main Bank", Balance = 5000, Currency = "USD" },
                    new() { Name = "Savings", Balance = 12000, Currency = "USD" },
                    new() { Name = "Cash Wallet", Balance = 200, Currency = "USD" },
                    new() { Name = "Credit Card", Balance = 0, Currency = "USD" },
                    new() { Name = "Travel Fund", Balance = 1500, Currency = "EUR" },
                    new() { Name = "Investment", Balance = 8000, Currency = "USD" },
                    new() { Name = "Emergency Fund", Balance = 3000, Currency = "USD" },
                    new() { Name = "Joint Account", Balance = 4000, Currency = "USD" },
                    new() { Name = "Business Account", Balance = 10000, Currency = "USD" },
                    new() { Name = "Petty Cash", Balance = 100, Currency = "USD" },
                    new() { Name = "Gift Card", Balance = 50, Currency = "USD" },
                    new() { Name = "Online Wallet", Balance = 600, Currency = "USD" },
                    new() { Name = "Crypto Wallet", Balance = 0.5m, Currency = "BTC" },
                    new() { Name = "Euro Account", Balance = 2500, Currency = "EUR" },
                    new() { Name = "Pension Fund", Balance = 20000, Currency = "USD" }
                };
                context.Accounts.AddRange(accounts);
                context.SaveChanges();
            }

            // Transactions
            if (!context.Transactions.Any())
            {
                var transactions = new List<Transaction>();
                var accountIds = context.Accounts.Select(a => a.AccountID).ToList();
                var categories = new[] { "Salary", "Rent", "Food", "Utilities", "Shopping", "Travel", "Investment", "Gift", "Health", "Education", "Entertainment", "Insurance", "Tax", "Transfer", "Other" };
                var rand = new Random();
                for (int i = 0; i < 15; i++)
                {
                    transactions.Add(new Transaction
                    {
                        Date = DateTime.Now.AddDays(-i * 2),
                        Type = i % 2 == 0 ? "Income" : "Expense",
                        Category = categories[i % categories.Length],
                        Amount = rand.Next(50, 2000),
                        AccountID = accountIds[rand.Next(accountIds.Count)],
                        Notes = i % 3 == 0 ? "Sample note" : null
                    });
                }
                context.Transactions.AddRange(transactions);
                context.SaveChanges();
            }

        }
    }
}
