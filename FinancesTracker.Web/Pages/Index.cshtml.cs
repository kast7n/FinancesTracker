using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancesTracker.Application.Managers;
using FinancesTracker.Application.DTOs;

namespace FinancesTracker.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AccountManager _accountManager;
        private readonly TransactionManager _transactionManager;

        public decimal TotalBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public List<CategorySummary> CategorySummaries { get; set; } = new();
        public List<TrendPoint> TrendData { get; set; } = new();
        public string TimeFilter { get; set; } = "month";

        public IndexModel(AccountManager accountManager, TransactionManager transactionManager)
        {
            _accountManager = accountManager;
            _transactionManager = transactionManager;
        }

        public async Task OnGetAsync(string? filter)
        {
            TimeFilter = filter ?? "month";
            var accounts = await _accountManager.GetAccountsFilteredAsync(new AccountFilterDto());
            TotalBalance = accounts.Sum(a => a.Balance);

            var transactions = (await _transactionManager.GetAllAsync()).ToList();
            if (TimeFilter == "week")
            {
                var start = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                transactions = transactions.Where(t => t.Date >= start).ToList();
            }
            else if (TimeFilter == "month")
            {
                var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                transactions = transactions.Where(t => t.Date >= start).ToList();
            }
            else if (TimeFilter == "year")
            {
                var start = new DateTime(DateTime.Today.Year, 1, 1);
                transactions = transactions.Where(t => t.Date >= start).ToList();
            }

            TotalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            TotalExpense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            CategorySummaries = transactions.GroupBy(t => t.Category)
                .Select(g => new CategorySummary { Category = g.Key, Amount = g.Sum(t => t.Amount), Type = g.First().Type })
                .ToList();
            TrendData = transactions
                .GroupBy(t => t.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new TrendPoint { Date = g.Key, Income = g.Where(t => t.Type == "Income").Sum(t => t.Amount), Expense = g.Where(t => t.Type == "Expense").Sum(t => t.Amount) })
                .ToList();
        }

        public class CategorySummary
        {
            public string Category { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Type { get; set; } = string.Empty;
        }
        public class TrendPoint
        {
            public DateTime Date { get; set; }
            public decimal Income { get; set; }
            public decimal Expense { get; set; }
        }
    }
}
