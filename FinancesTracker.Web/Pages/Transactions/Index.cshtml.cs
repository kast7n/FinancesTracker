using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancesTracker.Application.Managers;
using FinancesTracker.Application.DTOs;

namespace FinancesTracker.Web.Pages.Transactions
{
    public class IndexModel(TransactionManager transactionManager, AccountManager accountManager) : PageModel
    {
        private readonly TransactionManager _transactionManager = transactionManager;
        private readonly AccountManager _accountManager = accountManager;
        public TransactionsPageViewModel ViewModel { get; set; } = new();

        [BindProperty]
        public TransactionAddEditDto? Transaction { get; set; }

        public async Task OnGetAsync()
        {
            var transactions = await _transactionManager.GetAllAsync();
            var accounts = (await _accountManager.GetAccountsFilteredAsync(new AccountFilterDto())).ToList();
            // Extract unique categories from transactions
            var categories = transactions
                .Select(t => t.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            ViewModel = new TransactionsPageViewModel
            {
                Transactions = transactions,
                Categories = categories,
                Accounts = accounts, // Pass full AccountDto list
                Currencies = ["LBP", "USD", "EUR"]
            };
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (Transaction == null)
            {
                ModelState.AddModelError(string.Empty, "DTO is null! Model binding failed at the deserialization step.");
                await PopulateViewModelAsync();
                return Page();
            }
            if (!ModelState.IsValid)
            {
                await PopulateViewModelAsync();
                return Page();
            }
            try
            {
                var transactionDto = new TransactionDto
                {
                    TransactionID = Transaction.TransactionID,
                    Date = Transaction.Date,
                    Type = Transaction.Type,
                    Category = Transaction.Category,
                    Amount = Transaction.Amount,
                    AccountID = Transaction.AccountID,
                    Notes = Transaction.Notes
                };
                await _transactionManager.AddAsync(transactionDto);
                TempData["SuccessMessage"] = "Transaction added successfully.";
                return RedirectToPage();
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateViewModelAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (Transaction == null)
            {
                ModelState.AddModelError(string.Empty, "DTO is null! Model binding failed at the deserialization step.");
                await PopulateViewModelAsync();
                return Page();
            }
            if (!ModelState.IsValid)
            {
                await PopulateViewModelAsync();
                return Page();
            }
            try
            {
                var transactionDto = new TransactionDto
                {
                    TransactionID = Transaction.TransactionID,
                    Date = Transaction.Date,
                    Type = Transaction.Type,
                    Category = Transaction.Category,
                    Amount = Transaction.Amount,
                    AccountID = Transaction.AccountID,
                    Notes = Transaction.Notes
                };
                await _transactionManager.UpdateAsync(transactionDto);
                TempData["SuccessMessage"] = "Transaction updated successfully.";
                return RedirectToPage();
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateViewModelAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRemoveAsync([FromBody] List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                    await _transactionManager.RemoveAsync(id);
                return new JsonResult(new { success = true, message = "Transaction(s) deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        private async Task PopulateViewModelAsync()
        {
            var transactions = await _transactionManager.GetAllAsync();
            var accounts = (await _accountManager.GetAccountsFilteredAsync(new AccountFilterDto())).ToList();
            var categories = transactions
                .Select(t => t.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            ViewModel = new TransactionsPageViewModel
            {
                Transactions = transactions,
                Categories = categories,
                Accounts = accounts,
                Currencies = ["LBP", "USD", "EUR"]
            };
        }
    }
}
