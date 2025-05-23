namespace FinancesTracker.Application.DTOs
{
    public class TransactionAddEditDto
    {
        public int TransactionID { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int AccountID { get; set; }
        public string? Notes { get; set; }
    }
}
