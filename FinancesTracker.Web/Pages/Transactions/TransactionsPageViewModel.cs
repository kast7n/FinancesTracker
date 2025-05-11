using System.Collections.Generic;
using FinancesTracker.Application.DTOs;

namespace FinancesTracker.Web.Pages.Transactions
{
    public class TransactionsPageViewModel
    {
        public IEnumerable<TransactionDto> Transactions { get; set; } = [];
        public List<string> Categories { get; set; } = []; // Example filter dropdown
        public List<AccountDto> Accounts { get; set; } = new(); // For filter dropdown, now AccountDto
        public List<string> Currencies { get; set; } = []; // Example filter dropdown
        // Add more filter options as needed
    }
}
