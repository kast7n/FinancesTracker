namespace FinancesTracker.Application.DTOs
{
    public class AccountDto
    {
        public int AccountID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
