using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancesTracker.Application.DTOs;
using FinancesTracker.Application.Managers;

namespace FinancesTracker.Web.Pages.Accounts
{
    [IgnoreAntiforgeryToken]
    public class IndexModel(AccountManager accountManager) : PageModel
    {
        private readonly AccountManager _accountManager = accountManager;
        public List<AccountDto> Accounts { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Type { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal? MinBalance { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal? MaxBalance { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Currency { get; set; }
        [BindProperty]
        public AccountDto Account { get; set; } = new();
        [BindProperty]
        public List<int> DeleteIds { get; set; } = new();
        [BindProperty]
        public string? AccountIds { get; set; }

        private async Task<List<AccountDto>> GetFilteredAccounts(string? search, string? type, decimal? minBalance, decimal? maxBalance, string? createdFrom, string? createdTo)
        {
            var filter = new AccountFilterDto
            {
                Name = search,
                MinBalance = minBalance,
                MaxBalance = maxBalance
                // Add more filter properties as needed
            };
            return (await _accountManager.GetAccountsFilteredAsync(filter)).ToList();
        }

        public async Task<IActionResult> OnGetApiAsync(string? search, string? type, decimal? minBalance, decimal? maxBalance, string? createdFrom, string? createdTo)
        {
            var accounts = await GetFilteredAccounts(search, type, minBalance, maxBalance, createdFrom, createdTo);
            return new JsonResult(accounts);
        }

        public async Task OnGetAsync()
        {
            Accounts = await GetFilteredAccounts(Search, Type, MinBalance, MaxBalance, null, null);
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            try
            {
                await _accountManager.AddAsync(Account);
                TempData["SuccessMessage"] = "Account added successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            try
            {
                await _accountManager.UpdateAsync(Account);
                TempData["SuccessMessage"] = "Account updated successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(AccountIds))
                {
                    var ids = AccountIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    foreach (var id in ids)
                    {
                        await _accountManager.RemoveAsync(id);
                    }
                }
                TempData["SuccessMessage"] = "Account(s) deleted successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Page();
            }
        }
    }
}
