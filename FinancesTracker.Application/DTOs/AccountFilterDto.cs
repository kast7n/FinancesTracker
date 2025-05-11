namespace FinancesTracker.Application.DTOs
{
    public class AccountFilterDto
    {
        public string? Name { get; set; }
        public string? Currency { get; set; }
        public decimal? MinBalance { get; set; }
        public decimal? MaxBalance { get; set; }
    }
}
