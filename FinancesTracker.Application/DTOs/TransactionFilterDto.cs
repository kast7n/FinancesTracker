namespace FinancesTracker.Application.DTOs
{
    public class TransactionFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Type { get; set; }
        public string? Category { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public int? AccountID { get; set; }
    }
}
